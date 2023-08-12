using NSubstitute;
using TeletekstBot.Application.Handlers;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.Application.Notifications;
using TeletekstBot.Domain.Entities;

namespace TeletekstBot.Tests.Application;

public class CreateMastodonPostHandlerTests
{
    [Test]
    public async Task Handle_ShouldPostToMastodon()
    {
        // Arrange
        var mastodonService = Substitute.For<IMastodonService>();
        var page = new Page();
        var newPageEvent = new NewPageEvent(page, "");
        
        var createMastodonPostHandler = new CreateMastodonPostHandler(mastodonService);

        // Act
        await createMastodonPostHandler.Handle(newPageEvent, CancellationToken.None);

        // Assert
        await mastodonService.Received().Post(page, Arg.Any<Stream>());
    }
}