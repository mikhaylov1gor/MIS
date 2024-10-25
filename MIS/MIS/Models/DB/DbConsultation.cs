using MIS.Models.DTO;

namespace MIS.Models.DB
{
    public class DbConsultation
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        public Guid inspectionId { get; set; }
        public DbSpecialty speciality { get; set; }
        public List<DbComment>? comments { get; set; }
    }
}
