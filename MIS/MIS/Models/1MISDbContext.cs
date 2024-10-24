using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MIS.Models;

namespace MIS.Models
{
    public class MisDbContext : DbContext
    {
        public DbSet<CommentModel> Comments { get; set; }
        public DbSet<ConsultationModel> Consultations { get; set; }
        public DbSet<DiagnosisModel> Diagnosis { get; set; }
        public DbSet<DoctorModel> Doctors { get; set; }

        public DbSet<InspectionModel> Inspections { get; set; }
        public DbSet<PatientModel> Patients { get; set; }
        public DbSet<ResponseModel> Responses { get; set; }
        public DbSet<SpecialityModel> Specialties { get; set; }

        public MisDbContext(DbContextOptions<MisDbContext> options) : base(options){}
    }
}