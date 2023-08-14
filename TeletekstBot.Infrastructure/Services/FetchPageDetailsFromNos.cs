using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.Infrastructure.Interfaces;

namespace TeletekstBot.Infrastructure.Services;

public class FetchPageDetailsFromNos : IFetchPageDetailsFromNos
{
    private readonly IBrowserFactory _browserFactory;
    private readonly ILogger<FetchPageDetailsFromNos>  _logger;
    private readonly ITeletekstHtmlParser  _teletekstHtmlParserParser;
    
    private const string NosUrl = "https://nos.nl/teletekst";

    // Dimensions of the complete browser window
    private const int ViewPortWidth = 460;
    private const int ViewPortHeight = 950;
    
    // Clip of the area we want to screenshot
    private const int ClipWidth = 370;
    private const int ClipHeight = 460;
    private const int ClipStartX = 40;
    private const int ClipStartY = 330;
    
    public FetchPageDetailsFromNos(ILogger<FetchPageDetailsFromNos> logger, IBrowserFactory browserFactory, 
        ITeletekstHtmlParser teletekstHtmlParserParser)
    {
        _logger = logger;
        _browserFactory = browserFactory;
        _teletekstHtmlParserParser = teletekstHtmlParserParser;
    }
    
    public async Task<(string, Domain.Entities.Page?)> GetPageAndScreenshot(int pageNr)
    {
        _logger.LogInformation("Retrieving page {PageNr} from NOS", pageNr);
        
        var browserPage = await SetupBrowserPage($"{NosUrl}#{pageNr}"); 
        
        var html = await browserPage.GetContentAsync();
        _teletekstHtmlParserParser.LoadHtml(html);
        
        if (!_teletekstHtmlParserParser.IsANewsPage())
        {
            return (string.Empty, null);
        }

        var nosPage = _teletekstHtmlParserParser.ToPage();
        nosPage.PageNumber = pageNr;
        
        var screenshotPath = Path.Combine(Path.GetTempPath(), $"screenshot_{pageNr}.png");
        await CreateScreenshot(browserPage, screenshotPath);

        _logger.LogInformation("Screenshot and page created at {FilePath} for page {PageNr}", screenshotPath, pageNr);
        
        return (screenshotPath, nosPage);
    }
    
    private async Task<IPage> SetupBrowserPage(string url)
    {
        var browser = await _browserFactory.Create();
        var browserPage = await browser.NewPageAsync();
        await browserPage.SetViewportAsync(new ViewPortOptions
        {
            Width = ViewPortWidth,
            Height = ViewPortHeight
            
        });
        await browserPage.GoToAsync(url);
        await browserPage.WaitForNetworkIdleAsync();

        return browserPage;
    }
    
    private static async Task CreateScreenshot(IPage browserPage, string screenshotPath)
    {
        await browserPage.ScreenshotAsync(screenshotPath, new ScreenshotOptions
        {
            Clip = new Clip
            {
                Width = ClipWidth,
                Height = ClipHeight,
                X = ClipStartX,
                Y = ClipStartY
            }
        });
    }
}