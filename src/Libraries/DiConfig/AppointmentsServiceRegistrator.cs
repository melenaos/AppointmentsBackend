using Appointments.Application;
using Appointments.Dal;
using Microsoft.Extensions.DependencyInjection;

namespace DiConfig
{
    public static class AppointmentsServiceRegistrator
    {
        public static IServiceCollection AddAppointmentsServices(this IServiceCollection services)
        {
            services
                .AddDal()
                .AddApplication();

            return services;
        }
    }
}
 