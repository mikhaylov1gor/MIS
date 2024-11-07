using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MIS.Middleware;
using MIS.Models.DB;
using MIS.Models.DTO;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MIS.Services
{
    public interface IInspectionService
    {
        Task<InspectionModel> getInspection(Guid id);

        Task<ActionResult> editInspection(Guid id, InspectionEditModel inspectionEdit, ClaimsPrincipal user);

        Task<List<InspectionPreviewModel>> getChain(Guid id);

    }
    public class InspectionService : IInspectionService
    {
        private readonly MisDbContext _context;

        public InspectionService(MisDbContext context)
        {
            _context = context;
        }
        public async Task<InspectionModel> getInspection(Guid id)
        {
            var model = await _context.Inspections
                .Include(d => d.doctor)
                .Include(p => p.patient)
                .Include(d => d.diagnoses)
                .Include(c => c.consultations)
                    .ThenInclude(s => s.rootComment)
                .Include(c => c.consultations)
                    .ThenInclude(s => s.speciality)
                .Where(i => i.id == id)
                .FirstOrDefaultAsync();

            if (model == null)
            {
                throw new KeyNotFoundException("inspection not found"); //ex
            }

            var transformedConsultations = model.consultations?
                .Select(c => new InspectionConsultationModel
            {
                id = c.id,
                createTime = c.createTime,
                inspectionId = c.inspectionId,
                speciality = new SpecialtyModel
                {
                    id = c.speciality.id,
                    createTime = c.speciality.createTime,
                    name = c.speciality.name,
                },
                rootComment = new InspectionCommentModel
                {
                    id = c.rootComment.id,
                    createTime = c.rootComment.createTime,
                    parentId = c.rootComment.parentId,
                    content = c.rootComment.content,
                    author = new DoctorModel
                    {
                        id = c.rootComment.author.id,
                        createTime = c.rootComment.author.createTime,
                        name = c.rootComment.author.name,
                        birthday = c.rootComment.author.birthday,
                        gender = c.rootComment.author.gender,
                        email = c.rootComment.author.email,
                        phone = c.rootComment.author.phone,
                    }
                },
                commentsNumber = c.commentsNumber,
            }).ToList();

            var transformedDiagnoses = model.diagnoses
                                .Select(d => new DiagnosisModel
                                {
                                    id = d.id,
                                    createTime = d.createTime,
                                    code = d.code,
                                    name = d.name,
                                    description = d.description,
                                    type = d.type
                                }).ToList();

            var inspection = new InspectionModel
            {
                id = id,
                createTime = model.createTime,
                date = model.date,
                anamnesis = model.anamnesis,
                complaints = model.complaints,
                treatment = model.treatment,
                conclusion = model.conclusion,
                nextVisitDate = model.nextVisitDate,
                deathDate = model.deathDate,
                baseInspectionId = model.baseInspectionId,
                previousInspectionId = model.previousInspectionId,
                patient = new PatientModel
                {
                    id = model.patient.id,
                    createTime = model.patient.createTime,
                    name = model.patient.name,
                    birthday = model.patient.birthday,
                    gender = model.patient.gender,
                },
                doctor = new DoctorModel
                {
                    id = model.doctor.id,
                    createTime = model.doctor.createTime,
                    name = model.doctor.name,
                    birthday = model.doctor.birthday,
                    gender = model.doctor.gender,
                    email = model.doctor.email,
                    phone = model.doctor.phone,
                },
                diagnoses = transformedDiagnoses,
                consultations = transformedConsultations,
            };

            return inspection;
        }

        public async Task<ActionResult> editInspection(Guid id, InspectionEditModel inspectionEdit, ClaimsPrincipal user)
        {
            // проверка на аутентификацию
            var doctorId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (doctorId == null || !Guid.TryParse(doctorId, out var parsedId))
                throw new UnauthorizedAccessException(); //ex

            // проверка на наличие
            var inspection = await _context.Inspections
                .Include(d=>d.diagnoses)
                .Include(d=>d.doctor)
                .Where(i=>i.id == id)
                .FirstOrDefaultAsync();

            if (inspection == null)
                throw new KeyNotFoundException("inspection not found"); // ex

            // проверка на права доступа
            if (inspection.doctor.id != parsedId)
                throw new ForbiddenAccessException("forbidden"); //ex


            inspection.anamnesis = inspectionEdit.anamnesis;
            inspection.complaints = inspectionEdit.complaints;
            inspection.treatment = inspectionEdit.treatment;
            inspection.conclusion = inspectionEdit.conclusion;
            inspection.nextVisitDate = inspectionEdit.nextVisitDate;
            inspection.deathDate = inspectionEdit.deathDate;

            if (inspection.deathDate != null) { inspection.conclusion = Conclusion.Death; }

            // доп проверки
            if (inspection.nextVisitDate < inspection.date)
                throw new ValidationAccessException("wrong next visit date"); //ex

            if (inspectionEdit.conclusion == Conclusion.Death && inspectionEdit.nextVisitDate != null)
                throw new ValidationAccessException("patient dead, but next visit date exists"); //ex

            if (inspectionEdit.conclusion == Conclusion.Death && inspection.hasNested)
                throw new ValidationAccessException("patient has nested inspections, death conclusion forbidden"); // ex

            // очистка диагнозов 
            if (inspection.diagnoses != null)
            {
                _context.Diagnosis.RemoveRange(inspection.diagnoses);
            }

            inspection.diagnoses.Clear();

            inspection.diagnoses = new List<DbDiagnosis>();
            if (inspectionEdit.diagnosis.Count < 1)
                throw new ValidationAccessException("diagnosis count must be greater than 0"); //ex

            bool isMainExist = false;

            foreach (var d in inspectionEdit.diagnosis)
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
                inspection.diagnoses.Add(new DbDiagnosis
                {
                    createTime = DateTime.UtcNow,
                    code = icd10Record.code,
                    name = icd10Record.name,
                    description = d.description,
                    type = d.type,
                });
                if (!isMainExist)
                    throw new ValidationAccessException("there is no main diagnosis");//ex
            }

            await _context.SaveChangesAsync();
            return null;

        }

        public async Task<List<InspectionPreviewModel>> getChain(Guid id)
        {
            var rootInspection = await _context.Inspections.FindAsync(id);

            if (rootInspection == null)
            {
                throw new KeyNotFoundException("root inspection not found"); // ex
            }

            var inspections = new List<InspectionPreviewModel>();

            if (rootInspection.hasChain)
            {
                var nextInspection = await _context.Inspections
                    .Include(d=>d.diagnoses)
                    .Include(d=>d.doctor)
                    .Include(p=>p.patient)
                    .Where(p => p.previousInspectionId == rootInspection.id)
                    .FirstOrDefaultAsync();

                var mainDiagnosis = nextInspection.diagnoses
                    .Where(t => t.type == DiagnosisType.Main)
                    .FirstOrDefault();

                inspections.Add(new InspectionPreviewModel
                {
                    id = nextInspection.id,
                    createTime = nextInspection.createTime,
                    previousId = nextInspection.previousInspectionId,
                    date = nextInspection.date,
                    conclusion = nextInspection.conclusion,
                    doctorId = nextInspection.doctor.id,
                    patientId = nextInspection.patient.id,
                    diagnosis = new DiagnosisModel
                    {
                        id = mainDiagnosis.id,
                        createTime = mainDiagnosis.createTime,
                        code = mainDiagnosis.code,
                        name = mainDiagnosis.name,
                        description = mainDiagnosis.description,
                        type = mainDiagnosis.type,
                    },
                    hasChain = nextInspection.hasChain,
                    hasNested = nextInspection.hasNested,
                });

                while (nextInspection.hasNested)
                {
                    nextInspection = await _context.Inspections
                        .Include(d => d.diagnoses)
                        .Include(d => d.doctor)
                        .Include(p => p.patient)
                        .Where(p => p.previousInspectionId == nextInspection.id)
                        .FirstOrDefaultAsync();

                    mainDiagnosis = nextInspection.diagnoses
                        .Where(t => t.type == DiagnosisType.Main)
                        .FirstOrDefault();

                    inspections.Add(new InspectionPreviewModel
                    {
                        id = nextInspection.id,
                        createTime = nextInspection.createTime,
                        previousId = nextInspection.previousInspectionId,
                        date = nextInspection.date,
                        conclusion = nextInspection.conclusion,
                        doctorId = nextInspection.doctor.id,
                        patientId = nextInspection.patient.id,
                        diagnosis = new DiagnosisModel
                        {
                            id = mainDiagnosis.id,
                            createTime = mainDiagnosis.createTime,
                            code = mainDiagnosis.code,
                            name = mainDiagnosis.name,
                            description = mainDiagnosis.description,
                            type = mainDiagnosis.type,
                        },
                        hasChain = nextInspection.hasChain,
                        hasNested = nextInspection.hasNested,
                    });

                }
            }
            return inspections;
        }

    }
}
