using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DictionaryController : ControllerBase
    {
        [HttpGet("/specialty")]
        public string get()
        {
            return "get";
        }
        [HttpGet("/icd10")]
        public string get1()
        {
            return "get";
        }
        [HttpGet("/icd10/roots")]
        public string get2()
        {
            return "get";
        }
    }
}
