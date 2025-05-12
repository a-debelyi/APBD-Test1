using APBD_Test1.Core.Models.DTOs.Request;
using APBD_Test1.Core.Models.DTOs.Response;

namespace APBD_Test1.Infrastracture.Repositories;

public interface IAppointmentsRepository
{
    Task<bool> ExistsAppointmentAsync(int id);
    Task<AppointmentResponseDto> GetAppointmentAsync(int id);
    Task<int> CreateAppointmentAsync(AppointmentRequestDto appointment, int doctorId);
}