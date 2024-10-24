namespace MIS.Models
{
    public class Icd10SearchModel
    {
        public Icd10RecordModel? records { get; set; }
        public PageInfoModel pagination { get; set; }
    }
}
