using PuppeteerSharp;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.Infrastructure.Interfaces;

namespace TeletekstBot.Infrastructure.Services;

public class FetchPageNumbersFromNos : IFetchPageNumbersFromNos
{
    private readonly IBrowserFactory _browserFactory;
    private readonly ITeletekstHtmlParser _teletekstHtmlParserParser;
    
    private const string NosIndexUrl = "https://nos.nl/teletekst#101";

    public FetchPageNumbersFromNos(IBrowserFactory browserFactory, 
        ITeletekstHtmlParser teletekstHtmlParserParser)
    {
        _browserFactory = browserFactory;
        _teletekstHtmlParserParser = teletekstHtmlParserParser;
    }
    public async Task<List<int>> GetNumbers()
    {
        var browserPage = await SetupBrowserPage(); 
        
        var html = await browserPage.GetContentAsync();
        _teletekstHtmlParserParser.LoadHtml(html);

        return _teletekstHtmlParserParser.RelevantPageNumbers();
    }
    
    private async Task<IPage> SetupBrowserPage()
    {
        var browser = await _browserFactory.Create();
        var browserPage = await browser.NewPageAsync();
        await browserPage.GoToAsync(NosIndexUrl);
        await browserPage.WaitForNetworkIdleAsync();

        return browserPage;
    }
}