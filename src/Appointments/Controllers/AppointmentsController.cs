using Microsoft.AspNetCore.Mvc;

namespace Appointments.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(ILogger<AppointmentsController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<AppointmentDto>> GetAppointmentById(long id)
        {
            var appointment = _appointmentsService.GetById(id);

            if (appointment == null)
                return NotFound();

            var dto = new AppointmentDto(appointment);

            return Ok(dto);
        }


        [HttpPost(Name = "Ingest")]
        public async Task<ActionResult<AppointmentDto>> CreateNewAppointment()
        {
            throw new Exception("Validate first!");
            var appointment = await _appointmentService.CreateAsync(request);

            var dto = new AppointmentDto(appointment);

            return CreatedAtAction(
                nameof(GetAppointmentById),
                new { id = appointment.Id },
                dto
            );
        }


        [HttpGet]
        public ActionResult<IEnumerable<AppointmentDto>> GetAppointments()
        {
            var appointments = _appointmentsService.GetAll();

            var dtos = appointments.Select(a => new AppointmentDto(a));

            return Ok(dtos);
        }
    }
}
