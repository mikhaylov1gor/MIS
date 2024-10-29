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
            return await _patientService.createPatient(patient);
            
        }

        // получить лист пациентов
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<PatientPagedListModel>> getPatients(
            [FromQuery] string name,
            [FromQuery] Conclusion[] conclusions,
            [FromQuery] PatientSorting sorting,
            [FromQuery] bool scheduledVisits,
            [FromQuery] bool onlyMine,
            [FromQuery] int page = 1,
            [FromQuery] int size = 5)
        {
            var patients = await _patientService.getPatients(name, conclusions, sorting, scheduledVisits, onlyMine, page, size, User);

            if (patients == null)
            {
                return StatusCode(404, new ResponseModel { status = "404", message = "Not Found" });
            }
            else
            {
                return StatusCode(200, patients);
            }
        }

        [Authorize]
        [HttpPost("{id}/inspections")]
        public async Task<Guid> createInspection(Guid id, InspectionCreateModel model)
        {
            return await _patientService.createInspection(id, model, User);
        }
        // !!!!!!!!!!!!
        [Authorize]
        [HttpGet("{id}/inspections")]
        public InspectionPagedListModel getInspections(
            Guid id,
            [FromQuery] bool grouped,
            [FromQuery] List<Guid> icdRoots,
            [FromQuery] int page,
            [FromQuery] int size)
        {
            return _patientService.getInspections(id, grouped, icdRoots, page, size);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientModel>> getPatient(Guid id)
        {
            var patient = await _patientService.getPatient(id);

            if (patient == null)
            {
                return StatusCode(404, new ResponseModel { status = "404", message = "Not Found" });
            }
            else
            {
                return patient;
            }
        }

        [Authorize]
        [HttpGet("{id}/inspections/search")]
        public async Task<List<InspectionShortModel>> getShortInspection(Guid id, [FromQuery] string request)
        {
            return await _patientService.getShortInspection(id, request);
        }
    }
}
