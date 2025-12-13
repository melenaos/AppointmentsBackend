using Appointments.Application.Dtos;
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


        [HttpPost("Ingest")]
        public async Task<ActionResult<AppointmentDto>> CreateNewAppointment(AppointmentDto request)
        {
            var result = await _appointmentService.Create(request);

            if (!result.IsSuccess)
                return BadRequest(new
                {
                    errors = result.Errors.Select(e => new
                    {
                        e.Code,
                        e.Message
                    })
                });

            return CreatedAtAction(
                nameof(GetAppointmentById),
                new { id = result.Value!.Id },
                result.Value);
        }


        [HttpGet]
        public ActionResult<IEnumerable<AppointmentDto>> GetAppointments()
        {
            var appointments = _appointmentService.GetAll();

            return Ok(appointments);
        }
    }
}
