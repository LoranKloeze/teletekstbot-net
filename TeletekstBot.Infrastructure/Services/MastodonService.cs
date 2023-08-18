using Mastonet;
using Mastonet.Entities;
using Microsoft.Extensions.Logging;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.Domain.Entities;
using TeletekstBot.Infrastructure.Extensions;

namespace TeletekstBot.Infrastructure.Services;

public class MastodonService : IMastodonService
{
    private readonly ILogger<MastodonService> _logger;
    private readonly IMastodonClient _mastodonClient;
    public MastodonService(ILogger<MastodonService> logger, IMastodonClient mastodonClient)
    {
        _logger = logger;
        _mastodonClient = mastodonClient;
    }
    public async Task Post(Page page, Stream screenshotStream)
    {
        _logger.LogInformation("[Mastodon] Posting page {PageNr} with title '{Title}'", page.PageNumber, page.Title);

        var attachment = await _mastodonClient.UploadMedia(screenshotStream, "screenshot.jpg", page.Body.AddSpacesWhenApplicable(), new AttachmentFocusData
        {
            X = 0.0,
            Y = 1.0
        });
        
        var url = $"https://nos.nl/teletekst#{page.PageNumber}";
        var body = $"[{page.PageNumber}] {page.Title}\n{url}";
        await _mastodonClient.PublishStatus(body, Visibility.Unlisted, null, new List<string> { attachment.Id });
    }
}