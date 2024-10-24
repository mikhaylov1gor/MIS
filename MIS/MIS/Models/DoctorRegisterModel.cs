using System.ComponentModel.DataAnnotations;

namespace MIS.Models
{
    public class DoctorRegisterModel
    {
        [MinLength(1), MaxLength(1000)]
        public string name { get; set; }
        [MinLength(6)]
        public string password { get; set; }
        [EmailAddress, MinLength(1)]
        public string email { get; set; }
        public DateTime? birthday { get; set; }
        public Gender gender { get; set; }
        [Phone]
        public string? phone { get; set; }
        public Guid speciality { get; set; }
    }
}
