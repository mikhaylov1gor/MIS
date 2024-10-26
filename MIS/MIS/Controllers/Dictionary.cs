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
        private IDictionaryService _dictionaryservice;

        public DictionaryController(IDictionaryService _service)
        {
            _dictionaryservice = _service;
        }

        // получить лист специализаций
        [HttpGet("/specialty")]
        public async Task<ActionResult<SpecialtiesPagedListModel>> getList(
                [FromQuery] string name,
                [FromQuery] int page = 1,
                [FromQuery] int size = 5)
        {
            if (page < 1 || size < 1)
            {
                return BadRequest("Page and Size parameters must be greater than 0");
            }

            var specialtiesPagedList = await _dictionaryservice.getList(name, page, size);

            return Ok(specialtiesPagedList);
        }

        [HttpGet("/icd10")]
        public async Task<ActionResult<Icd10SearchModel>> getIcd10(
                [FromQuery] string request,
                [FromQuery] int page = 1,
                [FromQuery] int size = 5)
        {
            if (page < 1 || size < 1)
            {
                return BadRequest("Page and Size parameters must be greater than 0");
            }

            var icd10SearchModel = await _dictionaryservice.getIcd10(request, page, size);

            return Ok(icd10SearchModel);
        }

        [HttpGet("/icd10/roots")]
        public IEnumerable<Icd10RecordModel> getIcdRoot()
        {
            return _dictionaryservice.getIcdList();
        }
    }
}
