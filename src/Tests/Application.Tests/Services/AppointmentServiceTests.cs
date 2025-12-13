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
        public class GetAll : AppointmentServiceTests
        {
            [Fact]
            public async Task ReturnsMappedAppointmentDtos()
            {
                // Arrange
                var appointments = new List<Appointment>
                    {
                        new Appointment
                        {
                            Id = 1,
                            ClientName = "John",
                            AppointmentTime = DateTime.UtcNow.AddDays(1),
                            ServiceDurationMinutes = 30
                        },
                        new Appointment
                        {
                            Id = 2,
                            ClientName = "Jane",
                            AppointmentTime = DateTime.UtcNow.AddDays(2),
                            ServiceDurationMinutes = 60
                        }
                    };

                var appointmentRepository = new Mock<IAppointmentRepository>();
                appointmentRepository
                    .Setup(r => r.GetAll())
                    .ReturnsAsync(appointments);

                var service = ProvideAppointmentService(
                    appointmentRepository: appointmentRepository.Object);

                // Act
                var result = await service.GetAll();

                // Assert
                var resultList = result.ToList();

                Assert.Equal(2, resultList.Count);

                Assert.Equal(1, resultList[0].Id);
                Assert.Equal("John", resultList[0].ClientName);
                Assert.Equal(30, resultList[0].ServiceDurationMinutes);

                Assert.Equal(2, resultList[1].Id);
                Assert.Equal("Jane", resultList[1].ClientName);
                Assert.Equal(60, resultList[1].ServiceDurationMinutes);
            }

            [Fact]
            public async Task WhenNoAppointments_ReturnsEmptyCollection()
            {
                var appointmentRepository = new Mock<IAppointmentRepository>();
                appointmentRepository
                    .Setup(r => r.GetAll())
                    .ReturnsAsync(Enumerable.Empty<Appointment>());

                var service = ProvideAppointmentService(
                    appointmentRepository: appointmentRepository.Object);

                var result = await service.GetAll();

                Assert.NotNull(result);
                Assert.Empty(result);
            }
        }

        public class GetById: AppointmentServiceTests
        {
            [Fact]
            public async Task WhenAppointmentExists_ReturnsAppointmentDto()
            {
                // Arrange
                var appointment = new Appointment
                {
                    Id = 5,
                    ClientName = "John",
                    AppointmentTime = DateTime.UtcNow.AddDays(1),
                    ServiceDurationMinutes = 30
                };

                var appointmentRepository = new Mock<IAppointmentRepository>();
                appointmentRepository
                    .Setup(r => r.Get(5))
                    .ReturnsAsync(appointment);

                var service = ProvideAppointmentService(
                    appointmentRepository: appointmentRepository.Object);

                // Act
                var result = await service.GetById(5);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(5, result!.Id);
                Assert.Equal("John", result.ClientName);
                Assert.Equal(30, result.ServiceDurationMinutes);
            }

            [Fact]
            public async Task WhenAppointmentDoesNotExist_ReturnsNull()
            {
                // Arrange
                var appointmentRepository = new Mock<IAppointmentRepository>();
                appointmentRepository
                    .Setup(r => r.Get(It.IsAny<long>()))
                    .ReturnsAsync((Appointment?)null);

                var service = ProvideAppointmentService(
                    appointmentRepository: appointmentRepository.Object);

                // Act
                var result = await service.GetById(42);

                // Assert
                Assert.Null(result);
            }
        }

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

            [Theory]
            [InlineData(15, 0, 0)]   // invalid minute
            [InlineData(45, 0, 0)]   // invalid minute
            [InlineData(0, 5, 0)]    // invalid seconds
            [InlineData(30, 10, 0)]  // invalid seconds
            [InlineData(0, 0, 5)]    // invalid milliseconds
            [InlineData(30, 0, 250)] // invalid milliseconds
            [InlineData(15, 5, 10)]  // everything invalid
            public async Task InvalidTimeAlignment_ReturnsValidationError(
                int minutes,
                int seconds,
                int milliseconds)
            {
                // Arrange
                var appointmentRepository = new Mock<IAppointmentRepository>();
                var service = ProvideAppointmentService(
                    appointmentRepository: appointmentRepository.Object);

                appointmentRepository
                    .Setup(r => r.CreateNew(It.IsAny<Appointment>()))
                    .ReturnsAsync((Appointment a) => a);

                var baseTime = DateTime.UtcNow.Date.AddDays(1).AddHours(10);

                var dto = new AppointmentDto
                {
                    ClientName = "John",
                    AppointmentTime = baseTime
                        .AddMinutes(minutes)
                        .AddSeconds(seconds)
                        .AddMilliseconds(milliseconds)
                };

                // Act
                var result = await service.Create(dto);

                // Assert
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
                    AppointmentTime = DateTime.UtcNow.Date.AddDays(1).AddHours(10).AddMinutes(minutes),
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
