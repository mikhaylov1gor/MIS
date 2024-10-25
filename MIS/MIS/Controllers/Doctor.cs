using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using MIS.Models.DTO;
using MIS.Services;

namespace MIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Doctor : ControllerBase
    {
        private DoctorService _doctorService;

        [HttpPost("/register")]
        public bool register(DoctorRegisterModel doctor)
        {
            return _doctorService.register(doctor);
        }

        [HttpPost("/login")]
        public bool login(LoginCredentialsModel loginCredentials)
        {
            return _doctorService.login(loginCredentials);
        }

        [HttpPost("/logout")]
        public bool logout()
        {
            return _doctorService.logout();
        }

        [HttpGet("/profile")]
        public DoctorModel getProfile()
        {
            return _doctorService.getProfile();
        }

        [HttpPut("/profile")]
        public bool editProfile(DoctorEditModel doctorEdit)
        {
            return _doctorService.editProfile(doctorEdit);
        }

    }
}
