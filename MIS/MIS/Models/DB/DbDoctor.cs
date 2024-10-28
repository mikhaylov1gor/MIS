using System.ComponentModel.DataAnnotations;
using MIS.Models.DTO;

namespace MIS.Models.DB
{
    public class DbDoctor
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
        public string passwordHash { get; set; }
        public Guid specialtyId { get; set; }
    }
}
