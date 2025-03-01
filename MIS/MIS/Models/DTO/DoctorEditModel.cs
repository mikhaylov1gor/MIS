using System.ComponentModel.DataAnnotations;

namespace MIS.Models.DTO
{
    public class DoctorEditModel
    {
        [EmailAddress, MinLength(1)]
        public string email { get; set; }
        [MinLength(1), MaxLength(1000)]
        public string name { get; set; }
        [Required]
        public DateTime? birthday { get; set; }
        public Gender gender { get; set; }
        [Phone]
        public string? phone { get; set; }
    }
}
