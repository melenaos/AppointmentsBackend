using Appointments.Dal.Repositories;
using Appointments.Dal.Repositories.Abstructions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointments.Dal
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDal(this IServiceCollection services)
        {
            services.AddSingleton<IAppointmentRepository, MemoryAppointmentRepository>();

           return services;
        }
    }
}
