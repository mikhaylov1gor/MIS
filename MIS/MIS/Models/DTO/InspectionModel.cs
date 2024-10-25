using System.ComponentModel.DataAnnotations;

namespace MIS.Models.DTO
{
    public class InspectionModel
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        public DateTime date { get; set; }
        public string? anamnesis { get; set; }
        public string? complaints { get; set; }
        public string? treatment { get; set; }
        public Conclusion conclusion { get; set; }
        public DateTime? nextVisitDate { get; set; }
        public DateTime? deathDate { get; set; }
        public Guid? baseInspectionId { get; set; }
        public Guid? previousInspectionId { get; set; }
        public PatientModel patient { get; set; }
        public DoctorModel doctor { get; set; }
        [MinLength(1)]
        public List<DiagnosisModel> diagnoses { get; set; }
        public List<InspectionConsultationModel>? consultations { get; set; }
    }
}
