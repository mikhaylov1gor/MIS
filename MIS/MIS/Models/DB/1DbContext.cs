using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace MIS.Models.DB
{
    public class MisDbContext : DbContext
    {
        public DbSet<DbDiagnosis> Diagnosis { get; set; }
        public DbSet<DbDoctor> Doctors { get; set; }
        public DbSet<DbIcd10> Icd10 { get; set; }

        public DbSet<DbInspection> Inspections { get; set; }
        public DbSet<DbInspectionComment> InspectionComments { get; set; }
        public DbSet<DbInspectionConsultation> InspetionConsultations { get; set; }

        public DbSet<DbPatient> Patients { get; set; }
        public DbSet<DbSpecialty> Specialties { get; set; }

        public DbSet<DbTokenBlackList> TokenBlackList { get; set; }

        public MisDbContext(DbContextOptions<MisDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbIcd10>()
                .HasIndex(e => e.parentId)
                .HasDatabaseName("IX_icd10_parentId");
        }
    }
}