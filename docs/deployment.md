# Deployment Guide

## Prerequisites

- .NET 8 SDK installed
- SQL Server instance available
- MQTT Broker (Mosquitto recommended) running on port 1883

## Local Development Deployment

### 1. Database Setup

Create a SQL Server database for telemetry:

```sql
CREATE DATABASE TelemetryPOC;
```

Update the connection string in `src/MqttPoc.Subscriber/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "TelemetryDb": "Server=YOUR_SERVER;Database=TelemetryPOC;Trusted_Connection=true;"
  }
}
```

Or use User Secrets for development:

```powershell
cd src/MqttPoc.Subscriber
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:TelemetryDb" "Server=YOUR_SERVER;Database=TelemetryPOC;Trusted_Connection=true;"
```

### 2. Run Migrations (if EF Core migrations are present)

```powershell
cd src/MqttPoc.Subscriber
dotnet ef database update
```

### 3. Start the Solution

**Option A: Using Visual Studio**
1. Open `MqttPoc.sln`
2. Build the solution
3. Set startup projects:
   - Right-click solution → Properties
   - Select multiple startup projects: API, Publisher, Subscriber

**Option B: Using Command Line**

Terminal 1 - Start API:
```powershell
dotnet run --project src/MqttPoc.API/MqttPoc.API.csproj
```

Terminal 2 - Start Publisher:
```powershell
dotnet run --project src/MqttPoc.Publisher/MqttPoc.Publisher.csproj
```

Terminal 3 - Start Subscriber:
```powershell
dotnet run --project src/MqttPoc.Subscriber/MqttPoc.Subscriber.csproj
```

## Docker Deployment

### 1. Build Docker Image for API

```bash
docker build -f docker/Dockerfile.api -t mqtt-poc-api:latest .
```

### 2. Run with Docker Compose

```bash
docker-compose -f docker/docker-compose.yml up -d
```

This will start:
- MQTT Broker (Mosquitto)
- SQL Server (or provide external instance)
- API service
- Publisher service (optional)
- Subscriber service

### 3. Verify Services

```bash
# Check running containers
docker ps

# View logs
docker-compose -f docker/docker-compose.yml logs -f

# Test API
curl http://localhost:5000/api/telemetry
```

## Production Deployment

### 1. Build Release Packages

```powershell
dotnet publish -c Release -o ./publish/api src/MqttPoc.API/MqttPoc.API.csproj
dotnet publish -c Release -o ./publish/publisher src/MqttPoc.Publisher/MqttPoc.Publisher.csproj
dotnet publish -c Release -o ./publish/subscriber src/MqttPoc.Subscriber/MqttPoc.Subscriber.csproj
```

### 2. Configuration for Production

Update `appsettings.json` files with production settings:
- MQTT broker endpoint (with proper host/port)
- SQL Server connection string (with authentication if required)
- API endpoint configurations
- Logging levels

### 3. Environment Variables

Set required environment variables:
- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__TelemetryDb=<production-connection-string>`
- `MQTT_Broker=<production-mqtt-broker>`

### 4. Run Services

As Windows Services:
```powershell
# Create Windows Service (example for Subscriber)
sc.exe create MqttPocSubscriber binPath="C:\path\to\MqttPoc.Subscriber.exe"
```

As systemd services (Linux):
```bash
# Create systemd service files in /etc/systemd/system/
systemctl start mqtt-poc-subscriber
systemctl enable mqtt-poc-subscriber
```

## Monitoring & Maintenance

### Health Checks
- API: `GET /health` (if implemented)
- Database: Verify SQL Server connectivity
- MQTT: Check broker logs

### Logs Location
- Windows: `%LOCALAPPDATA%\MqttPoc\logs\`
- Linux: `/var/log/mqtt-poc/`
- Docker: `docker logs <container-name>`

### Backup Strategy
- Database: Set up SQL Server maintenance plans
- Configuration: Version control for appsettings files (exclude sensitive data)

## Troubleshooting

### MQTT Connection Issues
- Verify Mosquitto is running on port 1883
- Check firewall rules
- Review subscriber logs for connection errors

### Database Connection Issues
- Verify SQL Server is running
- Check connection string syntax
- Ensure database user has appropriate permissions
- Test connection: `sqlcmd -S SERVER -U USER -P PASSWORD -d TelemetryPOC -Q "SELECT 1"`

### API Not Accessible
- Verify port (default: 5000)
- Check firewall/network policies
- Review ASP.NET Core Kestrel logs

## Performance Tuning

- MQTT: Adjust QoS levels (0 = fire-and-forget, 1 = at-least-once)
- Database: Add appropriate indexes on telemetry tables
- API: Enable response caching where applicable
- Subscriber: Batch database inserts for high-volume scenarios
