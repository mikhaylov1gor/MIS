using Microsoft.Identity.Client;

namespace MIS.Models.DTO
{
    public class ConsultationModel
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        public Guid inspectionId { get; set; }
        public SpecialtyModel speciality { get; set; }
        public List<CommentModel>? comments { get; set; }

    }
}
