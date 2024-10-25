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
        private ReportService _reportService;

        [HttpGet("{id}/icdrootsreport")]
        public IcdRootsReportModel getReport(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end,
            [FromQuery] List<Guid> icdRoots)
        {
            return _reportService.getReport(start, end, icdRoots);
        }
    }
}
