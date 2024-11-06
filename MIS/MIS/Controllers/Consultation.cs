using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MIS.Middleware;
using MIS.Models.DTO;
using MIS.Services;
using System.ComponentModel.DataAnnotations;
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

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<InspectionPagedListModel>> getList(
                [FromQuery] bool grouped,
                [FromQuery] List<Guid> icdRoots,
                [FromQuery] int page = 1,
                [FromQuery] int size = 5)
        {
            var inspectionPagedList = await _consultationService.GetList(grouped, icdRoots, page, size, User);
            return Ok(inspectionPagedList);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ConsultationModel>> getConsultationById(Guid id)
        {
            var response = await _consultationService.GetById(id);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("{id}/comment")]
        public async Task<ActionResult<Guid>> postComment(Guid id, CommentCreateModel comment)
        {
            var response = await _consultationService.CreateById(id, comment, User);
            return Ok(response);
        }

        [Authorize]
        [HttpPut("/comment/{id}")]
        public async Task<ActionResult<ResponseModel>> editComment(Guid id, InspectionCommentCreateModel comment)
        {
            var response = await _consultationService.EditById(id, comment, User);
            return Ok(response);
        }
    }
}
