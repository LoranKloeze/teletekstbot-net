using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.Infrastructure.Interfaces;

namespace TeletekstBot.Infrastructure.Services;

public class FetchPageFromNos : IFetchPageFromNos
{
    private readonly IBrowserFactory _browserFactory;
    private readonly ILogger<FetchPageFromNos>  _logger;
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
    
    public FetchPageFromNos(ILogger<FetchPageFromNos> logger, IBrowserFactory browserFactory, 
        ITeletekstHtmlParser teletekstHtmlParserParser)
    {
        _logger = logger;
        _browserFactory = browserFactory;
        _teletekstHtmlParserParser = teletekstHtmlParserParser;
    }
    
    public async Task<(string, Domain.Entities.Page?)> GetPageAndScreenshot(int pageNr)
    {
        var browser = await _browserFactory.Create();
        var browserPage = await browser.NewPageAsync();
        await browserPage.SetViewportAsync(new ViewPortOptions
        {
            Width = ViewPortWidth,
            Height = ViewPortHeight
            
        });
        
        _logger.LogInformation("Retrieving page {PageNr} from NOS", pageNr);

        var filePath = Path.Combine(Path.GetTempPath(), $"screenshot_{pageNr}.png");
        
        await browserPage.GoToAsync($"{NosUrl}#{pageNr}");
        await browserPage.WaitForNetworkIdleAsync();
        
        _logger.LogInformation("Retrieving html for page {PageNr}", pageNr);
        var html = await browserPage.GetContentAsync();
        _teletekstHtmlParserParser.LoadHtml(html);
        if (!_teletekstHtmlParserParser.IsANewsPage())
        {
            return (string.Empty, null);
        }

        var page = _teletekstHtmlParserParser.ToPage();
        page.PageNumber = pageNr;

        await browserPage.ScreenshotAsync(filePath, new ScreenshotOptions
        {
            Clip = new Clip
            {
                Width = ClipWidth,
                Height = ClipHeight,
                X = ClipStartX,
                Y = ClipStartY
            }
        });

        _logger.LogInformation("Screenshot and page created at {FilePath} for page {PageNr}", filePath, pageNr);
        return (filePath, page);
    }
}