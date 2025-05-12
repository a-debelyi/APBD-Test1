using System.ComponentModel.DataAnnotations;

namespace APBD_Test1.Core.Models.DTOs.Request;

public class AppointmentServiceRequestDto
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string ServiceName { get; set; }

    [Required, Range(0.0, 1000000000000)] public Decimal ServiceFee { get; set; }
}