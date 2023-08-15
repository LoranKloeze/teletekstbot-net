using MediatR;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.Application.Notifications;

namespace TeletekstBot.Application.Handlers;

// ReSharper disable once UnusedType.Global
public class CreateMastodonPostHandler : INotificationHandler<NewPageEvent>
{
    private readonly IMastodonService _mastodonService;
    private readonly IFileStreamProvider _fileStreamProvider;

    public CreateMastodonPostHandler(IMastodonService mastodonService, IFileStreamProvider fileStreamProvider)
    {
        _mastodonService = mastodonService;
        _fileStreamProvider = fileStreamProvider;
        
    }

    public async Task Handle(NewPageEvent notification, CancellationToken cancellationToken)
    {
        var stream = _fileStreamProvider.CreateStream(notification.ScreenshotPath, FileMode.Open, FileAccess.Read);
        await _mastodonService.Post(notification.Page, stream);
    }
}