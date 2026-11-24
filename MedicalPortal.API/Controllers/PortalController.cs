using MedicalPortal.Application;
using MedicalPortal.Domain;
using Microsoft.AspNetCore.Mvc;
namespace MedicalPortal.API.Controllers;
public record BookingRequest(int DoctorId, int PatientId, DateTime StartsAtUtc, bool IsOnline);
public record RecordRequest(int AppointmentId, string Notes);
[ApiController, Route("api")]
public class PortalController(IPortalService service, IRepository<Doctor> doctors,
    IRepository<Patient> patients, IRepository<Appointment> appointments) : ControllerBase
{
    [HttpGet("doctors")] public async Task<IActionResult> Doctors() => Ok(await doctors.GetAllAsync());
    [HttpGet("patients")] public async Task<IActionResult> Patients() => Ok(await patients.GetAllAsync());
    [HttpGet("appointments")] public async Task<IActionResult> Appointments() => Ok(await appointments.GetAllAsync());
    [HttpPost("appointments")]
    public async Task<IActionResult> Book(BookingRequest request) =>
        StatusCode(201, await service.BookAsync(request.DoctorId, request.PatientId, request.StartsAtUtc, request.IsOnline));
    [HttpGet("patients/{id:int}/history")]
    public async Task<IActionResult> History(int id) => Ok(await service.GetHistoryAsync(id));
    [HttpPost("records")]
    public async Task<IActionResult> AddRecord(RecordRequest request) =>
        StatusCode(201, await service.AddRecordAsync(request.AppointmentId, request.Notes));
    [HttpPost("appointments/{id:int}/telemedicine")]
    public async Task<IActionResult> Session(int id) => Ok(await service.CreateSessionAsync(id));
}
