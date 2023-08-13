using System.Diagnostics;
using Bluesky.Net;
using FishyFlip;
using HtmlAgilityPack;
using Mastonet;
using StackExchange.Redis;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.Infrastructure.Factories;
using TeletekstBot.Infrastructure.Interfaces;
using TeletekstBot.Infrastructure.Services;

namespace TeletekstBot.BotService;

public static class InfrastructureExtension
{
    public static void AddInfrastructureLayer(this IServiceCollection serviceCollection, HostBuilderContext hostContext)
    {
        serviceCollection.AddHttpClient();
        serviceCollection.AddBluesky();
        serviceCollection.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var redisConnection = hostContext.Configuration["Redis:Host"];
            Debug.Assert(redisConnection != null, nameof(redisConnection) + " != null");
            return ConnectionMultiplexer.Connect(redisConnection);
        });
        serviceCollection.AddTransient<IParseIncomingPage, ParseIncomingPage>();
        serviceCollection.AddTransient<IMastodonClient, MastodonClient>(_ =>
        {
            var accessToken = hostContext.Configuration["Mastodon:AccessToken"];
            Debug.Assert(accessToken != null, nameof(accessToken) + " != null");
            return new MastodonClient("mastodon.nl", accessToken);
        });
        serviceCollection.AddTransient<IMastodonService, MastodonService>();
        serviceCollection.AddTransient<IBlueSkyService, BlueSkyService>();
        serviceCollection.AddTransient<IPageStore, PageStore>();
        serviceCollection.AddSingleton<IBrowserFactory, BrowserFactory>();
        serviceCollection.AddTransient<IFetchScreenshotFromNos, FetchScreenshotFromNos>();
        serviceCollection.AddTransient<HtmlDocument>();
        serviceCollection.AddTransient<ATProtocol>(_ => new ATProtocolBuilder().EnableAutoRenewSession(true).Build());
    }
}