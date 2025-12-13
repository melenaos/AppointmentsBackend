using Appointments.Application.Dtos;
using Appointments.Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointments.Application.Services.Abstructions
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDto>> GetAll();
        Task<AppointmentDto?> GetById(long id);
        Task<Result<AppointmentDto>> Create(AppointmentDto appointment);
    }
}
