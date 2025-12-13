using Appointments.Application.Services;
using Appointments.Application.Services.Abstructions;
using Appointments.Dal.Repositories;
using Appointments.Dal.Repositories.Abstructions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointments.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddServices();

           return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IAppointmentService, AppointmentService>();
            return services;
        }
    }
}
