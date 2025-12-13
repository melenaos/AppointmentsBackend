using Appointments.Dal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointments.Application.Dtos
{
    public class AppointmentDto
    {
        public long? Id { get; set; }
        public string ClientName { get; set; }
        public DateTime AppointmentTime { get; set; }
        public int? ServiceDurationMinutes { get; set; }

        public AppointmentDto() { }

        public AppointmentDto(Appointment appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment));

            Id = appointment.Id;
            ClientName = appointment.ClientName;
            AppointmentTime = appointment.AppointmentTime;
            ServiceDurationMinutes = appointment.ServiceDurationMinutes;
        }

        public Appointment GetEntity()
            => new Appointment
            {
                Id = null,
                ClientName = ClientName,
                AppointmentTime = AppointmentTime,
                ServiceDurationMinutes = ServiceDurationMinutes ?? throw new Exception("Buisiness layer should have set this") 
            };
    }
}
