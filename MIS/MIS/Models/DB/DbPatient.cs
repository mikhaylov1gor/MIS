using System.ComponentModel.DataAnnotations;
using MIS.Models.DTO;

namespace MIS.Models.DB
{
    public class DbPatient
    {
        public Guid id { get; private set; }
        public DateTime createTime { get; set; }
        [MinLength(1)]
        public string name { get; set; }
        public DateTime? birthday { get; set; }
        public Gender gender { get; set; }
    }
}
