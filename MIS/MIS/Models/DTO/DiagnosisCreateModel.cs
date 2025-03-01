using System.ComponentModel.DataAnnotations;

namespace MIS.Models.DTO
{
    public class DiagnosisCreateModel
    {
        public Guid icdDiagnosisId { get; set; }
        [MaxLength(5000)]
        public string? description { get; set; }
        public DiagnosisType type { get; set; }
    }
}
