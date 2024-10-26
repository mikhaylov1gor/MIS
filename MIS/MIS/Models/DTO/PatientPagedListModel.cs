namespace MIS.Models.DTO
{
    public class PatientPagedListModel
    {
        public List<PatientModel>? patients { get; set; }
        public PageInfoModel pagination { get; set; }
    }
}
