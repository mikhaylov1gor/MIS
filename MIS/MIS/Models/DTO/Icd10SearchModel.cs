namespace MIS.Models.DTO
{
    public class Icd10SearchModel
    {
        public Icd10RecordModel? records { get; set; }
        public PageInfoModel pagination { get; set; }
    }
}
