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
            var inspection = await _inspectionService.getInspection(id);

            if (inspection == null)
            {
                return StatusCode(404, new ResponseModel { status = "404", message = "Not Found" });
            }
            else
            {
                return Ok(inspection);
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel>>  editInspection(Guid id, InspectionEditModel inspectionEdit)
        {
            var response = await _inspectionService.editInspection(id, inspectionEdit);

            return response;
        }

        [Authorize]
        [HttpGet("{id}/chain")]
        public async Task<ActionResult<InspectionPreviewModel>> getChainInspections(Guid id)
        {
            var inspections = await _inspectionService.getChain(id);

            if (inspections == null)
            {
                return StatusCode(404, new ResponseModel { status = "404", message = "Not Found" });
            }
            else
            {
                return Ok(inspections);
            }
        }
    }
}
