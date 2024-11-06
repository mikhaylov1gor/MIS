using Microsoft.AspNetCore.Authorization;
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

        // регистрация доктора\
        [AllowAnonymous]
        [HttpPost("/register")]
        public async Task<ActionResult> register(DoctorRegisterModel doctor)
        {
            var response = await _doctorService.register(doctor);
            return Ok(response);
        }

        // логин
        [AllowAnonymous]
        [HttpPost("/login")]
        public async Task<ActionResult<TokenResponseModel>> login(LoginCredentialsModel loginCredentials)
        {
            var response = await _doctorService.login(loginCredentials);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("/logout")]
        public async Task<ActionResult> logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var response = await _doctorService.logout(token,User);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("/profile")]
        public async Task<ActionResult<DoctorModel>> getProfile()
        {
            var response = await _doctorService.getProfile(User);
            return Ok(response);
        }

        [Authorize]
        [HttpPut("/profile")]
        public async Task<ActionResult> editProfile(DoctorEditModel doctorEdit)
        {
            var response = await _doctorService.editProfile(doctorEdit, User);
            return Ok(response);
        }

    }
}
