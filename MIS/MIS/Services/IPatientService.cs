using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using MIS.Models;

namespace MIS.Services
{
    public interface IPatientService
    {
        bool createPatient(PatientCreateModel patient);

        PatientPagedListModel getPatients(
            [FromQuery] string name,
            [FromQuery] Conclusion[] conclusions,
            [FromQuery] PatientSorting sorting,
            [FromQuery] bool scheduledVisits,
            [FromQuery] bool onlyMine,
            [FromQuery] int page,
            [FromQuery] int size);

        bool createInspection(Guid id, InspectionCreateModel model);

        InspectionPagedListModel getInspections(
            Guid id,
            [FromQuery] bool grouped,
            [FromQuery] List<Guid> icdRoots,
            [FromQuery] int page,
            [FromQuery] int size);

        PatientModel getPatient(Guid id);

        InspectionShortModel[] getShortInspection(Guid id, [FromQuery] string request);
    }

    public class PatientService : IPatientService
    {
        public bool createPatient(PatientCreateModel patient)
        {
            return true;
        }

        public PatientPagedListModel getPatients(
            [FromQuery] string name,
            [FromQuery] Conclusion[] conclusions,
            [FromQuery] PatientSorting sorting,
            [FromQuery] bool scheduledVisits,
            [FromQuery] bool onlyMine,
            [FromQuery] int page,
            [FromQuery] int size)
        {
            return null;
        }

        public bool createInspection(Guid id, InspectionCreateModel model)
        {
            return true;
        }

        public InspectionPagedListModel getInspections(
            Guid id,
            [FromQuery] bool grouped,
            [FromQuery] List<Guid> icdRoots,
            [FromQuery] int page,
            [FromQuery] int size)
        {
            return null;
        }

        public PatientModel getPatient(Guid id)
        {
            return null;
        }

        public InspectionShortModel[] getShortInspection(Guid id, [FromQuery] string request)
        {
            return null;
        }
    }

}
