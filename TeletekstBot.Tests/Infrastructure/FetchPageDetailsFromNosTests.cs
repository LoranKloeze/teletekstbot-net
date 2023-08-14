using Microsoft.Extensions.Logging;
using NSubstitute;
using PuppeteerSharp;
using TeletekstBot.Infrastructure.Interfaces;
using TeletekstBot.Infrastructure.Services;

namespace TeletekstBot.Tests.Infrastructure;

public class FetchPageDetailsFromNosTests
{
    [Test]
    public async Task GetPageAndScreenshot_WithValidPageNumberReturnsScreenshotFilePathAndPage()
    {
        // Arrange
        const int pageNr = 110;
        
        var logger = Substitute.For<ILogger<FetchPageDetailsFromNos>>();
        var browserFactory = Substitute.For<IBrowserFactory>();
        var teletekstHtmlParser = Substitute.For<ITeletekstHtmlParser>();
        var browser = Substitute.For<IBrowser>();
        var page = Substitute.For<IPage>();
        
        var html = MockFile.GetFileText("full_nos_page_110.html");
        page.GetContentAsync().ReturnsForAnyArgs(html);
        
        browserFactory.Create().ReturnsForAnyArgs(browser);
        browser.NewPageAsync().ReturnsForAnyArgs(page);

        teletekstHtmlParser.IsANewsPage().ReturnsForAnyArgs(true);
        var returnedPage = new TeletekstBot.Domain.Entities.Page { PageNumber = pageNr };
        teletekstHtmlParser.ToPage(pageNr).ReturnsForAnyArgs(returnedPage);
        
        var fetchPageFromNos = new FetchPageDetailsFromNos(logger, browserFactory, teletekstHtmlParser);

        var expectedFilePath = Path.Combine(Path.GetTempPath(), $"screenshot_{pageNr}.png");
        
        // Act
        var (filePath, fetchedPage) = await fetchPageFromNos.GetPageAndScreenshot(pageNr);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(filePath, Is.EqualTo(expectedFilePath));
            Assert.That(fetchedPage, Is.Not.Null);
            Assert.That(fetchedPage?.PageNumber, Is.EqualTo(pageNr));
        });
        await page.ReceivedWithAnyArgs().ScreenshotAsync(default, default);
    }
}