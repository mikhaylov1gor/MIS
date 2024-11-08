using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS.Middleware;
using MIS.Models.DB;
using MIS.Models.DTO;
using System.Linq;

namespace MIS.Services
{
    public interface IReportService
    {
        Task<IcdRootsReportModel> getReport(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end,
            [FromQuery] List<Guid> icdRoots);
    }
    public class ReportService : IReportService
    {
        private readonly MisDbContext _context;

        public ReportService(MisDbContext context)
        {
            _context = context;
        }

        // getReport
        public async Task<IcdRootsReportModel> getReport(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end,
            [FromQuery] List<Guid> icdRoots)
        {
            if (start > end)
            {
                throw new ValidationAccessException("start must be less than end");
            }
            var summaryByRoot = new Dictionary<string, int>();

            // проверка рутовых диагнозов
            if (icdRoots.Count() == 0)
            {
                icdRoots = await _context.Icd10
                    .Where(p => p.parentId == null)
                    .Select(g => g.id)
                    .ToListAsync();

                foreach (var root in icdRoots)
                {
                    var icdRoot = await _context.Icd10
                        .FirstOrDefaultAsync(i => i.id == root);
                    summaryByRoot.Add(icdRoot.code, 0);
                }
            }
            else
            {
                var roots = await _context.Icd10
                    .Where(p => p.parentId == null)
                    .Select(p => p.id)
                    .ToListAsync();

                foreach (var root in icdRoots)
                {
                    if (!roots.Contains(root))
                    {
                        throw new ValidationAccessException("icdRoot not found");//ex
                    }

                    var icdRoot = await _context.Icd10
                        .FirstOrDefaultAsync(i => i.id == root);
                    summaryByRoot.Add(icdRoot.code, 0);
                }
            }
            var report = new IcdRootsReportModel();

            var filters = new IcdRootsReportFiltersModel { start = start, end = end , icdRoots = icdRoots};

            var records = new List<IcdRootsReportRecordModel>();

            var patients = await _context.Patients
                .Include(p=>p.inspections)
                    .ThenInclude(i=>i.diagnoses)
                .ToListAsync();

            foreach (var patient in patients)
            {
                bool isSatisfy = false;
                var visitsByRoot = new Dictionary<string, int>();
                
                foreach (var root in summaryByRoot.Keys)
                {
                    visitsByRoot[root] = 0;
                }
                var inspections = patient.inspections;
                foreach (var inspection in inspections)
                {
                    if (inspection.date >= start && inspection.date <= end)
                    {
                        var diagnoses = inspection.diagnoses;

                        foreach (var diagnosis in diagnoses)
                        {
                            if (diagnosis.type != DiagnosisType.Main) // подсчет только по главным диагнозам
                                continue;

                            var code = diagnosis.code;
                            var matchingIcd10 = await _context.Icd10
                                .Where(c => c.code == code)
                                .FirstOrDefaultAsync();


                            if (icdRoots.Contains(matchingIcd10.id) ||
                               (matchingIcd10.parentId.HasValue && icdRoots.Contains(matchingIcd10.parentId.Value)))
                            {
                                isSatisfy = true;

                                if (summaryByRoot.ContainsKey(matchingIcd10.code))
                                {
                                    summaryByRoot[matchingIcd10.code]++;
                                    visitsByRoot[matchingIcd10.code]++;
                                }
                            }

                        }
                    }
                }

                if (isSatisfy)
                {
                    var record = new IcdRootsReportRecordModel
                    {
                        patientName = patient.name,
                        patientBirthday = patient.birthday,
                        gender = patient.gender,
                        visitsByRoot = visitsByRoot
                    };
                    records.Add(record);
                }
            }
            records.OrderBy(p => p.patientName);

            report.filters = filters;
            report.records = records;
            report.summaryByRoot = summaryByRoot;
            return report;
        }
    }
}
