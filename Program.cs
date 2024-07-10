using NBU_Currency_Rate_Monitor;
using Serilog;

var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(Path.Combine(exePath, "CRM_log.txt"))
    .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .UseSerilog()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(exePath);
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<WorkerOptions>(context.Configuration);
        services.AddSingleton<ControlHandler>();
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();