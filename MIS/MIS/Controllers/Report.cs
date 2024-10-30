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
        [HttpGet("{id}/icdrootsreport")]
        public async Task<IcdRootsReportModel> getReport(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end,
            [FromQuery] List<Guid> icdRoots)
        {
            return await _reportService.getReport(start, end, icdRoots);
        }
    }
}
