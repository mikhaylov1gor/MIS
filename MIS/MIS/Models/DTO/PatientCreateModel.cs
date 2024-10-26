using System.ComponentModel.DataAnnotations;

namespace MIS.Models.DTO
{
    public class PatientCreateModel
    {
        [MinLength(1)]
        [MaxLength(1000)]
        public string name { get; set; }
        public DateTime? birthday { get; set; }
        public Gender gender { get; set; }
    }
}
