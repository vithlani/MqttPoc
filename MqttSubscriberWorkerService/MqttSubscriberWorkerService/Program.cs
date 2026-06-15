using Microsoft.EntityFrameworkCore;
using MqttPoc.Shared.Data;
using MqttSubscriberWorkerService.Services;
using MqttSubscriberWorkerService.Workers;
using MqttPoc.Repositories;


var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<TelemetryDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TelemetryDb"));

});
builder.Services.AddScoped<ITelmetryRepository, TelemetryRepository>();
builder.Services.AddScoped<IDeviceTelemetryRepository, DeviceTelemetryRepository>();
builder.Services.AddScoped<TelemetryProcessor>();
builder.Services.AddHostedService<MqttWorker>();

var host = builder.Build();
host.Run();
