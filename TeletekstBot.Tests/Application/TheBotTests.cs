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
    private IFetchPageFromNos _fetchPageFromNos = null!;
    private IPageStore _pageStore = null!;
    private ILogger<TheBot> _logger = null!;
    private IFetchScreenshotFromNos _fetchScreenshotFromNos = null!;
    private IHostEnvironment _env = null!;
    private IMediator _mediator = null!;
    

    [SetUp]
    public void SetUp()
    {
        _fetchPageFromNos = Substitute.For<IFetchPageFromNos>();
        _pageStore = Substitute.For<IPageStore>();
        _logger = Substitute.For<ILogger<TheBot>>();
        _fetchScreenshotFromNos = Substitute.For<IFetchScreenshotFromNos>();
        _env = Substitute.For<IHostEnvironment>();
        _mediator = Substitute.For<IMediator>();

        _theBot = new TheBot(_mediator, _fetchPageFromNos, _pageStore, _logger, _fetchScreenshotFromNos, _env);
    }

    [Test]
    public async Task Run_ShouldPublishNewPages()
    {
        // Arrange
        _env.EnvironmentName.Returns("Production");
        var page = new Page { Title = "SampleTitle" };
        _fetchPageFromNos.Get(104, Arg.Any<CancellationToken>()).Returns(page);
        _fetchPageFromNos.Get(105, Arg.Any<CancellationToken>()).Returns(page);
        _pageStore.TitlePageNrExist("SampleTitle", 104).Returns(false);
        _fetchScreenshotFromNos.Get(104).Returns("samplePath");
        _fetchScreenshotFromNos.Get(105).Returns("samplePath");

        const int delayBetweenPageFetching = 50;

        // Act
        await _theBot.Run(delayBetweenPageFetching, false, CancellationToken.None);

        // Assert
        await _mediator.Received(2)
            .Publish(Arg.Is<NewPageEvent>(e => e.Page == page && e.ScreenshotPath == "samplePath"),
                CancellationToken.None);
    }

    [Test]
    public async Task Run_ShouldSkipExistingPages()
    {
        // Arrange
        _env.EnvironmentName.Returns("Production");
        var page = new Page { Title = "SampleTitle" };
        _fetchPageFromNos.Get(104, Arg.Any<CancellationToken>()).Returns(page);
        _pageStore.TitlePageNrExist("SampleTitle", 104).Returns(true);

        const int delayBetweenPageFetching = 50;

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
        _fetchPageFromNos.Get(104, Arg.Any<CancellationToken>()).Returns((Page)null!);

        const int delayBetweenPageFetching = 50;

        // Act
        await _theBot.Run(delayBetweenPageFetching, false, CancellationToken.None);

        // Assert
        await _mediator.DidNotReceive().Publish(Arg.Any<NewPageEvent>(), CancellationToken.None);
    }
}