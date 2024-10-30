using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var summaryByRoot = new Dictionary<string, int>();
            if (icdRoots == null)
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
            var report = new IcdRootsReportModel();

            var filters = new IcdRootsReportFiltersModel { start = start, end = end , icdRoots = icdRoots};

            var records = new List<IcdRootsReportRecordModel>();

            var patients = await _context.Patients.ToListAsync();

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
                            var code = diagnosis.code;
                            var matchingIcd10s = await _context.Icd10
                                .Where(c => c.code == code)
                                .ToListAsync();
                            foreach (var icd10 in matchingIcd10s)
                            {
                                if (icdRoots.Contains(icd10.id) || icdRoots.Contains(icd10.parentId.Value))
                                {
                                    isSatisfy = true;

                                    if (summaryByRoot.ContainsKey(icd10.code))
                                    {
                                        summaryByRoot[icd10.code]++;
                                        visitsByRoot[icd10.code]++;
                                    }
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
                    report.records.Add(record);
                }
            }

            return report;
        }
    }
}
