using Microsoft.Extensions.Logging;
using TeletekstBot.Application.Interfaces;

namespace TeletekstBot.Application.Services;

public class AdminTool : IAdminTool
{
    private readonly ILogger<AdminTool> _logger;
    private readonly IPageStore _pageStore;
    private readonly ITheBot _theBot;
    public AdminTool(ILogger<AdminTool> logger, IPageStore pageStore, ITheBot theBot)
    {
        _logger = logger;
        _pageStore = pageStore;
        _theBot = theBot;
    }

    public async Task Run(string[] cmdArgs, CancellationToken stoppingToken)
    {
        _logger.LogInformation("Admin tool started");
        switch (cmdArgs[1])
        {
            case "cache-clear":
                _logger.LogInformation("Clearing cache");
                _pageStore.ClearAllPages();
                break;
            case "cache-warmup":
                _logger.LogInformation("Warming up cache");
                const int delayBetweenPageFetching = 200;
                const bool runForever = false;
                const bool doPublish = false;
                await _theBot.Run(delayBetweenPageFetching, runForever, doPublish, stoppingToken);
                break;
            default:
                Console.WriteLine("Unknown command: " + cmdArgs[1]);
                break;
        }

        
    }
}