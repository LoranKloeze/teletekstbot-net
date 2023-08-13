using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.Application.Notifications;
using TeletekstBot.Application.Services;
using TeletekstBot.Domain.Entities;

namespace TeletekstBot.Tests.Application;

public class TheBotTests
{
    private ITheBot _theBot = null!;
    private IPageStore _pageStore = null!;
    private ILogger<TheBot> _logger = null!;
    private IFetchScreenshotFromNos _fetchScreenshotFromNos = null!;
    private IHostEnvironment _env = null!;
    private IMediator _mediator = null!;
    

    [SetUp]
    public void SetUp()
    {
        _pageStore = Substitute.For<IPageStore>();
        _logger = Substitute.For<ILogger<TheBot>>();
        _fetchScreenshotFromNos = Substitute.For<IFetchScreenshotFromNos>();
        _env = Substitute.For<IHostEnvironment>();
        _mediator = Substitute.For<IMediator>();

        _theBot = new TheBot(_mediator, _pageStore, _logger, _fetchScreenshotFromNos, _env);
    }

    [Test]
    public async Task Run_ShouldPublishNewPages()
    {
        // Arrange
        _env.EnvironmentName.Returns("Production");
        var page = new Page { Title = "SampleTitle" };
        const string screenshotPath = "@X:/fake_screenshot.jpg";
        _fetchScreenshotFromNos.GetPageAndScreenshot(104).Returns((screenshotPath, page));
        _fetchScreenshotFromNos.GetPageAndScreenshot(105).Returns((screenshotPath, page));
        _pageStore.TitlePageNrExist("SampleTitle", 104).Returns(false);

        const int delayBetweenPageFetching = 0;

        // Act
        await _theBot.Run(delayBetweenPageFetching, false, CancellationToken.None);

        // Assert
        await _mediator.Received(2)
            .Publish(Arg.Is<NewPageEvent>(e => e.Page == page && e.ScreenshotPath == screenshotPath),
                CancellationToken.None);
    }

    [Test]
    public async Task Run_ShouldSkipExistingPages()
    {
        // Arrange
        _env.EnvironmentName.Returns("Production");
        var page = new Page { Title = "SampleTitle" };
        const string screenshotPath = "@X:/fake_screenshot.jpg";
        _fetchScreenshotFromNos.GetPageAndScreenshot(104).Returns((screenshotPath, page));
        _pageStore.TitlePageNrExist("SampleTitle", 104).Returns(true);

        const int delayBetweenPageFetching = 0;

        // Act
        await _theBot.Run(delayBetweenPageFetching, false, CancellationToken.None);

        // Assert
        await _mediator.DidNotReceive().Publish(Arg.Any<NewPageEvent>(), CancellationToken.None);
    }

    [Test]
    public async Task Run_ShouldHandleNullPages()
    {
        // Arrange
        _env.EnvironmentName.Returns("Production");
        const string screenshotPath = "@X:/fake_screenshot.jpg";
        _fetchScreenshotFromNos.GetPageAndScreenshot(104).Returns((screenshotPath, (Page)null!));

        const int delayBetweenPageFetching = 0;

        // Act
        await _theBot.Run(delayBetweenPageFetching, false, CancellationToken.None);

        // Assert
        await _mediator.DidNotReceive().Publish(Arg.Any<NewPageEvent>(), CancellationToken.None);
    }
}