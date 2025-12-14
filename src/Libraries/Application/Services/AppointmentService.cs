using Appointments.Application.Common;
using Appointments.Application.Dtos;
using Appointments.Application.Results;
using Appointments.Application.Services.Abstructions;
using Appointments.Dal.Entities;
using Appointments.Dal.Repositories.Abstructions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointments.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        ILogger<AppointmentService> _logger;
        public AppointmentService(
            IAppointmentRepository appointmentRepository, 
            ILogger<AppointmentService> logger)
        {
            _appointmentRepository = appointmentRepository;
            _logger = logger;
        }

        public async Task<Result<AppointmentDto>> Create(AppointmentDto appointment)
        {
            ArgumentNullException.ThrowIfNull(appointment);

            // Validate input
            var errors = new List<ValidationError>();

            if (string.IsNullOrEmpty(appointment.ClientName))
                errors.Add(new("AppointmentCreate.ClientName.Missing", "The client name is required."));

            if (appointment.AppointmentTime < DateTime.UtcNow.AddMinutes(5))
                errors.Add(new("AppointmentCreate.AppointmentTime.Past", "Appointment time must be in the future."));

            if ((appointment.AppointmentTime.Minute != 30 && appointment.AppointmentTime.Minute != 0 )||
                appointment.AppointmentTime.Second != 0 || appointment.AppointmentTime.Millisecond != 0)
                errors.Add(new("AppointmentCreate.AppointmentTime.Alignment", "Appointment time must start on the hour or half-hour."));

            if (appointment.ServiceDurationMinutes.HasValue && appointment.ServiceDurationMinutes <= 0)
                errors.Add(new("AppointmentCreate.Duration.Invalid", "Duration must be greater than zero"));

            // Overlapping is allowed?
            // if (await _appointmentRepository.HasOverlap(appointment.AppointmentTime, appointment.ServiceDurationMinutes))
            //    errors.Add(new("AppointmentCreate.AppointmentTime.Overlap", "Overlapping appointment exists"));

            if (errors.Any())
                return Result<AppointmentDto>.Failure(errors.ToArray());

            // assume a default of 30 if missing. [Requirement]
            if (!appointment.ServiceDurationMinutes.HasValue) appointment.ServiceDurationMinutes = 30;

            // Store appointment
            Appointment entity = appointment.GetEntity();
            entity = await _appointmentRepository.CreateNew(entity);

            _logger.LogInformation("New appointment have beed created. Id: {AppointmentId}", entity.Id);

            return Result<AppointmentDto>.Success(new AppointmentDto(entity));
        }

        public async Task<IEnumerable<AppointmentDto>> GetAll()
        {
            var appointments = await _appointmentRepository.GetAll();

            return appointments.Select(a => new AppointmentDto(a));
        }

        public async Task<AppointmentDto?> GetById(long id)
        {
            var appointment = await _appointmentRepository.Get(id);

            if (appointment != null)
                return new AppointmentDto(appointment);
            else
                return null;
        }
    }
}
