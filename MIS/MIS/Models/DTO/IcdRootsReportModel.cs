namespace MIS.Models.DTO
{
    public class IcdRootsReportModel
    {
        public IcdRootsReportFiltersModel filters { get; set; }
        public List<IcdRootsReportRecordModel>? records { get; set; }
        public Dictionary<string, int> summaryByRoot { get; set; }
    }
}
