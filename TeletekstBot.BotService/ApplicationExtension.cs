using TeletekstBot.Application.Interfaces;
using TeletekstBot.Application.Services;

namespace TeletekstBot.BotService;

public static class ApplicationExtension
{
    public static void AddApplicationLayer(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IApp, App>();
        serviceCollection.AddTransient<ITheBot, TheBot>();
        serviceCollection.AddTransient<IAdminTool, AdminTool>();
    }
}