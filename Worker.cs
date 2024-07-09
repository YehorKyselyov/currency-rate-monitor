using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NBU_Currency_Rate_Monitor.Serializers;

namespace NBU_Currency_Rate_Monitor;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly HttpClient _httpClient = new();
    private readonly IOptionsMonitor<WorkerOptions> _optionsMonitor;

    public Worker(ILogger<Worker> logger, IOptionsMonitor<WorkerOptions> optionsMonitor)
    {
        _logger = logger;
        _optionsMonitor = optionsMonitor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await FetchAndLogCurrencyRates();
            await Task.Delay(_optionsMonitor.CurrentValue.Interval, stoppingToken);
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Service is starting...");
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Service is stopping.");
        return base.StopAsync(cancellationToken);
    }

    private async Task FetchAndLogCurrencyRates()
    {
        string? response = null;
        try
        {
            response = await _httpClient.GetStringAsync(_optionsMonitor.CurrentValue.CurrencyApiUrl);
        }
        catch (HttpRequestException httpRequestException)
        {
            _logger.LogError(httpRequestException, "Network error while fetching currency rates");
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching currency rates");
            return;
        }

        if (response == null)
        {
            _logger.LogError("Received an empty response from the currency rates API");
            return;
        }

        CurrenciesData currenciesData;
        try
        {
            currenciesData = new CurrenciesData(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing currency rates response");
            return;
        }

        foreach (var currencyData in currenciesData)
            _logger.LogInformation("Currency Data: Code = {Code}, Rate = {Rate}, Date = {Date}, Time = {Time}",
                currencyData.Code, currencyData.Rate, currencyData.Date, currencyData.Time);

        // Create DataIO dynamically with current options
        try
        {
            var currentOptions = _optionsMonitor.CurrentValue;
            IDataSerializer dataSerializer = currentOptions.OutputFormat.ToLower() switch
            {
                "json" => new JsonDataSerializer(),
                "csv" => new CsvDataSerializer(),
                "xml" => new XmlDataSerializer(),
                _ => throw new InvalidOperationException("Unsupported output format")
            };

            var dataIO = new DataIO(dataSerializer, currentOptions.OutputPath);
            await dataIO.SaveDataAsync(currenciesData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving currency rates data");
        }
    }
}

public class WorkerOptions
{
    public int Interval { get; set; }
    public string CurrencyApiUrl { get; set; }
    public string OutputFormat { get; set; }
    public string OutputPath { get; set; }
}