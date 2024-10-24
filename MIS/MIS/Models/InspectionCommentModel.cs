namespace MIS.Models
{
    public class InspectionCommentModel
    {
        public Guid id { get; set; }
        public DateTime createTime { get; set; }
        public Guid? parentId { get; set; }
        public string? content { get; set; }
        public DoctorModel author { get; set; }
        public DateTime modifyTime { get; set; }
    }
}
