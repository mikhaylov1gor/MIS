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
        [HttpPost]
        public async Task<IActionResult> createPatient(PatientCreateModel patient)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(401, new ResponseModel { status = "401" , message = "model is incorrect"});
            }

            try
            {
                await _patientService.createPatient(patient);
                return Ok(new ResponseModel { status = "200", message = "Success"});
            }

            catch (Exception ex)
            {
                ResponseModel model = new ResponseModel();
                return StatusCode(500, model);
            }
        }

        // получить лист пациентов
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
            var patients = await _patientService.getPatients(name, conclusions, sorting, scheduledVisits, onlyMine, page, size);

            if (patients == null)
            {
                return StatusCode(404, new ResponseModel { status = "404", message = "Not Found" });
            }
            else
            {
                return StatusCode(200, patients);
            }
        }

        [HttpPost("{id}/inspections")]
        public async Task<ResponseModel> createInspection(Guid id, InspectionCreateModel model)
        {
            var response = await _patientService.createInspection(id, model);

            return response;
        }
        // !!!!!!!!!!!!
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

        [HttpGet("{id}/inspections/search")]
        public IEnumerable<InspectionShortModel> getShortInspection(Guid id, [FromQuery] string request)
        {
            return _patientService.getShortInspection(id, request);
        }
    }
}
