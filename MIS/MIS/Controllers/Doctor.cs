using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace MIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Doctor : ControllerBase
    {
        [HttpPost("/register")]
        public string post()
        {
            return "post";
        }

        [HttpPost("/login")]
        public string post1()
        {
            return "post";
        }

        [HttpPost("/logout")]
        public string post2()
        {
            return "post";
        }

        [HttpGet("/profile")]
        public string get()
        {
            return "get";
        }

        [HttpPut("/profile")]
        public string post3()
        {
            return "post";
        }

    }
}
