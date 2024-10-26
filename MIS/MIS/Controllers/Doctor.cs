using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using MIS.Models.DB;
using MIS.Models.DTO;
using MIS.Services;
using System.Numerics;

namespace MIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Doctor : ControllerBase
    {
        private IDoctorService _doctorService;

        public Doctor(IDoctorService _service)
        {
            _doctorService = _service;
        }
        
        // регистрация доктора
        [HttpPost("/register")]
        public async Task<IActionResult> register(DoctorRegisterModel doctor)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(401, new ResponseModel { status = "401", message = "model is incorrect" });
            }

            try
            {
                await _doctorService.register(doctor);
                return Ok(new ResponseModel { status = "200", message = "Doctor was registered" });
            }

            catch (Exception ex)
            {
                return StatusCode(400, new ResponseModel { status = "400", message = "Invalid arguments"});
            }
        }

        // логин
        [HttpPost("/login")]
        public async Task<IActionResult> login(LoginCredentialsModel loginCredentials)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(401, new ResponseModel { status = "401", message = "model is incorrect" });
            }

            try
            {
                await _doctorService.login(loginCredentials);
                return Ok(new TokenResponseModel ());
            }

            catch (Exception ex)
            {
                return StatusCode(400, new ResponseModel { status = "400", message = "Invalid arguments" });
            }
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
