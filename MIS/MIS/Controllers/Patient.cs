using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MIS.Models.DB;
using MIS.Models.DTO;
using MIS.Services;

namespace MIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        public IPatientService _patientService;

        public PatientController(IPatientService service)
        {
            _patientService = service;
        }

        // создать пациента
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Guid>> createPatient(PatientCreateModel patient)
        {
            var response = await _patientService.createPatient(patient);
            return Ok(response);
            
        }

        // получить лист пациентов
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<PatientPagedListModel>> getPatients(
            [FromQuery] Conclusion[] conclusions,
            [FromQuery] PatientSorting sorting,
            [FromQuery] bool scheduledVisits,
            [FromQuery] bool onlyMine,
            [FromQuery] string name = "",
            [FromQuery] int page = 1,
            [FromQuery] int size = 5)
        {
            var response = await _patientService.getPatients(name, conclusions, sorting, scheduledVisits, onlyMine, page, size, User);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("{id}/inspections")]
        public async Task<ActionResult<Guid>> createInspection(Guid id, InspectionCreateModel model)
        {
            var response = await _patientService.createInspection(id, model, User);
            return Ok(response);
        }
        // !!!!!!!!!!!!
        [Authorize]
        [HttpGet("{id}/inspections")]
        public async Task<ActionResult<InspectionPagedListModel>> getInspections(
            Guid id,
            [FromQuery] bool grouped,
            [FromQuery] List<Guid> icdRoots,
            [FromQuery] int page = 1,
            [FromQuery] int size = 5)
        {
            var response = await _patientService.getInspections(id, grouped, icdRoots, page, size);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientModel>> getPatient(Guid id)
        {
            var response = await _patientService.getPatient(id);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("{id}/inspections/search")]
        public async Task<ActionResult<List<InspectionShortModel>>> getShortInspection(Guid id, [FromQuery] string request = "")
        {
            var response = await _patientService.getShortInspection(id, request);
            return Ok(response);
        }
    }
}
