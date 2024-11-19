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
                throw new ValidationAccessException("page or size must be greater than 0");//ex
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
                var roots = await _context.Icd10
                    .Where(p => p.parentId == null)
                    .Select(p => p.id)
                    .ToListAsync();

                foreach (var root in icdRoots)
                {
                    if (!roots.Contains(root))
                    {
                        throw new ValidationAccessException("icdRoot not found");//ex
                    }
                }
            }

            var icdCodes = await _context.Icd10
                    .Where(i => icdRoots.Contains(i.id))
                    .Select(c => c.code)
                    .ToListAsync();
            var query = new List<DbInspection>();

            var inspections = new List<InspectionPreviewModel>();

            if (grouped)
            {
                query = await _context.Inspections
                   .Include(p => p.patient)
                   .Include(d => d.diagnoses)
                   .Include(d => d.doctor)
                   .Where(i => (i.diagnoses.Any(d => _context.Icd10.Any(b => b.code == d.code) && d.type == DiagnosisType.Main)) &&
                                i.consultations.Any(c => c.speciality.id == doctor.specialtyId) &&
                                i.previousInspectionId == null)
                   .Skip((page - 1) * size)
                   .Take(size)
                   .ToListAsync();
            }
            else
            {
                query = await _context.Inspections
                   .Include(p => p.patient)
                   .Include(d => d.diagnoses)
                   .Include(d => d.doctor)
                   .Include(c => c.consultations)
                   .Where(i => (i.diagnoses.Any(d => _context.Icd10.Any(b => b.code == d.code) && d.type == DiagnosisType.Main)) &&
                                i.consultations.Any(c => c.speciality.id == doctor.specialtyId))
                   .Skip((page - 1) * size)
                   .Take(size)
                   .ToListAsync();
            }

            var totalItems = query.Count();

            foreach (var inspection in query)
            {
                var previewInspection = new InspectionPreviewModel
                {
                    id = inspection.id,
                    createTime = inspection.createTime,
                    previousId = inspection.previousInspectionId,
                    date = inspection.date,
                    conclusion = inspection.conclusion,
                    doctorId = inspection.doctor.id,
                    doctor = inspection.doctor.name,
                    patientId = inspection.patient.id,
                    patient = inspection.patient.name,
                    diagnosis = inspection.diagnoses
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
                    hasChain = inspection.hasChain,
                    hasNested = inspection.hasNested
                };
                inspections.Add(previewInspection);
            }

            var pageInfo = new PageInfoModel
            {
                size = size,
                count = (int)Math.Ceiling((double)totalItems / size),
                current = page
            };

            if (pageInfo.current > pageInfo.count)
                throw new ValidationAccessException("cuurent page must be less than page count");

            return new InspectionPagedListModel
            {
                inspections = inspections,
                pagination = pageInfo
            };
        }

        public async Task<ConsultationModel> GetById(Guid id)
        {
            var DBmodel = await _context.InspetionConsultations
                .Include(s => s.speciality)
                .Include(r => r.rootComment)
                    .ThenInclude(a => a.author)
                .Where(i => i.id == id)
                .FirstOrDefaultAsync();

            if (DBmodel == null)
            {
                throw new KeyNotFoundException("not found");//ex
            }

            var consultation = new ConsultationModel
            {
                id = DBmodel.id,
                createTime = DBmodel.createTime,
                inspectionId = DBmodel.inspectionId,
                speciality = new SpecialtyModel
                {
                    createTime = DBmodel.speciality.createTime,
                    id = DBmodel.speciality.id,
                    name = DBmodel.speciality.name,
                }
            };

            // нахождение комментариев
            var comments = new List<DbInspectionComment>();
            var currentComment = DBmodel.rootComment;

            while (currentComment != null)
            {
                comments.Add(currentComment);
                currentComment = await _context.InspectionComments
                    .Include(d => d.author)
                    .Where(i => i.parentId == currentComment.id)
                    .FirstOrDefaultAsync();
            }

            // трансформация комментариев
            var DTOcomments = new List<CommentModel>();
            foreach (var comment in comments)
            {
                var DTOcomment = new CommentModel
                {
                    id = comment.id,
                    createTime = comment.createTime,
                    modifiedDate = comment.modifyTime,
                    content = comment.content,
                    authorId = comment.author.id,
                    author = comment.author.name,
                    parentId = comment.parentId,
                };
                DTOcomments.Add(DTOcomment);
            }

            consultation.comments = DTOcomments;

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
            var doctor = await _context.Doctors.FindAsync(parsedId);

            // проверка на наличие консультации или родительского комментария
            var consultation = await _context.InspetionConsultations
                .Include(s => s.speciality)
                .Include(r => r.rootComment)
                .Where(i => i.id == id)
                .FirstOrDefaultAsync();

            var parentComment = await _context.InspectionComments.FindAsync(comment.parentId);
            if (consultation == null || parentComment == null)
            {
                throw new KeyNotFoundException("consultation or parent comment not found");// ex
            }

            // проверка доступа
            var inspection = await _context.Inspections
                .Include(d=> d.doctor)
                .Where(i => i.id == consultation.inspectionId)
                .FirstOrDefaultAsync();

            if (comment.content.Length < 1 || comment.content.Length >= 1000)
                throw new ValidationAccessException("comment length must be greater than 1"); // ex

            // проверка на принадлежность комментария консультации
            var currentComment = parentComment;
            while (currentComment.parentId != null)
            {
                currentComment = await _context.InspectionComments.FindAsync(currentComment.parentId);
            }
            if (consultation.rootComment != currentComment)
                throw new ValidationAccessException("this comment doesnt belong to consultation"); //ex

            if (consultation.speciality.id != doctor.specialtyId || inspection.doctor.id != doctor.id)
            {
                throw new ForbiddenAccessException("forbidden");//ex
            }

            var newComment = new DbInspectionComment
            {
                createTime = DateTime.UtcNow,
                parentId = parentComment.id,
                content = comment.content,
                author = doctor,
            };

            consultation.commentsNumber++;

            await _context.InspectionComments.AddAsync(newComment);
            await _context.SaveChangesAsync();
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
            var comment = await _context.InspectionComments
                .Include(d => d.author)
                .Where(i => i.id == id)
                .FirstOrDefaultAsync();

            if (comment == null)
                throw new KeyNotFoundException("comment not found");//ex

            // проверка на право доступа
            if (comment.author.id != parsedId)
                throw new ValidationAccessException("forbidden access"); //ex

            // 
            if (newComment.content.Length < 1 || comment.content.Length >= 1000)
                throw new ValidationAccessException("comment length must be greater than 1"); // ex

            else
            {
                comment.modifyTime = DateTime.UtcNow;
                comment.content = newComment.content;
                await _context.SaveChangesAsync();
                return null;
            }  
        }
    }
}
