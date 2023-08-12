using NSubstitute;
using TeletekstBot.Application.Handlers;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.Application.Notifications;
using TeletekstBot.Domain.Entities;

namespace TeletekstBot.Tests.Application;

public class CreateBlueSkyPostHandlerTests
{
    [Test]
    public async Task Handle_ShouldPostToBlueSky()
    {
        // Arrange
        var blueSkyService = Substitute.For<IBlueSkyService>();
        const string screenshotPath = @"X:\screenshot.png";
        var page = new Page();
        var newPageEvent = new NewPageEvent(page, screenshotPath);
        
        var createBlueSkyPostHandler = new CreateBlueSkyPostHandler(blueSkyService);

        // Act
        await createBlueSkyPostHandler.Handle(newPageEvent, CancellationToken.None);

        // Assert
        await blueSkyService.Received().Post(page, screenshotPath, CancellationToken.None);
    }
}