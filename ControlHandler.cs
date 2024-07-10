using Newtonsoft.Json.Linq;

namespace NBU_Currency_Rate_Monitor;

public class ControlHandler
{
    private readonly ILogger<ControlHandler> _logger;
    private readonly string _settingsFilePath = "appsettings.json";
    
    private readonly HashSet<string> validKeys = new HashSet<string>
    {
        "Interval",
        "OutputFormat",
        "OutputPath",
        "LogToConsole"
    };

    private readonly HashSet<string> validFormats = new HashSet<string>
    {
        "json",
        "csv",
        "xml"
    };

    public ControlHandler(ILogger<ControlHandler> logger)
    {
        _logger = logger;
    }

    public void StartListening()
    {
        Task.Run(() =>
        {
            while (true)
            {
                var input = Console.ReadLine();
                if (input.StartsWith("set "))
                {
                    var parts = input.Split(' ');
                    if (parts.Length != 3) continue;
                    var key = parts[1];
                    var value = parts[2];
                    if (ValidateInput(key, value))
                        UpdateAppSettings(key, value);
                }
                else
                {
                    _logger.LogWarning("Invalid command format. Use: set <key> <value>");
                }
            }
        });
    }
    
    private bool ValidateInput(string key, string value)
    {
        if (!validKeys.Contains(key))
        {
            _logger.LogWarning("Invalid key: {key}. Must be one of {validKeys}", key, string.Join(", ", validKeys));
            return false;
        }

        switch (key)
        {
            case "Interval":
                if (!int.TryParse(value, out int interval) || interval <= 0)
                {
                    _logger.LogWarning("Invalid Interval value: {value}. Must be a positive integer.", value);
                    return false;
                }
                break;
            case "OutputFormat":
                if (!validFormats.Contains(value.ToLower()))
                {
                    _logger.LogWarning("Invalid OutputFormat value: {value}. Must be one of {validFormats}.", value, string.Join(", ", validFormats));
                    return false;
                }
                break;

            case "OutputPath":
                if (string.IsNullOrWhiteSpace(value))
                {
                    _logger.LogWarning("Invalid OutputPath value: {value}. Must be a valid file path.", value);
                    return false;
                }
                break;

            case "LogToConsole":
                if (!bool.TryParse(value, out bool log))
                {
                    _logger.LogWarning("Invalid LogToConsole value: {value}. Must be a boolean.", value);
                    return false;
                }
                break;
        }

        return true;
    }

    private void UpdateAppSettings(string key, string value)
    {
        var json = File.ReadAllText(_settingsFilePath);
        var jObject = JObject.Parse(json);
        var section = jObject;

        if (key.Contains(":"))
        {
            var keys = key.Split(':');
            for (int i = 0; i < keys.Length - 1; i++)
            {
                section = (JObject)section[keys[i]];
            }
            key = keys[^1];
        }

        var typedValue = ConvertValueToExpectedType(key, value);
        section[key] = typedValue;
        
        File.WriteAllText(_settingsFilePath, jObject.ToString());
        _logger.LogInformation("Updated {key} to {value} in appsettings.json", key, value);
    }

    private JToken ConvertValueToExpectedType(string key, string value)
    {
        JToken typedValue = value;
        switch (key)
        {
            case "Interval":
                typedValue = int.Parse(value);
                break;

            case "LogToConsole":
                typedValue = bool.Parse(value);
                break;
        }
        return typedValue;
    }
}