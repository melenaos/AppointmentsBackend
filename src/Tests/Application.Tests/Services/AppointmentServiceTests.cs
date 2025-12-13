using Appointments.Application.Dtos;
using Appointments.Application.Services;
using Appointments.Dal.Entities;
using Appointments.Dal.Repositories.Abstructions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests.Services
{
    public class AppointmentServiceTests
    {
        
        public class Create : AppointmentServiceTests
        {
            [Fact]
            public async Task NullAppointment_ThrowsArgumentNullException()
            {
               var service = ProvideAppointmentService();

                await Assert.ThrowsAsync<ArgumentNullException>(() =>
                    service.Create(null!)
                );
            }

            [Fact]
            public async Task MissingClientName_ReturnsValidationError()
            {
                var service = ProvideAppointmentService();
              
                var dto = new AppointmentDto
                {
                    ClientName = "",
                    AppointmentTime = DateTime.UtcNow.Date.AddDays(1).AddHours(10),
                    ServiceDurationMinutes = 30
                };

                var result = await service.Create(dto);

                Assert.False(result.IsSuccess);
                Assert.Contains(result.Errors, e =>
                    e.Code == "AppointmentCreate.ClientName.Missing" &&
                    !string.IsNullOrWhiteSpace(e.Message));
            }

            [Fact]
            public async Task AppointmentInPast_ReturnsValidationError()
            {
                var service = ProvideAppointmentService();

                var dto = new AppointmentDto
                {
                    ClientName = "John",
                    AppointmentTime = DateTime.UtcNow.Date.AddDays(-1),
                    ServiceDurationMinutes = 30
                };

                var result = await service.Create(dto);

                Assert.False(result.IsSuccess);
                Assert.Contains(result.Errors, e =>
                    e.Code == "AppointmentCreate.AppointmentTime.Past" &&
                    !string.IsNullOrWhiteSpace(e.Message));
            }

            [Fact]
            public async Task InvalidDuration_ReturnsValidationError()
            {
                var appointmentRepository = new Mock<IAppointmentRepository>();
                var service = ProvideAppointmentService(appointmentRepository: appointmentRepository.Object);

                appointmentRepository
                    .Setup(r => r.CreateNew(It.IsAny<Appointment>()))
                    .ReturnsAsync((Appointment a) => a);

                var dto = new AppointmentDto
                {
                    ClientName = "John",
                    AppointmentTime = DateTime.UtcNow.Date.AddDays(1).AddHours(5).AddMinutes(15), // e.g. 1/1/26 1:15:00
                };

                var result = await service.Create(dto);

                Assert.False(result.IsSuccess);
                Assert.Contains(result.Errors, e =>
                    e.Code == "AppointmentCreate.AppointmentTime.Alignment" &&
                    !string.IsNullOrWhiteSpace(e.Message));
            }

            [Theory]
            [InlineData(0)]
            [InlineData(30)]
            public async Task AppointmentTimeAligned_Succeeds(int minutes)
            {
                var appointmentRepository = new Mock<IAppointmentRepository>();
                var service = ProvideAppointmentService(appointmentRepository: appointmentRepository.Object);

                appointmentRepository
                    .Setup(r => r.CreateNew(It.IsAny<Appointment>()))
                    .ReturnsAsync((Appointment a) => a);

                var dto = new AppointmentDto
                {
                    ClientName = "John",
                    AppointmentTime = DateTime.UtcNow.Date.AddHours(10).AddMinutes(minutes),
                    ServiceDurationMinutes = 30
                };

                var result = await service.Create(dto);

                Assert.True(result.IsSuccess);
            }


            [Fact]
            public async Task InvalidAppointmentTimeAlligment_ReturnsValidationError()
            {
                var service = ProvideAppointmentService();

                var dto = new AppointmentDto
                {
                    ClientName = "John",
                    AppointmentTime = DateTime.UtcNow.Date.AddDays(1).AddHours(10),
                    ServiceDurationMinutes = 0
                };

                var result = await service.Create(dto);

                Assert.False(result.IsSuccess);
                Assert.Contains(result.Errors, e =>
                    e.Code == "AppointmentCreate.Duration.Invalid" &&
                    !string.IsNullOrWhiteSpace(e.Message));
            }

            [Fact]
            public async Task MultipleValidationErrors_ReturnsAllErrors()
            {
                var service = ProvideAppointmentService();

                var dto = new AppointmentDto
                {
                    ClientName = "",
                    AppointmentTime = DateTime.UtcNow.Date.AddDays(-1),
                    ServiceDurationMinutes = 0
                };

                var result = await service.Create(dto);

                Assert.False(result.IsSuccess);
                Assert.Equal(3, result.Errors.Count);
            }

            [Fact]
            public async Task MissingDuration_DefaultsTo30Minutes()
            {
                var appointmentRepository = new Mock<IAppointmentRepository>();
                var service = ProvideAppointmentService(appointmentRepository: appointmentRepository.Object);
              
                Appointment? savedEntity = null;

                appointmentRepository
                    .Setup(r => r.CreateNew(It.IsAny<Appointment>()))
                    .Callback<Appointment>(a => savedEntity = a)
                    .ReturnsAsync((Appointment a) => a);

                var dto = new AppointmentDto
                {
                    ClientName = "John",
                    AppointmentTime = DateTime.UtcNow.Date.AddDays(1).AddHours(10),
                    ServiceDurationMinutes = null
                };

                var result = await service.Create(dto);

                Assert.True(result.IsSuccess);
                Assert.NotNull(savedEntity);
                Assert.Equal(30, savedEntity!.ServiceDurationMinutes);
            }

            [Fact]
            public async Task ValidAppointment_ReturnsSuccess()
            {
                var appointmentRepository = new Mock<IAppointmentRepository>();
                var service = ProvideAppointmentService(appointmentRepository: appointmentRepository.Object);

                appointmentRepository
                    .Setup(r => r.CreateNew(It.IsAny<Appointment>()))
                    .ReturnsAsync((Appointment a) => a);

                var dto = new AppointmentDto
                {
                    ClientName = "John",
                    AppointmentTime = DateTime.UtcNow.Date.AddDays(1).AddHours(10),
                    ServiceDurationMinutes = 45
                };

                var result = await service.Create(dto);

                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Value);

                appointmentRepository.Verify(
                    r => r.CreateNew(It.IsAny<Appointment>()),
                    Times.Once);
            }
        }


        protected AppointmentService ProvideAppointmentService(IAppointmentRepository? appointmentRepository = null)
        {
            appointmentRepository = appointmentRepository ?? new Mock<IAppointmentRepository>().Object;

            return new AppointmentService(appointmentRepository);
        }
    }
}
