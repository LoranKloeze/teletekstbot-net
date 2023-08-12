using Microsoft.Extensions.DependencyInjection;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.BotService;

namespace TeletekstBot.Tests.BotService;

public class ApplicationExtensionTests
{
    [Test]
    public void AddApplicationLayer_AddsCorrectServices()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddApplicationLayer();
 
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
            typeof(IApp),
            typeof(ITheBot),
            typeof(IAdminTool)
        };
    }
}