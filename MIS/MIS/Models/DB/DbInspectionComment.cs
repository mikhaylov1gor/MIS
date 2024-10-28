using MIS.Models.DTO;

namespace MIS.Models.DB
{
    public class DbInspectionComment
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        public Guid? parentId { get; set; }
        public string? content { get; set; }
        public DbDoctor author { get; set; }
        public DateTime modifyTime { get; set; }
    }
}
