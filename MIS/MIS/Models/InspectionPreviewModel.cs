using System.ComponentModel.DataAnnotations;

namespace MIS.Models
{
    public class InspectionPreviewModel
    {
        public Guid id {  get; set; }
        public DateTime createTime { get; set; }
        public Guid? previousId { get; set; }
        public DateTime date {  get; set; }
        public Conclusion conclusion { get; set; }
        public Guid doctorId { get; set; }
        [MinLength(1)]
        public string doctor {  get; set; }
        public Guid patientId {  get; set; }
        [MinLength(1)]
        public string patient { get; set; }
        public DiagnosisModel diagnosis { get; set; }
        public bool hasChain { get; set; }
        public bool hasNested {  get; set; }
    }
}
