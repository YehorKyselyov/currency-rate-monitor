using NBU_Currency_Rate_Monitor;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
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