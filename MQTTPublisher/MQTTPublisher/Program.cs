using System.Text.Json;
using MQTTnet;
using MQTTPublisher;

var mqttFactory = new MqttClientFactory();
var mqttClient = mqttFactory.CreateMqttClient();

var options = new MqttClientOptionsBuilder().WithTcpServer("localhost", 1883).Build();

await mqttClient.ConnectAsync(options);

Console.WriteLine("Connected to Mosquitto");

var random = new Random();

while (true)
{
    var telemetry = new DeviceTelemetry
    {
        DeviceId = "device001",
        Temperature = random.Next(20, 40),
        Humidity = random.Next(40, 80),
        Timestamp = DateTime.UtcNow
    };

    var payload = JsonSerializer.Serialize(telemetry);
    var message = new MqttApplicationMessageBuilder()
        .WithTopic("devices/device001/telemetry")
        .WithPayload(payload).Build();

    await mqttClient.PublishAsync(message);

    Console.WriteLine($"Published : {payload}");

    await Task.Delay(5000);
}