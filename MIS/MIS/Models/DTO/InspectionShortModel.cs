namespace MIS.Models.DTO
{
    public class InspectionShortModel
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        public DateTime date { get; set; }
        public DiagnosisModel diagnosis { get; set; }
    }
}
