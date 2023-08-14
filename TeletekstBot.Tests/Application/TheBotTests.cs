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
    private IFetchPageFromNos _fetchPageFromNos = null!;
    private IHostEnvironment _env = null!;
    private IMediator _mediator = null!;
    private IFetchPageNumbersFromNos _fetchPageNumbersFromNos = null!;


    [SetUp]
    public void SetUp()
    {
        _pageStore = Substitute.For<IPageStore>();
        _logger = Substitute.For<ILogger<TheBot>>();
        _fetchPageFromNos = Substitute.For<IFetchPageFromNos>();
        _fetchPageNumbersFromNos = Substitute.For<IFetchPageNumbersFromNos>();
        _env = Substitute.For<IHostEnvironment>();
        _mediator = Substitute.For<IMediator>();
        
        _theBot = new TheBot(_mediator, _pageStore, _logger, _fetchPageFromNos, _fetchPageNumbersFromNos, _env);
    }

    [Test]
    public async Task Run_ShouldPublishNewPages()
    {
        // Arrange
        _env.EnvironmentName.Returns("Production");
        var page = new Page { Title = "SampleTitle" };
        const string screenshotPath = "@X:/fake_screenshot.jpg";
        _fetchPageFromNos.GetPageAndScreenshot(104).Returns((screenshotPath, page));
        _fetchPageFromNos.GetPageAndScreenshot(105).Returns((screenshotPath, page));
        _pageStore.TitlePageNrExist("SampleTitle", 104).Returns(false);
        _fetchPageNumbersFromNos.GetNumbers()
            .Returns(new List<int> { 104, 105, 106 });

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
        _fetchPageFromNos.GetPageAndScreenshot(104).Returns((screenshotPath, page));
        _pageStore.TitlePageNrExist("SampleTitle", 104).Returns(true);
        _fetchPageNumbersFromNos.GetNumbers()
            .Returns(new List<int> { 104, 105, 106 });

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
        _fetchPageFromNos.GetPageAndScreenshot(104).Returns((screenshotPath, null!));
        _fetchPageNumbersFromNos.GetNumbers()
            .Returns(new List<int> { 104, 105, 106 });

        const int delayBetweenPageFetching = 0;

        // Act
        await _theBot.Run(delayBetweenPageFetching, false, CancellationToken.None);

        // Assert
        await _mediator.DidNotReceive().Publish(Arg.Any<NewPageEvent>(), CancellationToken.None);
    }
}