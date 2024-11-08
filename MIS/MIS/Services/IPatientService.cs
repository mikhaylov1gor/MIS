using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Abstractions;
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
                throw new ValidationAccessException("page or size must be greater than one");
            }
            // проверка на аутентификацию
            var doctorId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (doctorId == null || !Guid.TryParse(doctorId, out var parsedId))
            {
                throw new UnauthorizedAccessException();
            }

            // выборка по имени
            var query = _context.Patients
                .Include(p => p.inspections)
                    .ThenInclude(i => i.doctor)
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
                    .Where(p => p.inspections.Any(n => n.nextVisitDate != null && !n.hasNested));
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
                throw new UnauthorizedAccessException();

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.id == parsedId);

            // проверка на наличие пациента
            var rootPatient = await _context.Patients.FindAsync(id);

            if (rootPatient == null)
                throw new KeyNotFoundException("patient not found"); //ex

            // проверка на валидацию модели
            if (model.date == null || model.complaints == null || model.anamnesis == null || model.treatment == null)
                throw new ValidationAccessException("some of model parameters is required"); //ex

            // проверка предыдущего осмотра (проверка на смерть/выздоровление, на наличие дочернего)
            var previousInspection = await _context.Inspections.FindAsync(model.previousInspectionId);

            if (previousInspection != null && previousInspection.conclusion == Conclusion.Death)
                throw new ValidationAccessException("patient dead"); //ex

            if (previousInspection != null && previousInspection.conclusion == Conclusion.Recovery)
                throw new ValidationAccessException("patient recovered"); //ex

            if (previousInspection != null && previousInspection.hasNested)
                throw new ValidationAccessException("this inspection already have child inspection"); //

            // доп проверки
            if (previousInspection != null && previousInspection.date > model.date) 
                throw new ValidationAccessException("date of inspection must be less than parent inspection date"); //ex

            if (model.conclusion == Conclusion.Disease && !model.nextVisitDate.HasValue)
                throw new ValidationAccessException("model conclusion id disease but next visit date doesnt exists"); //ex

            if (model.conclusion == Conclusion.Recovery && model.nextVisitDate.HasValue)
                throw new ValidationAccessException("model conclusion is recovery but next visit date exists");//ex

            if (model.conclusion == Conclusion.Death && model.nextVisitDate.HasValue)
                throw new ValidationAccessException("model conculsion is death but next visit date exists");//ex

            if (model.conclusion == Conclusion.Death && !model.deathDate.HasValue)
                throw new ValidationAccessException("model conclusion is death but death date doesnt exists"); //ex

            if (model.nextVisitDate < model.date)
                throw new ValidationAccessException("next visit date must be greater than inspection's date"); // ex

            if (model.deathDate.HasValue && model.deathDate > DateTime.UtcNow)
                throw new ValidationAccessException("death date must be less than current DateTime"); // ex

            if (model.date > DateTime.UtcNow)
                throw new ValidationAccessException("inspection's date must be less than current DateTime"); //ex

            if (model.deathDate.HasValue && model.conclusion != Conclusion.Death)
                throw new ValidationAccessException("death date exists, but conclusion isnt death"); //ex

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
            await _context.AddAsync(inspection);
            // трансформация консультаций
            inspection.consultations = new List<DbInspectionConsultation>();

            foreach (var consultation in model.consultations)
            {
                var DBSpecialty = await _context.Specialties.FindAsync(consultation.specialityId);

                if (DBSpecialty == null) 
                    throw new KeyNotFoundException("specialty not found"); //ex

                if (inspection.consultations.Any(s => s.speciality.id == DBSpecialty.id) | consultation.comment == null)
                    throw new ValidationAccessException("this specialty already exists or comment is null"); //ex

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
                    throw new KeyNotFoundException("icdDiagnosis not found"); //ex
                }
                if (d.type == DiagnosisType.Main)
                {
                    if (isMainExist) throw new ValidationAccessException("there should be only one main diagnosis");
                    else isMainExist = true;
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
            if (!isMainExist)
                throw new ValidationAccessException("there is no main diagnosis");//ex

            //

            if (model.previousInspectionId == null)
            {
                inspection.baseInspectionId = inspection.id;
            }
            else
            {
                previousInspection = await _context.Inspections.FindAsync(inspection.previousInspectionId);
                if (previousInspection == null)
                    throw new KeyNotFoundException("previous inspection not found"); //ex

                inspection.baseInspectionId = previousInspection.baseInspectionId;
            }

            if (inspection.previousInspectionId != null)
            {
                var prev = await _context.Inspections.FindAsync(inspection.previousInspectionId);
                prev.nextVisitDate = inspection.date;
                if (prev.hasNested)
                    throw new ValidationAccessException("this inspection already have child inspection");//ex

                prev.hasNested = true;
                if (prev.baseInspectionId == prev.id)
                    prev.hasChain = true;
            }

            await _context.SaveChangesAsync();
            return inspection.id;

        }

        public async Task<InspectionPagedListModel> getInspections(
            Guid PatientId,
            [FromQuery] bool grouped,
            [FromQuery] List<Guid> icdRoots,
            [FromQuery] int page,
            [FromQuery] int size)
        {
            var patient = await _context.Patients
                .Where(i => i.id == PatientId)
                .FirstOrDefaultAsync();

            if (patient == null)
                throw new KeyNotFoundException("patient not found"); //ex

            if (page < 1 || size < 1)
                throw new ValidationAccessException("page or size must be greater than 0");


            // проверка рутовых диагнозов
            if (icdRoots.Count == 0)
            {
                icdRoots = await _context.Icd10
                    .Where(p => p.parentId == null)
                    .Select(p => p.id)
                    .ToListAsync();
            }
            else
            {
                var roots = await _context.Icd10
                    .Where(p => p.parentId == null)
                    .Select(p => p.id)
                    .ToListAsync();

                foreach (var root in icdRoots)
                {
                    if (!roots.Contains(root))
                    {
                        throw new ValidationAccessException("icdRoot not found");//ex
                    }
                }
            }

            var icdCodes = await _context.Icd10
                .Where(i => icdRoots.Contains(i.id))
                .Select(c => c.code)
                .ToListAsync();
            var query = new List<DbInspection>();

            if (grouped)
            {
                query = await _context.Inspections
                    .Include(p => p.patient)
                    .Include(d => d.diagnoses)
                    .Include(d => d.doctor)
                    .Where(p => p.patient.id == PatientId &&
                                p.diagnoses.Any(d => icdCodes.Contains(d.code)) &&
                                (p.previousInspectionId == null))
                    .Skip((page - 1) * size)
                    .Take(size)
                    .ToListAsync();
            }
            else
            {
                query = await _context.Inspections
                   .Include(p => p.patient)
                   .Include(d => d.diagnoses)
                   .Include(d => d.doctor)
                   .Where(p => p.patient.id == PatientId &&
                               p.diagnoses.Any(d => icdCodes.Contains(d.code)))
                   .Skip((page - 1) * size)
                   .Take(size)
                   .ToListAsync();
            }

            var inspections = new List<InspectionPreviewModel>();
            foreach (var inspection in query)
            {
                var mainDiagnosis = inspection.diagnoses
                    .Where(d => d.type == DiagnosisType.Main)
                    .FirstOrDefault();

                var previewInspection = new InspectionPreviewModel
                {
                    id = inspection.id,
                    createTime = inspection.createTime,
                    previousId = inspection.previousInspectionId,
                    date = inspection.date,
                    conclusion = inspection.conclusion,
                    doctorId = inspection.doctor.id,
                    doctor = inspection.doctor.name,
                    patientId = inspection.patient.id,
                    patient = inspection.patient.name,
                    diagnosis = new DiagnosisModel
                    {
                        id = mainDiagnosis.id,
                        createTime = mainDiagnosis.createTime,
                        code = mainDiagnosis.code,
                        name = mainDiagnosis.name,
                        description = mainDiagnosis.description,
                        type = mainDiagnosis.type,
                    },
                    hasChain = inspection.hasChain,
                    hasNested = inspection.hasNested,
                };

                inspections.Add(previewInspection);
            }

            var pageInfo = new PageInfoModel
            {
                size = size,
                count = (int)Math.Ceiling((double)inspections.Count/size),
                current = page
            };

            return new InspectionPagedListModel { inspections = inspections, pagination = pageInfo };
        }

        public async Task<PatientModel> getPatient(Guid id)
        {
            var DBpatient = await _context.Patients.FindAsync(id);

            if (DBpatient == null)
            {
                throw new KeyNotFoundException("patient not found"); //ex
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
            var patient = await _context.Patients
                .Where(i=>i.id == id)
                .FirstOrDefaultAsync();

            if (patient == null)
            {
                throw new KeyNotFoundException("patient not found");
            }

            var query = await _context.Inspections
                .Include(d=>d.diagnoses)
                .Where(i => i.patient.id == id &&
                             i.diagnoses.Any(d =>
                                EF.Functions.Like(d.name, $"%{request}%") ||
                                EF.Functions.Like(d.code, $"%{request}%")))
                .ToListAsync();

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

                inspections.Add(inspection);
            }  

            return inspections;
        }
    }

}
