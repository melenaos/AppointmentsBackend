using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointments.Dal.Entities
{
    /// <summary>
    /// The db entity for Appointment, don't use it directly -> use AppointmentDto instead
    /// </summary>
    public class Appointment
    {
        public long? Id { get; set; }
        public required string ClientName { get; set; }
        public DateTime ApointmentTime { get; set; }
        public int ServiceDurationMinutes { get; set; }
    }
}
