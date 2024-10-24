using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        [HttpPost]
        public string post()
        {
            return "post";
        }

        [HttpGet]
        public string get()
        {
            return "get";
        }

        [HttpPost("{id}/inspections")]
        public string post1()
        {
            return "post";
        }

        [HttpGet("{id}/inspections")]
        public string get1()
        {
            return "get";
        }

        [HttpGet("{id}")]
        public string get2()
        {
            return "get";
        }

        [HttpGet("{id}/inspections/search")]
        public string get3()
        {
            return "get";
        }
    }
}
