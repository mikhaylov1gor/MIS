using Microsoft.AspNetCore.Mvc;
using MIS.Models.DTO;

namespace MIS.Services
{
    public interface IConsultationService
    {
        InspectionPagedListModel GetList(
                        [FromQuery] bool grouped,
                        [FromQuery] List<Guid> icdRoots,
                        [FromQuery] int page,
                        [FromQuery] int size);
        ConsultationModel GetById(Guid id);
        CommentCreateModel CreateById(Guid id, CommentCreateModel comment);
        InspectionCommentCreateModel EditById(Guid id, CommentModel comment);
    }
    public class ConsultationService : IConsultationService
    {
        public InspectionPagedListModel GetList(
                [FromQuery] bool grouped,
                [FromQuery] List<Guid> icdRoots,
                [FromQuery] int page,
                [FromQuery] int size)
        {
            return null;
        }

        public ConsultationModel GetById(Guid id)
        {
            return null;
        }

        public CommentCreateModel CreateById (Guid id, CommentCreateModel comment)
        {
            return null;
        }

        public InspectionCommentCreateModel EditById (Guid id, CommentModel comment)
        {
            return null;
        }

    }
}
