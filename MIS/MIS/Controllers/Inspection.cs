using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MIS.Models;
using MIS.Services;

namespace MIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InspectionController : ControllerBase
    {
        private InspectionService _inspectionService;

        [HttpGet("{id}")]
        public ActionResult<InspectionModel> getInspection(Guid id)
        {
            return _inspectionService.getInspection(id);
        }

        [HttpPut("{id}")]
        public bool editInspection(Guid id, InspectionEditModel inspectionEdit)
        {
            return _inspectionService.editInspection(id, inspectionEdit);
        }

        [HttpGet("{id}/chain")]
        public IEnumerable<InspectionPreviewModel> getChainInspections(Guid id)
        {
            return _inspectionService.getChainInspections(id);
        }
    }
}
