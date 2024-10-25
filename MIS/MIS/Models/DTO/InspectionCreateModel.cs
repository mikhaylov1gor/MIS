using System.ComponentModel.DataAnnotations;

namespace MIS.Models.DTO
{
    public class InspectionCreateModel
    {
        public DateTime date { get; set; }
        [MinLength(1), MaxLength(5000)]
        public string anamnesis { get; set; }
        [MinLength(1), MaxLength(5000)]
        public string complaints { get; set; }
        [MinLength(1), MaxLength(5000)]
        public string treatment { get; set; }
        public Conclusion conclusion { get; set; }
        public DateTime? nextVisitDate { get; set; }
        public DateTime? deathDate { get; set; }
        public Guid? previousInspectionId { get; set; }
        [MinLength(1)]
        public List<DiagnosisCreateModel> diagnosis { get; set; }
        public List<ConsultationCreateModel>? consultation { get; set; }

    }
}
