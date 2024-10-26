using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MIS.Models.DTO;
using MIS.Services;
using System.ComponentModel.Design;

namespace MIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultationController : ControllerBase
    {
        private IConsultationService _consultationService;

        public ConsultationController(IConsultationService _service)
        {
            _consultationService = _service;
        }

        [HttpGet]
        public async Task<ActionResult<InspectionPagedListModel>> getList(
                [FromQuery] bool grouped,
                [FromQuery] List<Guid> icdRoots,
                [FromQuery] int page = 1,
                [FromQuery] int size = 5)
        {

            if (page < 1 || size < 1)
            {
                return BadRequest("Page and Size parameters must be greater than 0");
            }

            var inspectionPagedList = await _consultationService.GetList(grouped, icdRoots, page, size);

            return Ok(inspectionPagedList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ConsultationModel>> getConsultationById(Guid id)
        {
            var inspection = await _consultationService.GetById(id);
            
            if (inspection == null)
            {
                return StatusCode(404, new ResponseModel { status = "404", message = "not found" });
            }

            return Ok(inspection);
        }

        [HttpPost("{id}/comment")]
        public async Task<ActionResult<ResponseModel>> postComment(Guid id, CommentCreateModel comment)
        {
            var response = await _consultationService.CreateById(id, comment);

            return Ok(response);
        }

        [HttpPut("/comment/{id}")]
        public async Task<ActionResult<ResponseModel>> editComment(Guid id, InspectionCommentCreateModel comment)
        {
            var response = await _consultationService.EditById(id, comment);

            return Ok(response);
        }

    }
}
