using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NBU_Currency_Rate_Monitor.Serializers;

namespace NBU_Currency_Rate_Monitor;

/// <summary>
/// Background service that processes currency rates.
/// </summary>
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly HttpClient _httpClient = new();
    private readonly ControlHandler _controlHandler;
    private readonly IOptionsMonitor<WorkerOptions> _optionsMonitor;
    private WorkerOptions CurrentOptions => _optionsMonitor.CurrentValue; 

    public Worker(ILogger<Worker> logger, IOptionsMonitor<WorkerOptions> optionsMonitor, ControlHandler controlHandler)
    {
        _logger = logger;
        _optionsMonitor = optionsMonitor;
        _controlHandler = controlHandler;
    }

    /// <summary>
    /// The main execution loop of the background service.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await ProcessCurrencyRates();
            await Task.Delay(CurrentOptions.Interval, stoppingToken);
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Service is starting...");
        _controlHandler.StartListening();
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Service is stopping.");
        return base.StopAsync(cancellationToken);
    }

    /// <summary>
    /// Processes currency rates by fetching, parsing, saving, and logging them.
    /// </summary>
    private async Task ProcessCurrencyRates()
    {
        var response = await FetchCurrencyRatesAsync();
        if (response == null) return;

        var currenciesData = ParseCurrencyRates(response);
        if (currenciesData == null) return;

        SaveCurrencyRates(currenciesData);

        if (CurrentOptions.LogToConsole)
            LogCurrencyRates(currenciesData);
    }

    /// <summary>
    /// Fetches the currency rates from the configured API.
    /// </summary>
    /// <returns>The API response as a string, or null if an error occurred.</returns>
    private async Task<string?> FetchCurrencyRatesAsync()
    {
        try
        {
            return await _httpClient.GetStringAsync(CurrentOptions.CurrencyApiUrl);
        }
        catch (HttpRequestException httpRequestException)
        {
            _logger.LogError(httpRequestException, "Network error while fetching currency rates");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching currency rates");
        }

        return null;
    }

    /// <summary>
    /// Parses the currency rates from the API response.
    /// </summary>
    /// <param name="response">The API response as a string.</param>
    /// <returns>A <see cref="CurrenciesData"/> object, or null if parsing failed.</returns>
    private CurrenciesData? ParseCurrencyRates(string response)
    {
        try
        {
            return new CurrenciesData(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing currency rates response");
            return null;
        }
    }

    /// <summary>
    /// Saves the parsed currency rates to the configured output path.
    /// </summary>
    /// <param name="currenciesData">The parsed currency rates.</param>
    private void SaveCurrencyRates(CurrenciesData currenciesData)
    {
        try
        {
            IDataSerializer dataSerializer = CurrentOptions.OutputFormat.ToLower() switch
            {
                "json" => new JsonDataSerializer(),
                "csv" => new CsvDataSerializer(),
                "xml" => new XmlDataSerializer(),
                _ => throw new InvalidOperationException("Unsupported output format")
            };
            var outputPath = $"{CurrentOptions.OutputPath}.{CurrentOptions.OutputFormat.ToLower()}";
            dataSerializer.Serialize(currenciesData, outputPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving currency rates data");
        }
    }
    
    /// <summary>
    /// Logs the parsed currency rates to the console.
    /// </summary>
    /// <param name="currenciesData">The parsed currency rates.</param>
    private void LogCurrencyRates(CurrenciesData currenciesData)
    {
        foreach (var currencyData in currenciesData)
            _logger.LogInformation("Currency Data: Code = {Code}, Rate = {Rate}, Date = {Date}, Time = {Time}",
                currencyData.Code, currencyData.Rate, currencyData.Date, currencyData.Time);
    }
}

/// <summary>
/// Represents the configuration options for the <see cref="Worker"/> service.
/// </summary>
public class WorkerOptions
{
    /// <summary>
    /// The interval between each processing cycle, in milliseconds.
    /// </summary>
    public int Interval { get; set; }

    /// <summary>
    /// The URL of the currency API.
    /// </summary>
    public string CurrencyApiUrl { get; set; }

    /// <summary>
    /// The format of the output file (e.g., "json", "csv", "xml").
    /// </summary>
    public string OutputFormat { get; set; }

    /// <summary>
    /// The path of the output file without extension.
    /// </summary>
    public string OutputPath { get; set; }

    /// <summary>
    /// A value indicating whether to log the currency rates to the console.
    /// </summary>
    public bool LogToConsole { get; set; }
}