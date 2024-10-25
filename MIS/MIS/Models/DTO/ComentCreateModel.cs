using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace MIS.Models.DTO
{
    public class CommentCreateModel
    {
        [MaxLength(1000)]
        private string? content { get; set; }
        private Guid? parentId { get; set; }
    }
}