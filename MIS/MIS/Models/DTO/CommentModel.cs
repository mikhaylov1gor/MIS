using System.ComponentModel.DataAnnotations;

namespace MIS.Models.DTO
{
    public class CommentModel
    {
        public Guid id { get; private set; }
        public DateTime createTime { get; set; }
        public DateTime? modifiedDate { get; set; }

        [Required, MinLength(1)]
        public string content { get; set; }

        [Required]
        public Guid authorId { get; set; }
        public string author { get; set; }
        public Guid? parentId { get; set; }
    }
}
