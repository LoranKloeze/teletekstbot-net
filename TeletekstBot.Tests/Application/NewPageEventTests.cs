using NSubstitute;
using TeletekstBot.Application.Notifications;
using TeletekstBot.Domain.Entities;

namespace TeletekstBot.Tests.Application;

public class NewPageEventTests
{
    [Test]
    public void Constructor_ShouldAssignPageAndScreenshotPath()
    {
        // Arrange
        var page = Substitute.For<Page>();
        const string screenshotPath = @"X:\screenshot.png";

        // Act
        var newPageEvent = new NewPageEvent(page, screenshotPath);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(newPageEvent.Page, Is.EqualTo(page));
            Assert.That(newPageEvent.ScreenshotPath, Is.EqualTo(screenshotPath));
        });
    }
}