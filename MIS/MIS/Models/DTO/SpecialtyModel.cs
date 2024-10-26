using System.ComponentModel.DataAnnotations;

namespace MIS.Models.DTO
{
    public class SpecialtyModel
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        [MinLength(1)]
        public string name { get; set; }
    }
}
