using NSubstitute;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.BotService;

namespace TeletekstBot.Tests.BotService;

public class WorkerTests
{
    [Test]
    public void ExecuteAsync_StartsApp()
    {
        // Arrange
        var app = Substitute.For<IApp>();
        var worker = new Worker(app);
        
        // Act
        worker.StartAsync(CancellationToken.None);
        
        // Assert
        app.Received().Run(Arg.Any<string[]>(), Arg.Any<CancellationToken>());
    }
}