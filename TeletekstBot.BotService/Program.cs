using System.Reflection;
using Serilog;
using TeletekstBot.BotService;
using Worker = TeletekstBot.BotService.Worker;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

Log.Logger.Information("Starting TeletekstBot.BotService");

Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureServices((hostContext, services) =>
    {
        // Layers
        services.AddApplicationLayer();
        services.AddInfrastructureLayer(hostContext);

        // General
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            cfg.RegisterServicesFromAssembly(Assembly.Load("TeletekstBot.Application"));
        });
        
        // Worker
        services.AddHostedService<Worker>();
        
    }).Build().Run();
