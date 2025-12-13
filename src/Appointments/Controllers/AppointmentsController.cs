using Appointments.Application.Dtos;
using Appointments.Application.Services;
using Appointments.Application.Services.Abstructions;
using Microsoft.AspNetCore.Mvc;

namespace Appointments.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(
            IAppointmentService appointmentService,
            ILogger<AppointmentsController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<AppointmentDto>> GetAppointmentById(long id)
        {
            var appointment = _appointmentService.GetById(id);

            if (appointment == null)
                return NotFound();

            return Ok(appointment);
        }


        [HttpPost(Name = "Ingest")]
        public async Task<ActionResult<AppointmentDto>> CreateNewAppointment(AppointmentDto request)
        {
            // Need to validate
            var appointment = await _appointmentService.Create(request);

            return CreatedAtAction(
                nameof(GetAppointmentById),
                new { id = appointment.Id },
                appointment
            );
        }


        [HttpGet]
        public ActionResult<IEnumerable<AppointmentDto>> GetAppointments()
        {
            var appointments = _appointmentService.GetAll();

            return Ok(appointments);
        }
    }
}
