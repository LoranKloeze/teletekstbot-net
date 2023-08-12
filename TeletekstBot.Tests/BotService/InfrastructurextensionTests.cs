using Bluesky.Net;
using FishyFlip;
using HtmlAgilityPack;
using Mastonet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using StackExchange.Redis;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.BotService;
using TeletekstBot.Infrastructure.Interfaces;

namespace TeletekstBot.Tests.BotService;

public class InfrastructureExtensionTests
{
    [Test]
    public void AddInfrastructureLayer_AddsCorrectServices()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        IDictionary<object, object> properties = new Dictionary<object, object>();
        var hostContext = Substitute.For<HostBuilderContext>(properties);

        // Act
        serviceCollection.AddInfrastructureLayer(hostContext);
 
        // Assert
        ExpectedServices().ForEach(type =>
        {
            var matches = serviceCollection.Any(s => s.ServiceType == type);
            Assert.That(matches, Is.True, $"Expected service '{type}' to be registered when it was not");
        });
    }

    private static List<Type> ExpectedServices()
    {
        return new List<Type>
        {
            typeof(IHttpClientFactory),
            typeof(IBlueskyApi),
            typeof(IConnectionMultiplexer),
            typeof(IParseIncomingPage),
            typeof(IMastodonClient),
            typeof(IMastodonService),
            typeof(IBlueSkyService),
            typeof(IPageStore),
            typeof(IBrowserFactory),
            typeof(IFetchPageFromNos),
            typeof(IFetchScreenshotFromNos),
            typeof(HtmlDocument),
            typeof(ATProtocol)
            
        };
    }
}