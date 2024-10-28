using System.ComponentModel.DataAnnotations;
using MIS.Models.DTO;

namespace MIS.Models.DB
{
    public class DbInspection
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
        public DbPatient patient { get; set; }
        public DbDoctor doctor { get; set; }
        [MinLength(1)]
        public List<DbDiagnosis>? diagnoses { get; set; }
        public List<DbInspectionConsultation>? consultations { get; set; }
    }
}
