using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MIS.Models.DTO;
using MIS.Services;

namespace MIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultationController : ControllerBase
    {
        private ConsultationService _consultationService;

        [HttpGet]
        public ActionResult<InspectionPagedListModel> getList(
                [FromQuery] bool grouped,
                [FromQuery] List<Guid> icdRoots,
                [FromQuery] int page = 1,
                [FromQuery] int size = 5
            )
        {
            return _consultationService.GetList(grouped, icdRoots, page, size);
        }

        [HttpGet("{id}")]
        public ActionResult<ConsultationModel> getConsultationById(Guid id)
        {
            return _consultationService.GetById(id);
        }

        [HttpPost("{id}/comment")]
        public ActionResult<CommentCreateModel> postComment(Guid id, CommentCreateModel comment)
        {
            return _consultationService.CreateById(id, comment);   
        }

        [HttpPut("/comment/{id}")]
        public ActionResult<InspectionCommentCreateModel> editComment(Guid id, CommentModel comment)
        {
            return _consultationService.EditById(id, comment); 
        }

    }
}
