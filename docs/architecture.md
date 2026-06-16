# MQTT POC - Architecture

## Overview

This is a proof-of-concept MQTT-based telemetry system built with .NET 8. The architecture follows a producer-consumer pattern with a shared data layer.

## System Components

### MqttPoc.API
- **Type**: ASP.NET Core Minimal API
- **Purpose**: Provides HTTP endpoints for querying telemetry data
- **Dependencies**: MqttPoc.Shared
- **Key Features**:
  - RESTful endpoints for accessing stored telemetry
  - Integration with Entity Framework Core for data access

### MqttPoc.Publisher
- **Type**: Console Application
- **Purpose**: Generates and publishes sample device telemetry to MQTT broker
- **Dependencies**: MQTTnet
- **Key Features**:
  - Publishes simulated device telemetry data
  - Connects to MQTT broker on `localhost:1883`
  - Publishes to topic: `devices/device001/telemetry`

### MqttPoc.Subscriber
- **Type**: Worker Service (.NET Hosted Service)
- **Purpose**: Subscribes to MQTT messages and persists telemetry to database
- **Dependencies**: MqttPoc.Shared, MQTTnet, Entity Framework Core
- **Key Features**:
  - Subscribes to telemetry topics
  - Processes incoming MQTT messages
  - Persists data to SQL Server database
  - Runs as a background service

### MqttPoc.Shared
- **Type**: Class Library
- **Purpose**: Shared models, DTOs, and database context
- **Dependencies**: Entity Framework Core, SQL Server provider
- **Key Components**:
  - `TelemetryDbContext` - Entity Framework Core database context
  - Domain models for telemetry data
  - Shared configurations

## Data Flow

```
┌─────────────────┐
│  MqttPoc        │
│  Publisher      │
└────────┬────────┘
         │ Publishes telemetry
         │ (MQTT Topic: devices/device001/telemetry)
         ↓
   ┌──────────────┐
   │  MQTT Broker │
   │ (Mosquitto)  │
   └──────────────┘
         ↑
         │ Subscribes
         │
┌────────┴────────┐
│  MqttPoc        │
│  Subscriber     │
└────────┬────────┘
         │ Persists data
         ↓
    ┌─────────────┐
    │  SQL Server │
    │  Database   │
    └─────────────┘
         ↑
         │ Queries
         │
    ┌─────────────┐
    │  MqttPoc    │
    │  API        │
    └─────────────┘
```

## Technology Stack

- **.NET**: 8.0
- **MQTT Library**: MQTTnet 5.1.0.1559
- **ORM**: Entity Framework Core 9.0.16
- **Database**: SQL Server
- **MQTT Broker**: Mosquitto (recommended for local development)

## Project Structure

```
MqttPoc/
├── src/
│   ├── MqttPoc.API/           - API project
│   ├── MqttPoc.Publisher/     - Publisher console app
│   ├── MqttPoc.Subscriber/    - Subscriber worker service
│   └── MqttPoc.Shared/        - Shared library
├── tests/                      - Test projects (todo)
├── docs/                       - Documentation
├── docker/                     - Docker configuration
├── MqttPoc.sln                - Root solution file
└── README.md                   - Getting started guide
```

## Design Patterns

### Dependency Injection
All projects leverage .NET's built-in dependency injection container for service registration and resolution.

### Repository Pattern
The shared library uses the repository pattern for data access abstraction (in Controllers/Repositories of API project).

### Hosted Services
The Subscriber project uses .NET's `IHostedService` pattern for background processing of telemetry data.

## Configuration

Each project includes `appsettings.json` and `appsettings.Development.json` for environment-specific configuration.

Key configurations:
- **MQTT Broker URL**: Typically `localhost:1883` for development
- **Database Connection String**: Named `TelemetryDb` in connection strings
- **Topic Subscriptions**: Configured in Subscriber worker service

## Future Enhancements

- [ ] Add unit tests for business logic
- [ ] Add integration tests
- [ ] Implement metrics/monitoring
- [ ] Add API authentication
- [ ] Implement message retry logic
- [ ] Add configuration management service
- [ ] Docker Compose setup for complete local environment
