using System.Buffers;
using System.Text;
using System.Text.Json;
using MQTTnet;
using MqttPoc.Shared.Models;
using MqttSubscriberWorkerService.Services;

namespace MqttSubscriberWorkerService.Workers
{
    public class MqttWorker : BackgroundService
    {
        private readonly ILogger<MqttWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public MqttWorker(ILogger<MqttWorker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var mqttFactory = new MqttClientFactory();
            var mqttClient = mqttFactory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder().WithTcpServer("localhost", 1883).Build();

            mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                try
                {
                    var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload.ToArray());
                    _logger.LogInformation("Recieved Message : {Payload}", payload);
                    var telemetry = JsonSerializer.Deserialize<DeviceTelemetry>(payload);
                    if (telemetry == null)
                        return;

                    using var scope = _scopeFactory.CreateScope();
                    var processor = scope.ServiceProvider.GetRequiredService<TelemetryProcessor>();

                    await processor.ProcessAsync(telemetry);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error Processing MQTT message");
                }

            };

            await mqttClient.ConnectAsync(options, stoppingToken);

            _logger.LogInformation("Connected to Mosquitto");

            await mqttClient.SubscribeAsync("devices/+/telemetry", cancellationToken: stoppingToken);

            _logger.LogInformation("Subscribed to devices/+/telemetry");
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
