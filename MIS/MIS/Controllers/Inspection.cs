using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MIS.Models.DB;
using MIS.Models.DTO;
using MIS.Services;

namespace MIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InspectionController : ControllerBase
    {
        private IInspectionService _inspectionService;

        public InspectionController(IInspectionService _service)
        {
            _inspectionService = _service;
        }


        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<InspectionModel>> getInspection(Guid id)
        {
            var response = await _inspectionService.getInspection(id);
            return Ok(response);

        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult>  editInspection(Guid id, InspectionEditModel inspectionEdit)
        {
            var response = await _inspectionService.editInspection(id, inspectionEdit, User);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("{id}/chain")]
        public async Task<ActionResult<InspectionPreviewModel>> getChainInspections(Guid id)
        {
            var response = await _inspectionService.getChain(id);
            return Ok(response);
        }
    }
}
