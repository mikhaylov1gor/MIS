using MIS.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace MIS.Models.DB
{
    public class DbInspectionComment
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        public Guid? parentId { get; set; }
        [MinLength(1)]
        [MaxLength(1000)]
        public string? content { get; set; }
        public DbDoctor author { get; set; }
        public DateTime? modifyTime { get; set; }
    }
}
