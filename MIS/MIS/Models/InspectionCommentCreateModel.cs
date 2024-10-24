using System.ComponentModel.DataAnnotations;

namespace MIS.Models
{
    public class InspectionCommentCreateModel
    {
        [MinLength(1),MaxLength(1000)]
        public string content { get; set; }
    }
}
