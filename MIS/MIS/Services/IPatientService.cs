using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIS.Middleware;
using MIS.Models.DB;
using MIS.Models.DTO;
using System.Diagnostics;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MIS.Services
{
    public interface IPatientService
    {
        Task<Guid> createPatient(PatientCreateModel patient);

        Task<PatientPagedListModel> getPatients(
                    [FromQuery] string name,
                    [FromQuery] Conclusion[] conclusions,
                    [FromQuery] PatientSorting sorting,
                    [FromQuery] bool scheduledVisits,
                    [FromQuery] bool onlyMine,
                    [FromQuery] int page,
                    [FromQuery] int size,
                    ClaimsPrincipal user);

        Task<Guid> createInspection(Guid id, InspectionCreateModel model, ClaimsPrincipal user);

        Task<InspectionPagedListModel> getInspections(
                    Guid PatientId,
                    [FromQuery] bool grouped,
                    [FromQuery] List<Guid> icdRoots,
                    [FromQuery] int page,
                    [FromQuery] int size);

        Task<PatientModel> getPatient(Guid id);

        Task<List<InspectionShortModel>> getShortInspection(Guid id, string request);
    }

    public class PatientService : IPatientService
    {
        private readonly MisDbContext _context;

        public PatientService(MisDbContext context)
        {
            _context = context;
        }

        // Создать пациента
        public async Task<Guid> createPatient(PatientCreateModel patient)
        {
            if (patient.birthday > DateTime.UtcNow)
            {
                throw new ValidationAccessException(); //ex
            }
            DbPatient newPatient = new DbPatient { createTime = DateTime.UtcNow, birthday = patient.birthday, name = patient.name, gender = patient.gender };

            await _context.Patients.AddAsync(newPatient);
            await _context.SaveChangesAsync();

            return newPatient.id;
        }

        // получить лист пациентов
        public async Task<PatientPagedListModel> getPatients(
            [FromQuery] string name,
            [FromQuery] Conclusion[] conclusions,
            [FromQuery] PatientSorting sorting,
            [FromQuery] bool scheduledVisits,
            [FromQuery] bool onlyMine,
            [FromQuery] int page,
            [FromQuery] int size,
            ClaimsPrincipal user)
        {
            // проверка на валидность пагинации
            if (page < 1 || size < 1)
            {
                throw new ValidationAccessException();
            }
            // проверка на аутентификацию
            var doctorId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (doctorId == null || !Guid.TryParse(doctorId, out var parsedId))
            {
                throw new UnauthorizedAccessException();
            }

            // выборка по имени
            var query = _context.Patients
                .Where(i => EF.Functions.Like(i.name, $"%{name}%"));

            // выборка по заключениям
            if (conclusions.Length > 0)
            {
                query = query
                    .Where(p => p.inspections.Any(i => conclusions.Contains(i.conclusion)));
            }

            // выборка по доктору
            if (onlyMine)
            { 
                query = query
                    .Where(p => p.inspections.Any(d => d.doctor.id == parsedId));
            }

            // выборка по предстоящему визиту
            if (scheduledVisits)
            {
                query = query
                    .Where(p => p.inspections.Any(n => n.nextVisitDate != null));
            }

            // сортировка
            switch (sorting)
            {
                case PatientSorting.NameAsc:
                    query = query.OrderBy(p => p.name);
                    break;

                case PatientSorting.NameDesc:
                    query = query.OrderByDescending(p => p.name);
                    break;

                case PatientSorting.CreateAsc:
                    query = query.OrderBy(p => p.createTime);
                    break;

                case PatientSorting.CreateDesc:
                    query = query.OrderByDescending(p => p.createTime);
                    break;

               case PatientSorting.InspectionAcs:
                    query = query.OrderBy(p => p.inspections.Min(i => i.createTime));
                    break;

                case PatientSorting.InspectionDesc:
                    query = query.OrderByDescending(p => p.inspections.Min(i => i.createTime));
                    break;

                default:
                    query = query.OrderBy(p => p.name);
                    break;
            }

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

        // создать осмотр
        public async Task<Guid> createInspection(Guid id, InspectionCreateModel model, ClaimsPrincipal user)
        {
            // проверка на аутентификацию
            var doctorId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (doctorId == null || !Guid.TryParse(doctorId, out var parsedId))
            {
                throw new UnauthorizedAccessException();
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.id == parsedId);

            // проверка на наличие пациента
            var rootPatient = await _context.Patients.FindAsync(id);

            if (rootPatient == null)
            {
                throw new KeyNotFoundException(); //ex
            }

            // проверка предыдущего осмотра (проверка на смерть)
            var previousInspection = await _context.Inspections.FindAsync(model.previousInspectionId);

            if (previousInspection != null && previousInspection.conclusion == Conclusion.Death)
            {
                throw new ValidationAccessException(); //ex
            }

            // доп проверки
            if (model.conclusion == Conclusion.Death && model.nextVisitDate == null ||
                model.nextVisitDate < model.date)
            {
                throw new ValidationAccessException(); //ex
            }
            if (model.deathDate != null) { model.conclusion = Conclusion.Death; }
            // создание модели осмотра
            var inspection = new DbInspection
            {
                createTime = DateTime.UtcNow,
                date = model.date,
                anamnesis = model.anamnesis,
                complaints = model.complaints,
                treatment = model.treatment,
                conclusion = model.conclusion,
                nextVisitDate = model.nextVisitDate,
                deathDate = model.deathDate,
                previousInspectionId = model.previousInspectionId,
                patient = rootPatient,
                doctor = doctor
            };
       
            if (inspection.previousInspectionId == null)
            {
                inspection.baseInspectionId = inspection.id;
            }
            else
            {
                previousInspection = await _context.Inspections.FindAsync(inspection.previousInspectionId);
                if (previousInspection == null)
                {
                    throw new KeyNotFoundException(); //ex
                }
                inspection.baseInspectionId = previousInspection.baseInspectionId;
            }


            // трансформация консультаций
            inspection.consultations = new List<DbInspectionConsultation>();

            foreach (var consultation in model.consultations)
            {
                var DBSpecialty = await _context.Specialties.FindAsync(consultation.specialityId);
                if (DBSpecialty == null) 
                {
                    throw new KeyNotFoundException(); //ex
                }
                inspection.consultations.Add(new DbInspectionConsultation
                {
                    createTime = DateTime.UtcNow,
                    inspectionId = inspection.id,
                    speciality = DBSpecialty,
                    rootComment = new DbInspectionComment
                    {
                        createTime = DateTime.UtcNow,
                        parentId = null,
                        content = consultation.comment.content,
                        author = doctor,
                        modifyTime = DateTime.UtcNow,
                    },
                    commentsNumber = 1
                });
            }

            // трансформация диагнозов
            inspection.diagnoses = new List<DbDiagnosis>();
            bool isMainExist = false; // проверка на один мейн диагноз

            foreach (var d in model.diagnosis)
            {
                var icd10Record = await _context.Icd10.FindAsync(d.icdDiagnosisId);
                if (icd10Record == null)
                {
                    throw new KeyNotFoundException(); //ex
                }
                if (d.type == DiagnosisType.Main)
                {
                    if (isMainExist) throw new ValidationAccessException();
                    else isMainExist = true;
                    if (icd10Record.parentId != null) throw new ValidationAccessException(); // если основной диагноз не является корневым элементов мкб
                }
                {
                    inspection.diagnoses.Add(new DbDiagnosis
                    {
                        createTime = DateTime.UtcNow,
                        code = icd10Record.code,
                        name = icd10Record.name,
                        description = d.description,
                        type = d.type
                    });

                };
            }

            await _context.AddAsync(inspection);
            await _context.SaveChangesAsync();
            return inspection.id;

        }
        // !!!!!!!!!!!!
        public async Task<InspectionPagedListModel> getInspections(
            Guid PatientId,
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
                throw new KeyNotFoundException(); //ex
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

        public async Task<List<InspectionShortModel>> getShortInspection(Guid id, string request)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                throw new KeyNotFoundException();
            }

            var query = patient.inspections
                .Where(d => d.diagnoses.Any(r => EF.Functions.Like(r.name, $"%{request}") || EF.Functions.Like(r.code, $"%{request}")));

            if (query.Count() == 0)
            {
                return null;
            }
            var inspections = new List<InspectionShortModel>();

            foreach ( var inspectionDb in query)
            {
                var diagnosisDb = inspectionDb.diagnoses.FirstOrDefault(t => t.type == DiagnosisType.Main);
                var diagnosis = new DiagnosisModel
                {
                    id = diagnosisDb.id,
                    createTime = diagnosisDb.createTime,
                    code = diagnosisDb.code,
                    name = diagnosisDb.name,
                    description = diagnosisDb.description,
                    type = diagnosisDb.type,
                };

                var inspection = new InspectionShortModel
                {
                    id = inspectionDb.id,
                    createTime = inspectionDb.createTime,
                    date = inspectionDb.date,
                    diagnosis = diagnosis
                };
            }  

            return inspections;
        }
    }

}
