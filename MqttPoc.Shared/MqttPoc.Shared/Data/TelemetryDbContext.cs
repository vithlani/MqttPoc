using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MqttPoc.Shared.Models;

namespace MqttPoc.Shared.Data
{
    public class TelemetryDbContext : DbContext
    {
        public TelemetryDbContext( DbContextOptions options) : base(options)
        {
        }

        public DbSet<DeviceTelemetryEntity> DeviceTelemetry => Set<DeviceTelemetryEntity>();
    }
}
