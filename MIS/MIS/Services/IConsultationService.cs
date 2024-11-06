using Azure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS.Middleware;
using MIS.Models.DB;
using MIS.Models.DTO;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace MIS.Services
{
    public interface IConsultationService
    {
        Task<InspectionPagedListModel> GetList(
                [FromQuery] bool grouped,
                [FromQuery] List<Guid> icdRoots,
                [FromQuery] int page,
                [FromQuery] int size,
                ClaimsPrincipal user);
        Task<ConsultationModel> GetById(Guid id);
        Task<Guid> CreateById(Guid id, CommentCreateModel comment, ClaimsPrincipal user);
        Task<ActionResult> EditById(Guid id, InspectionCommentCreateModel newComment, ClaimsPrincipal user);
    }
    public class ConsultationService : IConsultationService
    {
        private readonly MisDbContext _context;

        public ConsultationService(MisDbContext context)
        {
            _context = context;
        }

        public async Task<InspectionPagedListModel> GetList(
                [FromQuery] bool grouped,
                [FromQuery] List<Guid> icdRoots,
                [FromQuery] int page,
                [FromQuery] int size,
                ClaimsPrincipal user)
        {
            // проверка на аутентификацию
            var doctorId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (doctorId == null || !Guid.TryParse(doctorId, out var parsedId))
            {
                throw new UnauthorizedAccessException();
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.id == parsedId);

            // проверка на пагинацию
            if (page < 1 || size < 1)
            {
                throw new ValidationAccessException();//ex
            }

            // проверка рутовых диагнозов
            if (icdRoots.Count == 0)
            {
                icdRoots = await _context.Icd10
                    .Where(p => p.parentId == null)
                    .Select(p => p.id)
                    .ToListAsync();
            }
            else
            {
                var roots  = await _context.Icd10
                    .Where(p => p.parentId == null)
                    .Select(p => p.id)
                    .ToListAsync(); 

                foreach (var root in icdRoots)
                {
                    if (!roots.Contains(root))
                    {
                        throw new ValidationAccessException();//ex
                    }
                }
            }
            
            var inspections = new List<InspectionPreviewModel>();

            if (grouped)
            {
                var query = _context.Inspections
                    .Include(i => i.doctor)
                    .Include(i => i.patient)
                    .Include(i => i.diagnoses);

                var groupedInspections = new List<DbInspection>();
                var allInspections = await query.ToListAsync();


                foreach (var inspection in allInspections)
                {
                    if (groupedInspections.Contains(inspection))
                    {
                        continue;
                    }
                    var chain = new List<DbInspection>();
                    var currentInspection = inspection;

                    while (currentInspection.previousInspectionId != null)
                    {
                        chain.Add(currentInspection);
                        currentInspection = allInspections.FirstOrDefault(i => i.id == currentInspection.previousInspectionId);
                        if (groupedInspections.Contains(currentInspection)) break;
                        chain.Add(currentInspection);
                    }
                    if (currentInspection != null)
                    {
                        groupedInspections.Add(currentInspection);
                    }
                    groupedInspections.AddRange(chain);
                }

                // дублирование кода (по-другому не знаю как сделать)
                var total = groupedInspections.Count();

                inspections = groupedInspections
                    .Skip((page - 1) * size)
                    .Take(size)
                    .Where(i =>
                    (i.diagnoses.Any(d => _context.Icd10.Any(b => b.code == d.code) && d.type == DiagnosisType.Main))
                    &&
                    i.consultations.Any(c => c.speciality.id == doctor.specialtyId))
                    .Select(i => new InspectionPreviewModel
                    {
                        id = i.id,
                        createTime = i.createTime,
                        previousId = i.previousInspectionId,
                        date = i.date,
                        conclusion = i.conclusion,
                        doctorId = i.doctor.id,
                        doctor = i.doctor.name,
                        patientId = i.patient.id,
                        patient = i.patient.name,
                        diagnosis = i.diagnoses
                                    .Where(b => b.type == DiagnosisType.Main)
                                    .Select(b => new DiagnosisModel
                                    {
                                        id = b.id,
                                        createTime = b.createTime,
                                        code = b.code,
                                        name = b.name,
                                        description = b.description,
                                        type = b.type
                                    })
                                    .FirstOrDefault(),
                        hasChain = (i.previousInspectionId != null),
                        hasNested = (i.nextVisitDate != null),
                    })
                    .ToList();

                var pageInf = new PageInfoModel
                {
                    size = size,
                    count = (int)Math.Ceiling((double)total / size),
                    current = page
                };

                return new InspectionPagedListModel
                {
                    inspections = inspections,
                    pagination = pageInf
                };
            }
            else
            {
                // запрос осмотров со специальностью доктора и мейн диагнозом соответстующим фильтрам
                var query = _context.Inspections
                    .Include(i => i.doctor)
                    .Include(i => i.patient)
                    .Include(i => i.diagnoses)
                        .Where(i =>
                                    (i.diagnoses.Any(d => _context.Icd10.Any(b => b.code == d.code) && d.type == DiagnosisType.Main))
                                    &&
                                    i.consultations.Any(c => c.speciality.id == doctor.specialtyId));

                var totalItems = await query.CountAsync();

                inspections = await query
                    .Skip((page - 1) * size)
                    .Take(size)
                    .Select(i => new InspectionPreviewModel
                    {
                        id = i.id,
                        createTime = i.createTime,
                        previousId = i.previousInspectionId,
                        date = i.date,
                        conclusion = i.conclusion,
                        doctorId = i.doctor.id,
                        doctor = i.doctor.name,
                        patientId = i.patient.id,
                        patient = i.patient.name,
                        diagnosis = i.diagnoses
                                    .Where(b => b.type == DiagnosisType.Main)
                                    .Select(b => new DiagnosisModel
                                    {
                                        id = b.id,
                                        createTime = b.createTime,
                                        code = b.code,
                                        name = b.name,
                                        description = b.description,
                                        type = b.type
                                    })
                                    .FirstOrDefault(),
                        hasChain = (i.previousInspectionId != null),
                        hasNested = (i.nextVisitDate != null),
                    })
                    .ToListAsync();

                var pageInfo = new PageInfoModel
                {
                    size = size,
                    count = (int)Math.Ceiling((double)totalItems / size),
                    current = page
                };

                return new InspectionPagedListModel
                {
                    inspections = inspections,
                    pagination = pageInfo
                };
            }
        }

        public async Task<ConsultationModel> GetById(Guid id)
        {
            var DBmodel = await _context.Consultations.FindAsync(id);

            if (DBmodel == null)
            {
                throw new KeyNotFoundException();//ex
            }

            var consultation = new ConsultationModel
            {
                createTime = DBmodel.createTime,
                inspectionId = DBmodel.inspectionId,
                speciality = new SpecialtyModel
                {
                    createTime = DBmodel.speciality.createTime,
                    id = DBmodel.speciality.id,
                    name = DBmodel.speciality.name,
                }
            };

            return consultation;

        }

        public async Task<Guid> CreateById (Guid id, CommentCreateModel comment, ClaimsPrincipal user)
        {
            // проверка на аутентификацию
            var doctorId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (doctorId == null || !Guid.TryParse(doctorId, out var parsedId))
            {
                throw new UnauthorizedAccessException(); //ex
            }

            // проверка на наличие консультации или родительского комментария
            var doctor = await _context.Doctors.FindAsync(parsedId);

            var consultation = await _context.Consultations.FindAsync(id);
            var parentComment = await _context.Comments.FindAsync(comment.parentId);
            if (consultation == null  || parentComment == null)
            {
                throw new KeyNotFoundException();// ex
            }

            // проверка доступа
            var inspection = await _context.Inspections
                .Where(i => i.id == consultation.inspectionId)
                .FirstOrDefaultAsync();

            if (consultation.speciality.id != doctor.specialtyId || inspection.doctor.id != doctor.id)
            {
                throw new ForbiddenAccessException();//ex
            }

            var newComment = new DbComment
            {
                createTime = DateTime.UtcNow,
                authorId = parsedId,
                author = doctor.name,
                content = comment.content,
                parentId = parentComment.id
            };

            consultation.comments.Add(newComment);

            return newComment.id;
        }

        public async Task<ActionResult> EditById (Guid id, InspectionCommentCreateModel newComment, ClaimsPrincipal user)
        {
            // проверка на аутентификацию
            var doctorId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (doctorId == null || !Guid.TryParse(doctorId, out var parsedId))
            {
                throw new UnauthorizedAccessException(); //ex
            }
            // проверка на наличие
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                throw new KeyNotFoundException();//ex
            }
            // проверка на право доступа
            if (comment.authorId != parsedId)
            {
                throw new ValidationAccessException();
            }

            else
            {
                comment.modifiedDate = DateTime.UtcNow;
                comment.content = newComment.content;
                await _context.SaveChangesAsync();
                return null;
            }  
        }
    }
}
