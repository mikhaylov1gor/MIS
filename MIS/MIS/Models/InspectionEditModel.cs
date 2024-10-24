using System.ComponentModel.DataAnnotations;

namespace MIS.Models
{
    public class InspectionEditModel
    {
        [MaxLength(5000)]
        public string? anamnesis {  get; set; }
        [MinLength(1), MaxLength(5000)]
        public string complaints { get; set; }
        [MinLength(1), MaxLength(5000)]
        public string treatment { get; set; }
        public Conclusion conclusion { get; set; }
        public DateTime? nextVisitDate { get; set; }
        public DateTime? deathDate { get; set; }
        [MinLength(1)]
        public List<DiagnosisCreateModel> diagnosis {  get; set; }

    }
}
