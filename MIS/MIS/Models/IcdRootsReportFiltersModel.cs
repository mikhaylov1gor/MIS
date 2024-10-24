namespace MIS.Models
{
    public class IcdRootsReportFiltersModel
    {
        public DateTime start {  get; set; }
        public DateTime end { get; set; }
        public List<string>? icdRoots { get; set; }
    }
}
