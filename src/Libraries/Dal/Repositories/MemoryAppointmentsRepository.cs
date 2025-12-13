using Appointments.Dal.Entities;
using Appointments.Dal.Repositories.Abstructions;
using System.Collections.Concurrent;

namespace Appointments.Dal.Repositories
{
    public class MemoryAppointmentRepository : IAppointmentRepository
    {
        private static readonly ConcurrentDictionary<long, Appointment> _appointments = new();
        private static long _nextId = 0;

        public Task<Appointment> CreateNew(Appointment appointment)
        {
            ArgumentNullException.ThrowIfNull(appointment);

            var id = Interlocked.Increment(ref _nextId);
            appointment.Id = id;

            _appointments.TryAdd(id, appointment);

            return Task.FromResult(appointment);
        }

        public Task<int> DeleteOlderThan(int days)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(days);

            var deleteBefore = DateTime.UtcNow.AddDays(-days);
            int removed = 0;

            // foreach won't break if the collection changes (ConcurrentDictionary)
            foreach (var kvp in _appointments)
            {
                if (kvp.Value.AppointmentTime < deleteBefore &&
                    _appointments.TryRemove(kvp.Key, out _))
                {
                    removed++;
                }
            }

            return Task.FromResult(removed);
        }

        public Task<Appointment?> Get(long id)
        {
            _appointments.TryGetValue(id, out var appointment);
            return Task.FromResult(appointment);
        }

        public Task<IEnumerable<Appointment>> GetAll()
        {
            // Snapshot the List
            return Task.FromResult<IEnumerable<Appointment>>(
                _appointments.Values.ToList()
            );
        }
    }
}
