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
    private IFetchPageDetailsFromNos _fetchPageDetailsFromNos = null!;
    private IHostEnvironment _env = null!;
    private IMediator _mediator = null!;
    private IFetchPagesFromNos _fetchPagesFromNos = null!;


    [SetUp]
    public void SetUp()
    {
        _pageStore = Substitute.For<IPageStore>();
        _logger = Substitute.For<ILogger<TheBot>>();
        _fetchPageDetailsFromNos = Substitute.For<IFetchPageDetailsFromNos>();
        _fetchPagesFromNos = Substitute.For<IFetchPagesFromNos>();
        _env = Substitute.For<IHostEnvironment>();
        _mediator = Substitute.For<IMediator>();
        
        _theBot = new TheBot(_mediator, _pageStore, _logger, _fetchPageDetailsFromNos, _fetchPagesFromNos, _env);
    }

    [Test]
    public async Task Run_ShouldPublishNewPages()
    {
        // Arrange
        _env.EnvironmentName.Returns("Production");
        var page = new Page { Title = "SampleTitle", Body = "Body of page"};
        const string screenshotPath = "@X:/fake_screenshot.jpg";
        _fetchPageDetailsFromNos.GetPageAndScreenshot(104).Returns((screenshotPath, page));
        _fetchPageDetailsFromNos.GetPageAndScreenshot(105).Returns((screenshotPath, page));
        _fetchPageDetailsFromNos.GetPageAndScreenshot(106).Returns((screenshotPath, page));
        _pageStore.TitlePageNrExist("SampleTitle", 104).Returns(false);
        var page104 = new Page { PageNumber = 104, Title = "SampleTitle" };
        var page105 = new Page { PageNumber = 105, Title = "SampleTitle" };
        var page106 = new Page { PageNumber = 106, Title = "SampleTitle" };
        _fetchPagesFromNos.GetPages().Returns(new List<Page> { page104, page105, page106 });

        const int delayBetweenPageFetching = 0;

        // Act
        await _theBot.Run(delayBetweenPageFetching, false, CancellationToken.None);

        // Assert
        await _mediator.Received(2)
            .Publish(Arg.Is<NewPageEvent>(e => e.Page == page105 || e.Page == page106 && e.ScreenshotPath == screenshotPath),
                CancellationToken.None);
    }

    [Test]
    public async Task Run_ShouldSkipExistingPages()
    {
        // Arrange
        _env.EnvironmentName.Returns("Production");
        var page = new Page { Title = "SampleTitle" };
        const string screenshotPath = "@X:/fake_screenshot.jpg";
        _fetchPageDetailsFromNos.GetPageAndScreenshot(104).Returns((screenshotPath, page));
        _pageStore.TitlePageNrExist("SampleTitle", 104).Returns(true);
        _fetchPagesFromNos.GetPages().Returns(new List<Page>());

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
        _fetchPageDetailsFromNos.GetPageAndScreenshot(104).Returns((screenshotPath, null!));
        _fetchPagesFromNos.GetPages().Returns(new List<Page>());

        const int delayBetweenPageFetching = 0;

        // Act
        await _theBot.Run(delayBetweenPageFetching, false, CancellationToken.None);

        // Assert
        await _mediator.DidNotReceive().Publish(Arg.Any<NewPageEvent>(), CancellationToken.None);
    }
}