using MedicalPortal.Domain;
namespace MedicalPortal.Application;
public interface IPortalService
{
    Task<Appointment> BookAsync(int doctorId, int patientId, DateTime startsAtUtc, bool online);
    Task<MedicalRecord> AddRecordAsync(int appointmentId, string notes);
    Task<List<MedicalRecord>> GetHistoryAsync(int patientId);
    Task<TelemedicineSession> CreateSessionAsync(int appointmentId);
}
public class PortalService(
    IRepository<Doctor> doctors, IRepository<Patient> patients,
    IRepository<Appointment> appointments, IRepository<MedicalRecord> records,
    IRepository<TelemedicineSession> sessions) : IPortalService
{
    public async Task<Appointment> BookAsync(int doctorId, int patientId, DateTime startsAtUtc, bool online)
    {
        if (await doctors.GetByIdAsync(doctorId) is null || await patients.GetByIdAsync(patientId) is null)
            throw new KeyNotFoundException("Лікаря або пацієнта не знайдено.");
        if (startsAtUtc.Kind != DateTimeKind.Utc || startsAtUtc <= DateTime.UtcNow ||
            startsAtUtc.Minute % 30 != 0 || startsAtUtc.Second != 0 || startsAtUtc.Ticks % TimeSpan.TicksPerSecond != 0)
            throw new ArgumentException("Вкажіть майбутній час UTC (Z), слот на 00 або 30 хвилин.");
        if ((await appointments.GetAllAsync()).Any(x => x.StartsAtUtc == startsAtUtc &&
            (x.DoctorId == doctorId || x.PatientId == patientId)))
            throw new InvalidOperationException("Лікар або пацієнт уже має запис у цей час.");
        var appointment = new Appointment { DoctorId = doctorId, PatientId = patientId, StartsAtUtc = startsAtUtc, IsOnline = online };
        await appointments.AddAsync(appointment);
        return appointment;
    }
    public async Task<MedicalRecord> AddRecordAsync(int appointmentId, string notes)
    {
        if (await appointments.GetByIdAsync(appointmentId) is null)
            throw new KeyNotFoundException("Запис на прийом не знайдено.");
        if (string.IsNullOrWhiteSpace(notes) || notes.Length > 4000)
            throw new ArgumentException("Опис має містити від 1 до 4000 символів.");
        var record = new MedicalRecord { AppointmentId = appointmentId, Notes = notes.Trim() };
        await records.AddAsync(record);
        return record;
    }
    public async Task<List<MedicalRecord>> GetHistoryAsync(int patientId)
    {
        if (await patients.GetByIdAsync(patientId) is null) throw new KeyNotFoundException("Пацієнта не знайдено.");
        var ids = (await appointments.GetAllAsync()).Where(x => x.PatientId == patientId).Select(x => x.Id).ToHashSet();
        return (await records.GetAllAsync()).Where(x => ids.Contains(x.AppointmentId)).ToList();
    }
    public async Task<TelemedicineSession> CreateSessionAsync(int appointmentId)
    {
        var appointment = await appointments.GetByIdAsync(appointmentId) ?? throw new KeyNotFoundException("Прийом не знайдено.");
        if (!appointment.IsOnline) throw new ArgumentException("Це очний прийом.");
        var existing = (await sessions.GetAllAsync()).FirstOrDefault(x => x.AppointmentId == appointmentId);
        if (existing is not null) return existing;
        var session = new TelemedicineSession { AppointmentId = appointmentId, RoomCode = Guid.NewGuid().ToString("N") };
        await sessions.AddAsync(session);
        return session;
    }
}
