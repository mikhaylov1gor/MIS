namespace MIS.Models.DTO
{
    public class InspectionConsultationModel
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        public Guid inspectionId { get; set; }
        public SpecialityModel speciality { get; set; }
        public InspectionCommentModel rootComment { get; set; }
        public int commentsNumber { get; set; }
    }
}
