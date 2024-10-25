namespace MIS.Models.DTO
{
    public class PatientPagedListModel
    {
        public List<PatientModel>? patientds { get; set; }
        public PageInfoModel pagination { get; set; }
    }
}
