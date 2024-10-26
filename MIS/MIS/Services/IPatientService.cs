using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS.Models.DB;
using MIS.Models.DTO;

namespace MIS.Services
{
    public interface IPatientService
    {
        Task createPatient(PatientCreateModel patient);

        Task<PatientPagedListModel> getPatients(
            [FromQuery] string name,
            [FromQuery] Conclusion[] conclusions,
            [FromQuery] PatientSorting sorting,
            [FromQuery] bool scheduledVisits,
            [FromQuery] bool onlyMine,
            [FromQuery] int page,
            [FromQuery] int size);

        Task<ResponseModel> createInspection(Guid id, InspectionCreateModel model);

        InspectionPagedListModel getInspections(
            Guid id,
            [FromQuery] bool grouped,
            [FromQuery] List<Guid> icdRoots,
            [FromQuery] int page,
            [FromQuery] int size);

        Task<PatientModel> getPatient(Guid id);

        InspectionShortModel[] getShortInspection(Guid id, [FromQuery] string request);
    }

    public class PatientService : IPatientService
    {
        private readonly MisDbContext _context;

        public PatientService(MisDbContext context)
        {
            _context = context;
        }

        // Создать пациента
        public async Task createPatient(PatientCreateModel patient)
        {
            DbPatient newPatient = new DbPatient { createTime = DateTime.Now, birthday = patient.birthday, name = patient.name, gender = patient.gender };

            await _context.Patients.AddAsync(newPatient);
            await _context.SaveChangesAsync();
        }

        public async Task<PatientPagedListModel> getPatients(
            [FromQuery] string name,
            [FromQuery] Conclusion[] conclusions,
            [FromQuery] PatientSorting sorting,
            [FromQuery] bool scheduledVisits,
            [FromQuery] bool onlyMine,
            [FromQuery] int page,
            [FromQuery] int size)
        {
            var query = _context.Patients
                .Where(i => EF.Functions.Like(i.name, $"%{name}%"));

            var totalItems = await query.CountAsync();

            var patients = await query
                .Skip((page - 1) * size)
                .Take(size)
                .Select(i => new PatientModel { id = i.id, createTime = i.createTime, name = i.name, birthday = i.birthday, gender = i.gender })
                .ToListAsync();

            var pageInfo = new PageInfoModel
            {
                size = size,
                count = (int)Math.Ceiling((double)totalItems / size),
                current = page
            };


            return new PatientPagedListModel
            {
                patients = patients,
                pagination = pageInfo

            };
        }

        public async Task<ResponseModel> createInspection(Guid id, InspectionCreateModel model)
        {
            var rootPatient = await _context.Patients.FindAsync(id);

            if (rootPatient == null)
            {
                return new ResponseModel { status = "404", message = "Not Found" };
            }


            var inspection = new DbInspection
            {
                createTime = DateTime.Now,
                date = model.date,
                anamnesis = model.anamnesis,
                complaints = model.complaints,
                treatment = model.treatment,
                conclusion = model.conclusion,
                nextVisitDate = model.nextVisitDate,
                deathDate = model.deathDate,
                //baseInspectionId
                previousInspectionId = model.previousInspectionId,
                patient = rootPatient,
                //doctor
            };

            inspection.consultations = new List<InspectionConsultationModel>();
            foreach (var d in model.consultations)
            {
                var DBSpecialty = await _context.Specialties.FindAsync(d.specialityId);
                if (DBSpecialty == null) 
                {
                    return new ResponseModel { status = "404", message = "Specialty Not Found" };
                }
                inspection.consultations.Add(new InspectionConsultationModel
                {
                    createTime = DateTime.Now,
                    inspectionId = inspection.id,
                    speciality = new SpecialtyModel
                    {
                        createTime = DBSpecialty.createTime,
                        name = DBSpecialty.name
                    }
                });
            }

            inspection.diagnoses = new List<DbDiagnosis>();
            foreach (var d in model.diagnosis)
            {
                var icd10Record = await _context.Icd10.FindAsync(d.icdDiagnosisId);
                if (icd10Record != null)
                {
                    inspection.diagnoses.Add(new DbDiagnosis
                    {
                        createTime = DateTime.Now,
                        code = icd10Record.code,
                        name = icd10Record.name,
                        description = d.description,
                        type = d.type
                    });
                }
            };

            await _context.AddAsync(inspection);
            await _context.SaveChangesAsync();
            return new ResponseModel { status = "200", message = "Success" };

        }
        // !!!!!!!!!!!!
        public InspectionPagedListModel getInspections(
            Guid id,
            [FromQuery] bool grouped,
            [FromQuery] List<Guid> icdRoots,
            [FromQuery] int page,
            [FromQuery] int size)
        {
            return null;
        }

        public async Task<PatientModel> getPatient(Guid id)
        {
            var DBpatient = await _context.Patients.FindAsync(id);

            if (DBpatient == null)
            {
                return null;
            }

            else
            {
                var patient = new PatientModel
                {
                    id = DBpatient.id,
                    createTime = DBpatient.createTime,
                    name = DBpatient.name,
                    birthday = DBpatient.birthday,
                    gender = DBpatient.gender,
                };
                return patient;
            }
        }

        public InspectionShortModel[] getShortInspection(Guid id, [FromQuery] string request)
        {
            return null;
        }
    }

}
