using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace MIS.Models.DTO
{
    public class CommentCreateModel
    {
        [MaxLength(1000)]
        public string? content { get; set; }
        public Guid? parentId { get; set; }
    }
}