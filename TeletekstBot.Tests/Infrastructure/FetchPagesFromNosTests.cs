using NSubstitute;
using PuppeteerSharp;
using TeletekstBot.Infrastructure.Interfaces;
using TeletekstBot.Infrastructure.Services;

namespace TeletekstBot.Tests.Infrastructure;

public class FetchPagesFromNosTests
{
    [Test]
    public async Task GetPages_FetchesPagesFromNos()
    {
        // Arrange
        var browserFactory = Substitute.For<IBrowserFactory>();
        var teletekstHtmlParser = Substitute.For<ITeletekstHtmlParser>();
        var browser = Substitute.For<IBrowser>();
        var page = Substitute.For<IPage>();

        browserFactory.Create().Returns(browser);
        browser.NewPageAsync().Returns(page);
        
        var fetchPageFromNos = new FetchPagesFromNos(browserFactory, teletekstHtmlParser);

        // Act
        await fetchPageFromNos.GetPages();
        
        // Assert
        await page.Received().GoToAsync("https://nos.nl/teletekst#101");
        teletekstHtmlParser.Received().RelevantPages();
    }
}