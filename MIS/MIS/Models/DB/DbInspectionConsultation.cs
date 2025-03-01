using MIS.Models.DTO;

namespace MIS.Models.DB
{
    public class DbInspectionConsultation
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        public Guid inspectionId { get; set; }
        public DbSpecialty speciality { get; set; }
        public DbInspectionComment rootComment { get; set; }
        public int commentsNumber { get; set; }
    }
}
