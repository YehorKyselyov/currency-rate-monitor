# Currency Rate Monitor

Currency Rate Monitor is a service application that periodically fetches currency rates from the National Bank of Ukraine, processes the data, and stores it in various formats (JSON, CSV, XML). The application supports dynamic configuration updates via control codes and can be hosted on Windows.

## Table of Contents
- [Installation](#installation)
- [Uninstallation](#uninstallation)
- [Configuration](#configuration)
- [Dependencies](#dependencies)
- [Usage](#usage)
- [Troubleshooting](#troubleshooting)

## Installation

### Prerequisites
- .NET SDK 7.0 or later

### Steps

1. Clone the repository:
    ```sh
    git clone https://github.com/YehorKyselyov/currency-rate-monitor.git
    cd currency-rate-monitor
    ```

2. Build the project:
    ```sh
    dotnet build
    ```

3. Publish the project:
    ```sh
    dotnet publish -c Release -o publish
    ```

### Installing as a Windows Service

4. Install the service (Run Command Prompt as Administrator):
    ```sh
    sc create NBURateMonitor binPath= "C:\path\to\publish\NBU Currency Rate Monitor.exe"
    ```

5. Start the service:
    ```sh
    sc start NBURateMonitor
    ```

### Running as a Standalone Application

Alternatively, you can run the service directly without installing it as a Windows Service. 

1. Navigate to the publish directory:
    ```sh
    cd C:\path\to\publish
    ```

2. Run the executable directly:
    ```sh
    NBU Currency Rate Monitor.exe
    ```

This way you will be able to interact with service at runtime as will be stated at [Usage](#usage) section

## Uninstallation

### Steps
1. Stop the service:
    ```sh
    sc stop NBURateMonitor
    ```

2. Delete the service:
    ```sh
    sc delete NBURateMonitor
    ```

3. Remove the application files (optional):
    ```sh
    rd /s /q C:\path\to\publish
    ```

## Configuration

### Configuration File
The configuration file `appsettings.json` contains various settings to customize the behavior of the service.

1. Interval: The interval between each fetch cycle, in milliseconds.
2. CurrencyApiUrl: The URL to fetch currency rates from.
3. OutputFormat: The format of the output file (json, csv, xml).
4. OutputPath: The path where the output file will be saved (without extension).
5. LogToConsole: Boolean indicating whether to log currency rates to the console.
6. Logging: Configuration for logging levels.

#### Example Configuration
```json
{
  "Interval": 2000,
  "CurrencyApiUrl": "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchangenew?xml",
  "OutputFormat": "xml",
  "OutputPath": "currency_rates",
  "LogToConsole": true,
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

## Dependencies

The project uses the following dependencies:

```xml
<ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="7.0.1" />
    <PackageReference Include="Microsoft.Extensions.Hosting.WindowsServices" Version="7.0.1" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
    <PackageReference Include="Serilog" Version="4.0.0" />
    <PackageReference Include="Serilog.Extensions.Hosting" Version="8.0.0" />
    <PackageReference Include="Serilog.Sinks.Console" Version="6.0.0" />
    <PackageReference Include="Serilog.Sinks.File" Version="6.0.0" />
</ItemGroup>
```

- **Microsoft.Extensions.Hosting**: Provides hosting abstractions for running background services.
- **Microsoft.Extensions.Hosting.WindowsServices**: Enables the application to run as a Windows service.
- **Newtonsoft.Json**: Used for JSON serialization and deserialization.
- **Serilog**: A diagnostic logging library.
- **Serilog.Extensions.Hosting**: Integration between Serilog and the generic host.
- **Serilog.Sinks.Console**: Serilog sink to log to the console.
- **Serilog.Sinks.File**: Serilog sink to log to a file.

These dependencies are managed via NuGet and will be restored automatically during the build process.

## Usage

### Control Codes
You can update the configuration dynamically using control codes via the console. The format for control codes is:
```sh
set <key> <value>
```
### Examples
Set the fetch interval to 5000 milliseconds:
```sh
set Interval 5000
```
Set the output format to JSON:
```sh
set OutputFormat json
```
Set the logging to console to false:
```sh
set LogToConsole false
```
### Troubleshooting

#### Common Issues

- **Service Fails to Start**:
  - Ensure that the service is installed with the correct path to the executable.
  - Check the Windows Event Viewer for any error messages.

- **Configuration Changes Not Applied**:
  - Make sure to use the correct format for control codes.
  - Verify that the `appsettings.json` file is correctly formatted and contains valid values.

- **High Memory Usage**:
  - Ensure that the application is built and run using the Release configuration.

#### Tips

- Use `sc query NBURateMonitor` to check the status of the service.
- Use `dotnet build --configuration Release` for optimized performance.
- Regularly check the logs for any errors or warnings to ensure smooth operation.
