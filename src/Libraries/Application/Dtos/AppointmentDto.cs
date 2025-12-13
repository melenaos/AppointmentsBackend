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
        public required string ClientName { get; set; }
        public DateTime ApointmentTime { get; set; }
        public int ServiceDurationMinutes { get; set; }

    }
}
