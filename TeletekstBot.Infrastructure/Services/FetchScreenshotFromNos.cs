using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.Infrastructure.Interfaces;

namespace TeletekstBot.Infrastructure.Services;

public class FetchScreenshotFromNos : IFetchScreenshotFromNos
{
    private readonly IBrowserFactory _browserFactory;
    private readonly ILogger<FetchScreenshotFromNos>  _logger;
    
    private const string NosUrl = "https://nos.nl/teletekst";

    private const string WatchDogJavascript =
        "() => document.getElementById('teletekst').innerText.indexOf(\"volgende  nieuws  weer&verkeer  sport\") > 0;";
    
    // Dimensions of the complete browser window
    private const int ViewPortWidth = 460;
    private const int ViewPortHeight = 950;
    
    // Clip of the area we want to screenshot
    private const int ClipWidth = 370;
    private const int ClipHeight = 460;
    private const int ClipStartX = 40;
    private const int ClipStartY = 330;
    
    
    
    public FetchScreenshotFromNos(ILogger<FetchScreenshotFromNos> logger, IBrowserFactory browserFactory)
    {
        _logger = logger;
        _browserFactory = browserFactory;
    }
    public async Task<string> Get(int pageNr)
    {
        var browser = await _browserFactory.Create();
        var page = await browser.NewPageAsync();
        await page.SetViewportAsync(new ViewPortOptions
        {
            Width = ViewPortWidth,
            Height = ViewPortHeight
            
        });
        
        _logger.LogInformation("Creating a screenshot for page {PageNr}", pageNr);

        var filePath = Path.Combine(Path.GetTempPath(), $"screenshot_{pageNr}.png");
        
        await page.GoToAsync($"{NosUrl}#{pageNr}");
        
        await page.WaitForNetworkIdleAsync();
        await page.WaitForFunctionAsync(WatchDogJavascript);

        await page.ScreenshotAsync(filePath, new ScreenshotOptions
        {
            Clip = new Clip
            {
                Width = ClipWidth,
                Height = ClipHeight,
                X = ClipStartX,
                Y = ClipStartY
            }
        });
        
        
        _logger.LogInformation("Screenshot created at {FilePath} for page {PageNr}", filePath, pageNr);
        return filePath;
    }
}