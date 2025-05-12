namespace APBD_Test1.Core.Models.DTOs.Response;

public class AppointmentResponseDto
{
    public DateTime Date { get; set; }
    public PatientResponseDto Patient { get; set; }
    public DoctorResponseDto Doctor { get; set; }
    public List<AppointmentServiceResponseDto> AppointmentServices { get; set; } = new();
}