using APBD_Test1.Core.Models.DTOs.Request;
using APBD_Test1.Core.Models.DTOs.Response;

namespace APBD_Test1.Application.Services;

public interface IAppointmentsService
{
    Task<AppointmentResponseDto> GetAppointmentAsync(int id);
    Task<int> CreateAppointmentAsync(AppointmentRequestDto appointment);
}