using System.ComponentModel.DataAnnotations;

namespace MIS.Models
{
    public class DiagnosisModel
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        [MinLength(1)]
        public string code { get; set; }
        [MinLength(1)]
        public string name { get; set; }
        public string? description { get; set; }
        public DiagnosisType type { get; set; }
    }
}
