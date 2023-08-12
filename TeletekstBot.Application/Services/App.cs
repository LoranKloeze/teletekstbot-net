using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TeletekstBot.Application.Interfaces;

namespace TeletekstBot.Application.Services;

public class App : IApp
{
    private readonly ILogger<App> _logger;
    private readonly IHost _host;
    private readonly ITheBot _theBot;
    private readonly IAdminTool _adminTool;
    
    public App(ILogger<App> logger, IHost host, ITheBot theBot, IAdminTool adminTool)
    {
        _logger = logger;
        _host = host;
        _theBot = theBot;
        _adminTool = adminTool;

    }

    public async Task Run(string[] args, CancellationToken stoppingToken)
    {
        if (args.Length == 1)
        {
            const int delayBetweenPageFetching = 2000;
            _logger.LogInformation("Starting worker");
            await _theBot.Run(delayBetweenPageFetching, true, stoppingToken);
        }
        else
        {
            _logger.LogInformation("Starting admin tool");
            _adminTool.Run(args);
            await _host.StopAsync(stoppingToken);
        }
    }
}