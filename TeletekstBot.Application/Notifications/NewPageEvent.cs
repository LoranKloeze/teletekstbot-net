using MediatR;
using TeletekstBot.Domain.Entities;

namespace TeletekstBot.Application.Notifications;

public class NewPageEvent : INotification
{
    public Page Page { get; }
    public string ScreenshotPath { get; }
    public NewPageEvent(Page page, string screenshotPath)
    {
        Page = page;
        ScreenshotPath = screenshotPath;
    }
        
}