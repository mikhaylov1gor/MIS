namespace MIS.Models.DB
{
    public class DbIcd10
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        public string? code { get; set; }
        public string? name { get; set; }
        public string? recordCode { get; set; }
        public Guid? parentId { get; set; }
    }
}
