namespace MIS.Models.DTO
{
    public class IcdRootsReportRecordModel
    {
        public string? patientName { get; set; }
        public DateTime? patientBirthday { get; set; }
        public Gender gender { get; set; }
        public Dictionary<string, int>? visitsByRoot { get; set; }
    }
}
