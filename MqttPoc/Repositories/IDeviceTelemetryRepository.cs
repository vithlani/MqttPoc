using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MqttPoc.Shared.Models;

namespace MqttPoc.Repositories
{
    public interface IDeviceTelemetryRepository
    {
        Task SaveAsync(DeviceTelemetry telemetry);
        Task<List<DeviceTelemetryEntity>> GetLatestTelemetryAsync();
    }
}
