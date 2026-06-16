# MqttPoc

MQTT proof-of-concept built with .NET 8. The solution includes a publisher that emits sample device telemetry, a subscriber worker service that consumes and processes MQTT messages, a shared library for telemetry data models and database context, and a minimal API project.

## Projects

- `MqttPoc` - Minimal ASP.NET Core API project.
- `MQTTPublisher` - Console app that publishes sample telemetry to an MQTT broker.
- `MqttSubscriberWorkerService` - Worker service that subscribes to telemetry messages and processes them.
- `MqttPoc.Shared` - Shared models and Entity Framework Core database context.

## Prerequisites

- .NET 8 SDK
- MQTT broker running locally on port `1883` such as Mosquitto
- SQL Server connection configured for the subscriber worker service

## Getting Started

Restore and build the solution:

```powershell
dotnet restore MqttPOC.sln
dotnet build MqttPOC.sln
```

Run the API:

```powershell
dotnet run --project MqttPoc\MqttPoc.csproj
```

Run the publisher:

```powershell
dotnet run --project MQTTPublisher\MQTTPublisher\MqttPublisher.csproj
```

Run the subscriber worker:

```powershell
dotnet run --project MqttSubscriberWorkerService\MqttSubscriberWorkerService\MqttSubscriberWorkerService.csproj
```

## Configuration

The publisher currently connects to `localhost:1883` and publishes telemetry to:

```text
devices/device001/telemetry
```

Configure the subscriber database connection string in the worker service app settings or through user secrets/environment variables using the connection name `TelemetryDb`.

## Notes

Generated build artifacts such as `bin/`, `obj/`, Visual Studio metadata, and local user-specific files are excluded by `.gitignore`.
