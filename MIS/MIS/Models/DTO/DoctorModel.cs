using System.ComponentModel.DataAnnotations;

namespace MIS.Models.DTO
{
    public class DoctorModel
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        [MinLength(1)]
        public string name { get; set; }
        public DateTime? birthday { get; set; }
        public Gender gender { get; set; }
        [EmailAddress, MinLength(1)]
        public string email { get; set; }
        [Phone]
        public string? phone { get; set; }
    }
}
