using NBU_Currency_Rate_Monitor;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<WorkerOptions>(context.Configuration);
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();