# Setup & Development Guide

## Prerequisites

### System Requirements
- Windows 10/11 or Linux/macOS with .NET support
- .NET 8 SDK (download from https://dotnet.microsoft.com/download)
- Visual Studio 2022 or Visual Studio Code (optional)
- SQL Server 2019+ or SQL Server Express
- MQTT Broker (Mosquitto)

### Required Tools

**Windows:**
```powershell
# Using Chocolatey
choco install dotnet-sdk -y
choco install mosquitto -y
choco install sql-server-express -y
```

**macOS:**
```bash
# Using Homebrew
brew install dotnet
brew install mosquitto
```

**Linux (Ubuntu/Debian):**
```bash
sudo apt-get update
sudo apt-get install dotnet-sdk-8.0
sudo apt-get install mosquitto mosquitto-clients
sudo apt-get install mssql-server
```

## Project Setup

### 1. Clone the Repository

```bash
git clone <repository-url>
cd MqttPoc
```

### 2. Restore Dependencies

```powershell
# Restore all projects
dotnet restore MqttPoc.sln

# Or restore individual projects
dotnet restore src/MqttPoc.API/
dotnet restore src/MqttPoc.Publisher/
dotnet restore src/MqttPoc.Subscriber/
dotnet restore src/MqttPoc.Shared/
```

### 3. Build Solution

```powershell
# Full build
dotnet build MqttPoc.sln

# Release build
dotnet build -c Release MqttPoc.sln
```

### 4. MQTT Broker Setup

#### Windows with Chocolatey
```powershell
choco install mosquitto
mosquitto -v  # Start broker (verbose)
```

#### Docker
```bash
docker run -d --name mosquitto -p 1883:1883 eclipse-mosquitto:latest
```

#### Manual Installation
1. Download from https://mosquitto.org/download/
2. Extract and configure `mosquitto.conf`
3. Run: `mosquitto -c mosquitto.conf`

### 5. Database Setup

#### SQL Server Connection String
Create database and update `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "TelemetryDb": "Server=(LocalDB)\\mssqllocaldb;Database=TelemetryPOC;Integrated Security=true;"
  }
}
```

#### Using LocalDB (Windows)
LocalDB is included with Visual Studio or SQL Server Express. Connection string:
```
Server=(LocalDB)\mssqllocaldb;Database=TelemetryPOC;Integrated Security=true;
```

#### Using SQL Server Express
```
Server=.\\SQLEXPRESS;Database=TelemetryPOC;Integrated Security=true;
```

#### Using Docker
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Your@Password123" \
  -p 1433:1433 --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest

# Connection string
# Server=localhost,1433;Database=TelemetryPOC;User Id=sa;Password=Your@Password123;
```

### 6. Entity Framework Core Migrations

If using EF Core migrations for database schema:

```powershell
# Navigate to Subscriber project
cd src/MqttPoc.Subscriber

# Create migration (if needed)
dotnet ef migrations add InitialCreate

# Apply migrations to database
dotnet ef database update
```

## Running the Application

### Using Visual Studio

1. Open `MqttPoc.sln` in Visual Studio 2022
2. Set startup projects (right-click solution → Properties):
   - ✓ MqttPoc.API
   - ✓ MqttPoc.Publisher (optional)
   - ✓ MqttPoc.Subscriber
3. Press `F5` to run

### Using Visual Studio Code

1. Open the project folder
2. Install recommended extensions: C#, REST Client
3. Create `.vscode/launch.json`:

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (API)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/bin/Debug/net8.0/MqttPoc.API.dll",
      "args": [],
      "cwd": "${workspaceFolder}/src/MqttPoc.API",
      "stopAtEntry": false,
      "serverReadyAction": {
        "pattern": "\\bNow listening on:\\s+(https?://\\S+)",
        "uriFormat": "{0}",
        "action": "openExternally"
      }
    }
  ]
}
```

4. Run: `F5` or `dotnet run --project src/MqttPoc.API`

### Using Command Line

**Terminal 1 - API:**
```powershell
cd src/MqttPoc.API
dotnet run
# API should be available at http://localhost:5000
```

**Terminal 2 - Publisher:**
```powershell
cd src/MqttPoc.Publisher
dotnet run
# Should publish telemetry messages to MQTT broker
```

**Terminal 3 - Subscriber:**
```powershell
cd src/MqttPoc.Subscriber
dotnet run
# Should subscribe and persist telemetry to database
```

## Testing the Application

### 1. Check MQTT Broker

```bash
# Subscribe to telemetry topic (another terminal)
mosquitto_sub -t "devices/+/telemetry"

# Manually publish a test message
mosquitto_pub -t "devices/device001/telemetry" -m '{"temperature":25.5}'
```

### 2. Test API Endpoints

```bash
# Get telemetry
curl http://localhost:5000/api/telemetry

# Or use REST Client extension in VS Code
GET http://localhost:5000/api/telemetry
```

### 3. Check Database

```powershell
# Using SQL Server Management Studio
# Connect to (LocalDB)\mssqllocaldb
# Database: TelemetryPOC
# Query: SELECT * FROM Telemetry

# Or via command line
sqlcmd -S "(LocalDB)\mssqllocaldb" -d TelemetryPOC -Q "SELECT * FROM Telemetry"
```

## Code Organization

```
src/
├── MqttPoc.API/
│   ├── Controllers/        - API endpoints
│   ├── Repositories/       - Data access layer
│   ├── Program.cs         - Startup configuration
│   └── appsettings*.json  - Configuration
├── MqttPoc.Publisher/
│   ├── Services/          - Publishing logic
│   ├── Program.cs         - Entry point
│   └── appsettings*.json  - Configuration
├── MqttPoc.Subscriber/
│   ├── Services/          - Business logic
│   ├── Workers/           - Hosted service implementations
│   ├── Program.cs         - Startup configuration
│   └── appsettings*.json  - Configuration
└── MqttPoc.Shared/
    ├── Data/              - EF Core DbContext
    ├── Models/            - Domain models
    └── MqttPoc.Shared.csproj
```

## Useful Commands

```powershell
# Clean build artifacts
dotnet clean MqttPoc.sln

# Run tests (when available)
dotnet test MqttPoc.sln

# Format code
dotnet format MqttPoc.sln

# List NuGet packages
dotnet package list MqttPoc.sln

# Update package
dotnet package update --project src/MqttPoc.Shared

# Create new project
dotnet new console -n MyNewProject -o src/
```

## Common Issues & Solutions

### Issue: "MQTT connection refused"
**Solution:**
- Verify Mosquitto is running: `netstat -an | find "1883"`
- Check firewall settings
- Ensure correct host/port in appsettings

### Issue: "Database connection failed"
**Solution:**
- Verify SQL Server instance is running
- Check connection string syntax
- Test with SSMS or sqlcmd first
- Ensure LocalDB/Express is installed

### Issue: "Port already in use"
**Solution:**
```powershell
# Find process using port 5000
netstat -ano | find "5000"
# Kill process: taskkill /PID <PID> /F
# Or change port in appsettings
```

### Issue: "Project not loading in Visual Studio"
**Solution:**
- Run: `dotnet restore MqttPoc.sln`
- Close and reopen Visual Studio
- Check .csproj file for syntax errors

## Next Steps

1. **Explore the codebase**: Start with `Program.cs` files
2. **Read documentation**: Check `docs/` folder
3. **Run integration tests** (when available)
4. **Review MQTT topics** and messages
5. **Understand data models** in Shared library
6. **Extend functionality** as needed

## Additional Resources

- [.NET 8 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [ASP.NET Core Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [MQTTnet GitHub](https://github.com/dotnet/MQTTnet)
- [Mosquitto Documentation](https://mosquitto.org/documentation/)
