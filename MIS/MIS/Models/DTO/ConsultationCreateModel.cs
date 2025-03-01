namespace MIS.Models.DTO
{
    public class ConsultationCreateModel
    {
        public Guid specialityId { get; set; }
        public InspectionCommentCreateModel comment { get; set; }
    }
}
