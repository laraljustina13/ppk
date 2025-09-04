using MedicalSystemApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalSystemApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Examination> Examinations { get; set; }
        public DbSet<ExaminationFile> ExaminationFiles { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure unique constraint for OIB
            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.OIB)
                .IsUnique();

            // Configure examination type constraint
            var examinationTypes = new[] { "GP", "KRV", "X-RAY", "CT", "MR", "ULTRA", "EKG", "ECHO", "EYE", "DERM", "DENTA", "MAMMO", "NEURO" };
            modelBuilder.Entity<Examination>()
                .Property(e => e.ExaminationType)
                .HasConversion<string>()
                .HasMaxLength(10);

            // Configure relationships
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(mr => mr.Patient)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(mr => mr.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Examination>()
                .HasOne(e => e.Patient)
                .WithMany(p => p.Examinations)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExaminationFile>()
                .HasOne(ef => ef.Examination)
                .WithMany(e => e.ExaminationFiles)
                .HasForeignKey(ef => ef.ExaminationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Examination)
                .WithMany(e => e.Prescriptions)
                .HasForeignKey(p => p.ExaminationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}