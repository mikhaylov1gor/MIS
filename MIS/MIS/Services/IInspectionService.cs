using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MIS.Models.DB;
using MIS.Models.DTO;
using System.Threading.Tasks;
using System.Transactions;

namespace MIS.Services
{
    public interface IInspectionService
    {
        Task<InspectionModel> getInspection(Guid id);

        Task<ActionResult> editInspection(Guid id, InspectionEditModel inspectionEdit);

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
            var model = await _context.Inspections.FindAsync(id);

            if (model == null)
            {
                return null;
            }

            var transformedConsultations = model.consultations?.Select(c => new InspectionConsultationModel
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

        public async Task<ActionResult> editInspection(Guid id, InspectionEditModel inspectionEdit)
        {
            var inspection = await _context.Inspections.FindAsync(id);

            if (inspection == null)
            {
                return null;
            }
            else
            {
                inspection.anamnesis = inspectionEdit.anamnesis;
                inspection.complaints = inspectionEdit.complaints;
                inspection.treatment = inspectionEdit.treatment;
                inspection.conclusion = inspectionEdit.conclusion;
                inspection.nextVisitDate = inspectionEdit.nextVisitDate;
                inspection.deathDate = inspectionEdit.deathDate;

                inspection.diagnoses = new List<DbDiagnosis>();
                foreach (var d in inspectionEdit.diagnosis)
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
                            type = d.type,
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return null;
            }
        }

        public async Task<List<InspectionPreviewModel>> getChain(Guid id)
        {
            var rootInspection = await _context.Inspections.FindAsync(id);

            if (rootInspection == null)
            {
                return null;
            }

            var inspections = new List<InspectionPreviewModel>();
            while (rootInspection.previousInspectionId != null || rootInspection.id != rootInspection.baseInspectionId)
            {
                var mainDiagnosis = rootInspection.diagnoses.FirstOrDefault(t => t.type == DiagnosisType.Main);

                inspections.Add(new InspectionPreviewModel
                {
                    id = rootInspection.id,
                    createTime = rootInspection.createTime,
                    previousId = rootInspection.previousInspectionId,
                    date = rootInspection.date,
                    conclusion = rootInspection.conclusion,
                    doctorId = rootInspection.doctor.id,
                    patientId = rootInspection.patient.id,
                    diagnosis = new DiagnosisModel
                    {
                        id = mainDiagnosis.id,
                        createTime = mainDiagnosis.createTime,
                        code = mainDiagnosis.code,
                        name = mainDiagnosis.name,
                        description = mainDiagnosis.description,
                        type = mainDiagnosis.type,
                    },
                    hasChain = (rootInspection.previousInspectionId  != null),
                    hasNested = (rootInspection.nextVisitDate != null),
                });
            }

            if (inspections == null)
            {
                return null;
            }
            else
            {
                return inspections;
            }
        }
    }
}
