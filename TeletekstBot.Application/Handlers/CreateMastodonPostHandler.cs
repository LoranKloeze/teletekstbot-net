using MediatR;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.Application.Notifications;

namespace TeletekstBot.Application.Handlers;

// ReSharper disable once UnusedType.Global
public class CreateMastodonPostHandler : INotificationHandler<NewPageEvent>
{
    private readonly IMastodonService _mastodonService;

    public CreateMastodonPostHandler(IMastodonService mastodonService)
    {
        _mastodonService = mastodonService;
    }   

    public async Task Handle(NewPageEvent notification, CancellationToken cancellationToken)
    {
        Stream stream;
        if (!string.IsNullOrEmpty(notification.ScreenshotPath))
        {
            stream =  new FileStream(notification.ScreenshotPath, FileMode.Open, FileAccess.Read);
        }
        else
        {
            stream = new MemoryStream();
        }
        await _mastodonService.Post(notification.Page, stream );
    }
}