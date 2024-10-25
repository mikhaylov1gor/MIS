using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace MIS.Models.DB
{
    public class MisDbContext : DbContext
    {
        public DbSet<DbComment> Comments { get; set; }
        public DbSet<DbConsultation> Consultations { get; set; }
        public DbSet<DbDiagnosis> Diagnosis { get; set; }
        public DbSet<DbDoctor> Doctors { get; set; }
        public DbSet<DbIcd10> Icd10 { get; set; }

        public DbSet<DbInspection> Inspections { get; set; }
        public DbSet<DbPatient> Patients { get; set; }
        public DbSet<DbSpecialty> Specialties { get; set; }

        public MisDbContext(DbContextOptions<MisDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}