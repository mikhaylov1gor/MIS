using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InspectionController : ControllerBase
    {
        [HttpGet("{id}")]
        public string get()
        {
            return "get";
        }

        [HttpPut("{id}")]
        public string put()
        {
            return "put";
        }

        [HttpGet("{id}/chain")]
        public string get1()
        {
            return "get";
        }
    }
}
