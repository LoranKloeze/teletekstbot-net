using Microsoft.Extensions.Logging;
using NSubstitute;
using PuppeteerSharp;
using TeletekstBot.Infrastructure.Interfaces;
using TeletekstBot.Infrastructure.Services;

namespace TeletekstBot.Tests.Infrastructure;

public class FetchScreenshotFromNosTests
{
    [Test]
    public async Task Get_WithValidPageNumberReturnsScreenshotFilePathAndPage()
    {
        // Arrange
        const int pageNr = 110;
        
        var logger = Substitute.For<ILogger<FetchScreenshotFromNos>>();
        var browserFactory = Substitute.For<IBrowserFactory>();
        var browser = Substitute.For<IBrowser>();
        var page = Substitute.For<IPage>();
        
        browserFactory.Create().ReturnsForAnyArgs(browser);
        browser.NewPageAsync().ReturnsForAnyArgs(page);
        
        var fetchScreenshotFromNos = new FetchScreenshotFromNos(logger, browserFactory);

        var expectedFilePath = Path.Combine(Path.GetTempPath(), $"screenshot_{pageNr}.png");
        
        // Act
        var (filePath, fetchedPage) = await fetchScreenshotFromNos.GetPageAndScreenshot(pageNr);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(filePath, Is.EqualTo(expectedFilePath));
            Assert.That(fetchedPage.PageNumber, Is.EqualTo(pageNr));
        });
        await page.ReceivedWithAnyArgs().ScreenshotAsync(default, default);
    }
}