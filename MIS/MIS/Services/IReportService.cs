using Microsoft.AspNetCore.Mvc;
using MIS.Models;

namespace MIS.Services
{
    public interface IReportService
    {
        IcdRootsReportModel getReport(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end,
            [FromQuery] List<Guid> icdRoots);
    }
    public class ReportService : IReportService
    {
        public IcdRootsReportModel getReport(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end,
            [FromQuery] List<Guid> icdRoots)
        {
            return null;
        }
    }
}
