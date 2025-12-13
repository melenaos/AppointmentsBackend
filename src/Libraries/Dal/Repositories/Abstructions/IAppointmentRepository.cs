using Appointments.Dal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointments.Dal.Repositories.Abstructions
{
    public interface IAppointmentRepository
    {
        Task<Appointment> CreateNew(Appointment appointment);
        Task<Appointment?> Get(long id);
        Task<IEnumerable<Appointment>> GetAll(/* paging-filters */);
        Task<int> DeleteOlderThan(int days);
    }
}
