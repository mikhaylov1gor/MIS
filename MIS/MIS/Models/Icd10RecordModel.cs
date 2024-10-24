namespace MIS.Models
{
    public class Icd10RecordModel
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        public string? code {  get; set; }
        public string? name { get; set; }
    }
}
