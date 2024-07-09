using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace NBU_Currency_Rate_Monitor;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly HttpClient _httpClient = new();
    private readonly string _outputFormat = "xml";
    private readonly WorkerOptions _options;

    public Worker(ILogger<Worker> logger, IOptions<WorkerOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await FetchAndLogCurrencyRates();
            await Task.Delay(_options.Interval, stoppingToken);
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
            response = await _httpClient.GetStringAsync(_options.CurrencyApiUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching currency rates");
            return;
        }
        
        var currenciesData = new CurrenciesData(response);
                
        foreach (var currencyData in currenciesData)
        {
            _logger.LogInformation("Currency Data: Code = {Code}, Rate = {Rate}, Date = {Date}, Time = {Time}",
                currencyData.Code, currencyData.Rate, currencyData.Date, currencyData.Time);
        }
    }
}

public class WorkerOptions
{
    public int Interval { get; set; }
    public string CurrencyApiUrl { get; set; }
}