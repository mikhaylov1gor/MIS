using System.ComponentModel.DataAnnotations;

namespace MIS.Models.DTO
{
    public class LoginCredentialsModel
    {
        [EmailAddress, MinLength(1)]
        public string email { get; set; }
        [MinLength(1)]
        public string password { get; set; }
    }
}
