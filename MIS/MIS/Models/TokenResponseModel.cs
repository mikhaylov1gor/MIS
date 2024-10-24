using System.ComponentModel.DataAnnotations;

namespace MIS.Models
{
    public class TokenResponseModel
    {
        [MinLength(1)]
        public string token { get; set; }
    }
}
