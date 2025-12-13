using Appointments.Application.Dtos;
using Appointments.Application.Services.Abstructions;
using Appointments.Dal.Repositories.Abstructions;
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
        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public Task<AppointmentDto> Create(AppointmentDto appointment)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AppointmentDto>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<AppointmentDto> GetById(long id)
        {
            throw new NotImplementedException();
        }
    }
}
