using Mastonet;
using Mastonet.Entities;
using Microsoft.Extensions.Logging;
using NSubstitute;
using TeletekstBot.Domain.Entities;
using TeletekstBot.Infrastructure.Services;
// ReSharper disable StringLiteralTypo

namespace TeletekstBot.Tests.Infrastructure;

public class MastodonServiceTests
{
    [Test] 
    public async Task Post_UploadsScreenshotToMastodon()
    {
        // Arrange
        var (mastodonService, mastodonClient) = CreateMastodonService();
        
        using var screenshotStream = new MemoryStream(new byte[]{1,2,3});
        const string screenshotFilename = "screenshot.jpg";
        var page = new Page
        {
            Title = "My title",
            Body = "Any body",
            PageNumber = 110
        };
        
        // Act
        await mastodonService.Post(page, screenshotStream);
        
        // Assert
        await mastodonClient.Received().UploadMedia(screenshotStream, screenshotFilename, page.Body, 
            Arg.Any<AttachmentFocusData>());


    }

    [Test]
    public async Task Post_PostsToMastodon()
    {
        
        // Arrange
        const string attachmentId = "150";
        var (mastodonService, mastodonClient) = CreateMastodonService(attachmentId);
        
        using var screenshotStream = new MemoryStream(new byte[]{1,2,3});
        var page = new Page
        {
            Title = "My title",
            Body = "Any body",
            PageNumber = 110
        };
        
        var expectedUrl = $"https://nos.nl/teletekst#{page.PageNumber}";
        var expectedBody = $"[{page.PageNumber}] {page.Title}\n{expectedUrl}";
        
        // Act
        await mastodonService.Post(page, screenshotStream);
        
        // Assert
        await mastodonClient.Received()
            .PublishStatus(expectedBody, Visibility.Unlisted, null, 
                Arg.Is<List<string>>(x => x.SequenceEqual(new List<string> { attachmentId })));

    }
    
    private static (MastodonService, IMastodonClient) CreateMastodonService(string attachmentId = "100")
    {
        var logger = Substitute.For<ILogger<MastodonService>>();
        var mastodonClient = Substitute.For<IMastodonClient>();

        var attachment = new Attachment { Id = attachmentId};
        mastodonClient.UploadMedia(default!, default!, default, default).ReturnsForAnyArgs(attachment);
        
        var mastodonService = new MastodonService(logger, mastodonClient);
        return (mastodonService, mastodonClient);
    }
}