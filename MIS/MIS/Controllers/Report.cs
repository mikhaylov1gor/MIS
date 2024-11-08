using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MIS.Models.DTO;
using MIS.Services;

namespace MIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private IReportService _reportService;

        public ReportController(IReportService service)
        {
            _reportService = service;
        }

        [Authorize]
        [HttpGet("icdrootsreport")]
        public async Task<ActionResult<IcdRootsReportModel>> getReport(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end,
            [FromQuery] List<Guid> icdRoots)
        {
            var response = await _reportService.getReport(start, end, icdRoots);
            return Ok(response);
        }
    }
}
