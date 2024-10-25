using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MIS.Models.DTO;
using MIS.Services;

namespace MIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DictionaryController : ControllerBase
    {
        private DictionaryService _dictionaryservice;

        [HttpGet("/specialty")]
        public ActionResult<SpecialtiesPagedListModel> getList(
                [FromQuery] string name,
                [FromQuery] int page = 1,
                [FromQuery] int size = 5)
        {
            return _dictionaryservice.getList(name, page, size);
        }
        [HttpGet("/icd10")]
        public ActionResult<Icd10SearchModel> getIcd10(
                [FromQuery] string request,
                [FromQuery] int page = 1,
                [FromQuery] int size = 5)
        {
            return _dictionaryservice.getIcd10(request, page, size);  
        }
        [HttpGet("/icd10/roots")]
        public IEnumerable<Icd10RecordModel> getIcdList()
        {
            return _dictionaryservice.getIcdList();
        }
    }
}
