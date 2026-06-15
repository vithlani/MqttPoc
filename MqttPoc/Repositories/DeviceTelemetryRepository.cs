using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MqttPoc.Shared.Data;
using MqttPoc.Shared.Models;

namespace MqttPoc.Repositories
{
    public class DeviceTelemetryRepository : IDeviceTelemetryRepository
    {
        private readonly TelemetryDbContext _context;

        public DeviceTelemetryRepository(TelemetryDbContext context)
        {
            _context = context;
        }

        public async Task<List<DeviceTelemetryEntity>> GetLatestTelemetryAsync()
        {
            return await _context.DeviceTelemetry.OrderByDescending(t => t.TimeStampUtc).Take(100).ToListAsync();
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
