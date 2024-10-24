namespace MIS.Models
{
    public class PatientPagedListModel
    {
        public List<PatientModel>? patientds {  get; set; }
        public PageInfoModel pagination { get; set; }
    }
}
