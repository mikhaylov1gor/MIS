using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        [HttpGet("{id}/icdrootsreport")]
        public string get()
        {
            return "get";
        }
    }
}
