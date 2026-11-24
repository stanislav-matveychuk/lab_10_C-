namespace MedicalPortal.Domain;
public class Doctor { public int Id { get; set; } public string Name { get; set; } = ""; public string Specialty { get; set; } = ""; }
public class Patient { public int Id { get; set; } public string Name { get; set; } = ""; }
public class Appointment
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public DateTime StartsAtUtc { get; set; }
    public bool IsOnline { get; set; }
}
public class MedicalRecord
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public string Notes { get; set; } = "";
}
public class TelemedicineSession
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public string RoomCode { get; set; } = "";
}
