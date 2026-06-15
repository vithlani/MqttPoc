using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttPoc.Shared.Models
{
    public class DeviceTelemetryEntity
    {
        public int Id { get; set; }

        public string DeviceId { get; set; } = string.Empty;

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public DateTime TimeStampUtc {  get; set; }

        public DateTime ReceivedAtUtc { get; set; }

    }
}
