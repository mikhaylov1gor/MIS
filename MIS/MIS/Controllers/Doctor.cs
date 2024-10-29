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
        [HttpPost("/register")]
        public async Task<IActionResult> register(DoctorRegisterModel doctor)
        {
            var result = await _doctorService.register(doctor);

            if (result is ResponseModel responseModel)
            {
                return StatusCode(int.Parse(responseModel.status), responseModel);
            }

            return Ok(result);
        }

        // логин
        [HttpPost("/login")]
        public async Task<ActionResult<TokenResponseModel>> login(LoginCredentialsModel loginCredentials)
        {
            var token = await _doctorService.login(loginCredentials);

            if (token == null) 
            {
                return StatusCode(401, new ResponseModel { status = "401", message = "Password or email is incorrect" });
            }

            return Ok(token);
        }

        [Authorize]
        [HttpPost("/logout")]
        public async Task<ResponseModel> logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            return await _doctorService.logout(token);
        }

        [Authorize]
        [HttpGet("/profile")]
        public async Task<ActionResult<DoctorModel>> getProfile()
        {
            var profile = await _doctorService.getProfile(User);

            if (profile == null)
            {
                return NotFound(new ResponseModel { status = "404", message = "Doctor profile not found" });
            }
            else
            {
                return Ok(profile);
            }
        }

        [Authorize]
        [HttpPut("/profile")]
        public async Task<ResponseModel> editProfile(DoctorEditModel doctorEdit)
        {
            return await _doctorService.editProfile(doctorEdit, User);
        }

    }
}
