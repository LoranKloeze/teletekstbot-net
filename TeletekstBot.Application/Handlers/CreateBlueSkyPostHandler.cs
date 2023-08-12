using MediatR;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.Application.Notifications;

namespace TeletekstBot.Application.Handlers;

// ReSharper disable once UnusedType.Global
public class CreateBlueSkyPostHandler: INotificationHandler<NewPageEvent>
{
    private readonly IBlueSkyService _blueSkyService;

    public CreateBlueSkyPostHandler(IBlueSkyService blueSkyService)
    {
        _blueSkyService = blueSkyService;
    }
    public async Task Handle(NewPageEvent notification, CancellationToken cancellationToken)
    {
        await _blueSkyService.Post(notification.Page, notification.ScreenshotPath, cancellationToken);
    }
}
