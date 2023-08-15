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
        var fileStreamProvider = Substitute.For<IFileStreamProvider>();
        var page = new Page();
        var newPageEvent = new NewPageEvent(page, "");
        
        var createMastodonPostHandler = new CreateMastodonPostHandler(mastodonService, fileStreamProvider);
        var mockData = new byte[] { 0x01, 0x02, 0x03 }; // mock data for the stream
        var expectedStream = new MemoryStream(mockData);
        fileStreamProvider.CreateStream(newPageEvent.ScreenshotPath, FileMode.Open, FileAccess.Read)
            .Returns(expectedStream);
        
        // Act
        await createMastodonPostHandler.Handle(newPageEvent, CancellationToken.None);

        // Assert
        await mastodonService.Received().Post(page, Arg.Is<MemoryStream>(ms => ms.Length == 3));
    }
}