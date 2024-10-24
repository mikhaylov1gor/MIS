using System.ComponentModel.DataAnnotations;

namespace MIS.Models
{
    public class PatientCreateModel
    {
        [MinLength(1)]
        [MaxLength(1000)]
        public int name {  get; set; }
        public DateTime? birthday { get; set; }
        public Gender gender { get; set; }
    }
}
