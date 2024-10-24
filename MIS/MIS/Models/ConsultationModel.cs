using Microsoft.Identity.Client;

namespace MIS.Models
{
    public class ConsultationModel
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        public Guid inspectionId { get; set; }
        public SpecialityModel speciality { get; set; }
        public CommentModel? comments { get; set; }

    }
}
