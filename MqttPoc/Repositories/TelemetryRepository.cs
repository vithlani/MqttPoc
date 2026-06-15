using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MqttPoc.Shared.Data;
using MqttPoc.Shared.Models;
namespace MqttPoc.Repositories
{
    public class TelemetryRepository : ITelmetryRepository
    {
        private readonly TelemetryDbContext _context;

        public TelemetryRepository(TelemetryDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(DeviceTelemetry telemetry)
        {
            var entity = new DeviceTelemetryEntity
            {
                DeviceId = telemetry.DeviceId,
                Temperature = telemetry.Temperature,
                Humidity = telemetry.Humidity,
                TimeStampUtc = telemetry.TimeStamp,
                ReceivedAtUtc = DateTime.UtcNow,
            };

            _context.DeviceTelemetry.Add(entity);
            await _context.SaveChangesAsync();
        }
    }
}
