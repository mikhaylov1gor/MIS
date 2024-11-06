using Azure;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
        [HttpGet("/specialty")]
        public async Task<ActionResult<SpecialtiesPagedListModel>> getList(
                [FromQuery] string name = "",
                [FromQuery] int page = 1,
                [FromQuery] int size = 5)
        {
            var response = await _dictionaryservice.getList(name, page, size);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("/icd10")]
        public async Task<ActionResult<Icd10SearchModel>> getIcd10(
                [FromQuery] string request = "",
                [FromQuery] int page = 1,
                [FromQuery] int size = 5)
        {
            var response = await _dictionaryservice.getIcd10(request, page, size);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("/icd10/roots")]
        public async Task<ActionResult<List<Icd10RecordModel>>> getIcdRoot()
        {
            var response = await _dictionaryservice.getIcdRoots();
            return Ok(response);
        }
    }
}
