using System.ComponentModel.DataAnnotations;

namespace APBD_Test1.Core.Models.DTOs.Request;

public class AppointmentRequestDto
{
    [Required, Range(1, Int32.MaxValue)] public int AppointmentId { get; set; }
    [Required, Range(1, Int32.MaxValue)] public int PatientId { get; set; }

    [Required, StringLength(7, MinimumLength = 2)]
    public string Pwz { get; set; }

    [Required] public List<AppointmentServiceRequestDto> Services { get; set; } = new();
}