using Microsoft.Extensions.Logging;
using TeletekstBot.Application.Interfaces;

namespace TeletekstBot.Application.Services;

public class AdminTool : IAdminTool
{
    private readonly ILogger<AdminTool> _logger;
    private readonly IPageStore _pageStore;
    public AdminTool(ILogger<AdminTool> logger, IPageStore pageStore)
    {
        _logger = logger;
        _pageStore = pageStore;
    }

    public void Run(string[] cmdArgs)
    {
        _logger.LogInformation("Admin tool started");
        switch (cmdArgs[1])
        {
            case "clear":
                _logger.LogInformation("Clearing all pages");
                _pageStore.ClearAllPages();
                break;
            default:
                Console.WriteLine("Unknown command: " + cmdArgs[1]);
                break;
        }

        
    }
}