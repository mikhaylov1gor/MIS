using System.ComponentModel.DataAnnotations;
using MIS.Models.DTO;

namespace MIS.Models.DB
{
    public class DbDiagnosis
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
