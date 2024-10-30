using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS.Models.DB;
using MIS.Models.DTO;
using System.Security.Claims;

namespace MIS.Services
{
    public interface IConsultationService
    {
        Task<InspectionPagedListModel> GetList(
                [FromQuery] bool grouped,
                [FromQuery] List<Guid> icdRoots,
                [FromQuery] int page,
                [FromQuery] int size);
        Task<ConsultationModel> GetById(Guid id);
        Task<Guid> CreateById(Guid id, CommentCreateModel comment, ClaimsPrincipal user);
        Task<ActionResult> EditById(Guid id, InspectionCommentCreateModel newComment);
    }
    public class ConsultationService : IConsultationService
    {
        private readonly MisDbContext _context;

        public ConsultationService(MisDbContext context)
        {
            _context = context;
        }

        // 
        public async Task<InspectionPagedListModel> GetList(
                [FromQuery] bool grouped,
                [FromQuery] List<Guid> icdRoots,
                [FromQuery] int page,
                [FromQuery] int size)
        {
            var query = _context.Inspections
                .Include(i => i.doctor)
                .Include(i => i.patient)
                .Include(i => i.diagnoses)
                .Where(i => icdRoots == null || i.diagnoses.Any(d => icdRoots.Contains(d.id)));

            if (grouped)
            {
                /*query = query.GroupBy(i => i.previousInspectionId).Select(g => g.FirstOrDefault());*/
            }

            var totalItems = await query.CountAsync();

            var inspections = await query
                .Skip((page - 1) * size)
                .Take(size)
                .Select(i => new InspectionPreviewModel
                {
                    createTime = i.createTime,
                    previousId = i.previousInspectionId,
                    date = i.date,
                    conclusion = i.conclusion,
                    doctorId = i.doctor.id,
                    doctor = i.doctor.name,
                    patientId = i.patient.id,
                    patient = i.patient.name,
/*                    diagnosis = i.diagnoses
                                .Select(b => new DiagnosisModel
                                {
                                    createTime = b.createTime,
                                    code = b.code,
                                    name = b.name,
                                    description = b.description,
                                    type = b.type
                                })
                                .FirstOrDefault(),*/
                    hasChain = (i.previousInspectionId  != null),
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

        public async Task<ConsultationModel> GetById(Guid id)
        {
            var DBmodel = await _context.Consultations.FindAsync(id);

            if (DBmodel == null)
            {
                return null;
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
                return Guid.Empty;
            }
            var doctor = await _context.Doctors.FindAsync(parsedId);

            var consultation = await _context.Consultations.FindAsync(id);
            if (consultation == null)
            {
                return Guid.Empty;
            }

            var newComment = new DbComment
            {
                createTime = DateTime.Now,
                authorId = parsedId,
                author = doctor.name,
                content = comment.content,
                // parentId ?
            };

            consultation.comments.Add(newComment);

            return newComment.id;
        }

        public async Task<ActionResult> EditById (Guid id, InspectionCommentCreateModel newComment)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return null;
            }
            else
            {
                comment.modifiedDate = DateTime.Now;
                comment.content = newComment.content;
                await _context.SaveChangesAsync();
                return null;
            }  
        }
    }
}
