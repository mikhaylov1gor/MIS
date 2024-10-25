using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MIS.Models;
using MIS.Services;

namespace MIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        public PatientService _patientService;

        [HttpPost]
        public bool createPatient(PatientCreateModel patient)
        {
            return _patientService.createPatient(patient);
        }

        [HttpGet]
        public ActionResult<PatientPagedListModel> getPatients(
            [FromQuery] string name,
            [FromQuery] Conclusion[] conclusions,
            [FromQuery] PatientSorting sorting,
            [FromQuery] bool scheduledVisits,
            [FromQuery] bool onlyMine,
            [FromQuery] int page = 1,
            [FromQuery] int size = 5)
        {
            return _patientService.getPatients(name, conclusions, sorting, scheduledVisits, onlyMine, page, size) ;
        }

        [HttpPost("{id}/inspections")]
        public bool createInspection(Guid id, InspectionCreateModel model)
        {
            return _patientService.createInspection(id, model);
        }

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
        public PatientModel getPatient(Guid id)
        {
            return _patientService.getPatient(id);
        }

        [HttpGet("{id}/inspections/search")]
        public IEnumerable<InspectionShortModel> getShortInspection(Guid id, [FromQuery] string request)
        {
            return _patientService.getShortInspection(id, request);
        }
    }
}
