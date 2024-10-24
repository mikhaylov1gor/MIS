namespace MIS.Models
{
    public class IcdRootsReportRecordModel
    {
        public string? patientName { get; set; }
        public DateTime patientBirthday { get; set; }
        public Gender gende { get; set; }
        public Dictionary<string, int>? visitsByRoot { get; set; }
    }
}
