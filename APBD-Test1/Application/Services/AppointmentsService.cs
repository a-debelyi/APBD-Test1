using APBD_8.Exceptions;
using APBD_Test1.Core.Models.DTOs.Request;
using APBD_Test1.Core.Models.DTOs.Response;
using APBD_Test1.Infrastracture.Repositories;

namespace APBD_Test1.Application.Services;

public class AppointmentsService : IAppointmentsService
{
    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly IPatientsRepository _patientsRepository;
    private readonly IDoctorsRepository _doctorsRepository;

    public AppointmentsService(IAppointmentsRepository appointmentsRepository, IPatientsRepository patientsRepository,
        IDoctorsRepository doctorsRepository)
    {
        _appointmentsRepository = appointmentsRepository;
        _patientsRepository = patientsRepository;
        _doctorsRepository = doctorsRepository;
    }

    public async Task<AppointmentResponseDto> GetAppointmentAsync(int id)
    {
        if (!await _appointmentsRepository.ExistsAppointmentAsync(id))
            throw new NotFoundException($"Appointment with id {id} does not exist");
        return await _appointmentsRepository.GetAppointmentAsync(id);
    }

    public async Task<int> CreateAppointmentAsync(AppointmentRequestDto appointment)
    {
        if (await _appointmentsRepository.ExistsAppointmentAsync(appointment.AppointmentId))
            throw new ConflictException($"Appointment with id {appointment.AppointmentId} already exists");
        if (!await _patientsRepository.ExistsPatientAsync(appointment.PatientId))
            throw new NotFoundException($"Patient with id {appointment.PatientId} does not exist");

        int? doctorId = await _doctorsRepository.GetDoctorIdAsync(appointment.Pwz);
        if (doctorId is null)
            throw new NotFoundException($"Doctor with pwz {appointment.Pwz} does not exist");
        return await _appointmentsRepository.CreateAppointmentAsync(appointment, doctorId.Value);
    }
}