using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.Infrastructure.Interfaces;
using TeletekstBot.Domain.Entities;

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
        // await browserPage.WaitForFunctionAsync(WatchDogJavascript);
        
        _logger.LogInformation("Retrieving html for page {PageNr}", pageNr);
        var html = await browserPage.GetContentAsync();
        if (!IsANewsPage(html))
        {
            return (string.Empty, null);
        }
        var page = ExtractPageFromHtml(html);
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

    private static Domain.Entities.Page ExtractPageFromHtml(string html)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);
        
        return new Domain.Entities.Page
        {
            Title = ExtractTitle(htmlDoc),
            Body = ExtractBody(htmlDoc)
        };
    }

    private static string ExtractTitle(HtmlDocument htmlDoc)
    {
        var titleNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"teletekst\"]/div[2]/pre/span[6]");
        return titleNode == null ? string.Empty : titleNode.InnerText.Trim();
    }
    
    private static string ExtractBody(HtmlDocument htmlDoc)
    {
        var sb = new StringBuilder();
        const string specialEofBodyChar = "&#xF020;";
        const int firstBodyNodeIndex = 13;

        var parentNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"teletekst\"]/div[2]/pre");
        if (parentNode == null)
        {
            return string.Empty;
        }
        
        var childNodes = parentNode.ChildNodes.Skip(firstBodyNodeIndex);
        foreach (var node in childNodes)
        {
            if (node.InnerHtml.StartsWith(specialEofBodyChar))
            {
                break;
            }
                
            sb.Append(node.InnerHtml);
        }

        var sanitized = WebUtility.HtmlDecode(sb.ToString().Trim());
        sanitized = sanitized.Replace("\r", "").Replace("\n", "");
        sanitized = RemoveHtmlEntities(sanitized);
        
        sanitized = WhitespaceRegex().Replace(sanitized, " ");
        
        return sanitized;
    }
    
    private bool IsANewsPage(string html)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);
        
        var containerNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"teletekst\"]");
        return containerNode == null || !containerNode.InnerText.Contains("copyright N O S");
    }
    
    private static string RemoveHtmlEntities(string html)
    {
        return HtmlTagsMyRegex().Replace(html, string.Empty);
    }
    
    private static Regex HtmlTagsMyRegex()
    {
        return new Regex("<.*?>", RegexOptions.Compiled);
    }
    
    private static Regex WhitespaceRegex()
    {
        return new Regex(@"\s+", RegexOptions.Compiled);
    }
    
    
}