using TeletekstBot.Application.Interfaces;

namespace TeletekstBot.BotService;

public class Worker : BackgroundService
{
    private readonly IApp _app;


    public Worker(IApp app )
    {
        _app = app;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var args = Environment.GetCommandLineArgs();
        await _app.Run(args, stoppingToken);
    }
}