using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NBU_Currency_Rate_Monitor.Serializers;

namespace NBU_Currency_Rate_Monitor;

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

    private void LogCurrencyRates(CurrenciesData currenciesData)
    {
        foreach (var currencyData in currenciesData)
            _logger.LogInformation("Currency Data: Code = {Code}, Rate = {Rate}, Date = {Date}, Time = {Time}",
                currencyData.Code, currencyData.Rate, currencyData.Date, currencyData.Time);
    }
}

public class WorkerOptions
{
    public int Interval { get; set; } // in milliseconds
    public string CurrencyApiUrl { get; set; }
    public string OutputFormat { get; set; }
    public string OutputPath { get; set; }
    public bool LogToConsole { get; set; }
}