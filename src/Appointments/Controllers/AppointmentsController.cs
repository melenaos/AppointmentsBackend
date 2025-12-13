using Appointments.Api.ViewModels;
using Appointments.Application.Common;
using Appointments.Application.Dtos;
using Appointments.Application.Services.Abstructions;
using Microsoft.AspNetCore.Mvc;

namespace Appointments.Controllers
{
    /// <summary>
    /// Manages appointments.
    /// </summary>
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


        /// <summary>
        /// Retrieves an appointment by its unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the appointment.
        /// </param>
        /// <response code="200">The appointment was found and returned successfully.</response>
        /// <response code="404">No appointment exists with the specified identifier.</response>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppointmentDto>> GetAppointmentById(long id)
        {
            var appointment = _appointmentService.GetById(id);

            if (appointment == null)
                return NotFound();

            return Ok(appointment);
        }


        /// <summary>
        /// Creates a new appointment.
        /// </summary>
        /// <remarks>
        /// Validates business rules such as ClientName been required
        /// and appointment time constraints.
        /// </remarks>
        /// <param name="request">Appointment data to be created.</param>
        /// <response code="201">Appointment created successfully.</response>
        /// <response code="400">Validation errors occurred.</response>
        [HttpPost("Ingest")]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(IEnumerable<ValidationError>), StatusCodes.Status400BadRequest)]
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

            var response = new AppointmentCreatedResponse(
                result.Value!.Id!.Value,
                "Appointment created successfully."
            );

            return CreatedAtAction(
                nameof(GetAppointmentById),
                new { id = response.Id },
                response);
        }


        /// <summary>
        /// Retrieves all appointments.
        /// </summary>
        /// <remarks>
        /// Returns the complete list of appointments currently stored in the system.
        /// </remarks>
        /// <response code="200">The list of appointments was retrieved successfully.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AppointmentDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<AppointmentDto>> GetAppointments()
        {
            var appointments = _appointmentService.GetAll();

            return Ok(appointments);
        }
    }
}
