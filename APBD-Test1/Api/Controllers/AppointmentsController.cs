using APBD_Test1.Application.Services;
using APBD_Test1.Core.Models.DTOs.Request;
using Microsoft.AspNetCore.Mvc;

namespace APBD_Test1.Api.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentsService _appointmentsService;

        public AppointmentsController(IAppointmentsService appointmentsService)
        {
            _appointmentsService = appointmentsService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentAsync(int id)
        {
            var appointment = await _appointmentsService.GetAppointmentAsync(id);
            return Ok(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointmentAsync(AppointmentRequestDto appointment)
        {
            var newAppointmentId = await _appointmentsService.CreateAppointmentAsync(appointment);
            return StatusCode(StatusCodes.Status201Created, new { id = newAppointmentId });
        }
    }
}