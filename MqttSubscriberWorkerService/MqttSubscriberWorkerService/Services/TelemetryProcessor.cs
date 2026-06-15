using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MqttPoc.Shared.Models;
using MqttPoc.Repositories;

namespace MqttSubscriberWorkerService.Services
{
    public class TelemetryProcessor
    {
        private readonly ITelmetryRepository _telemetryRepository;

        public TelemetryProcessor(ITelmetryRepository telemetryRepository)
        {
            _telemetryRepository = telemetryRepository;
        }

        public async Task ProcessAsync(DeviceTelemetry deviceTelemetry)
        {
            Console.WriteLine($"Processing {deviceTelemetry.DeviceId}");

            await _telemetryRepository.SaveAsync(deviceTelemetry);
        }
    }
}
