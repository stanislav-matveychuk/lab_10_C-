using MedicalPortal.Domain;
using Microsoft.EntityFrameworkCore;
namespace MedicalPortal.Infrastructure;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
    public DbSet<TelemedicineSession> Sessions => Set<TelemedicineSession>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Appointment>().HasOne<Doctor>().WithMany().HasForeignKey(x => x.DoctorId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Appointment>().HasOne<Patient>().WithMany().HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Appointment>().HasIndex(x => new { x.DoctorId, x.StartsAtUtc }).IsUnique();
        b.Entity<Appointment>().HasIndex(x => new { x.PatientId, x.StartsAtUtc }).IsUnique();
        b.Entity<MedicalRecord>().HasOne<Appointment>().WithMany().HasForeignKey(x => x.AppointmentId);
        b.Entity<TelemedicineSession>().HasOne<Appointment>().WithOne().HasForeignKey<TelemedicineSession>(x => x.AppointmentId);
        b.Entity<Doctor>().HasData(new Doctor { Id = 1, Name = "Олена Коваль (демо)", Specialty = "Терапевт" });
        b.Entity<Patient>().HasData(new Patient { Id = 1, Name = "Демонстраційний пацієнт" });
    }
}
