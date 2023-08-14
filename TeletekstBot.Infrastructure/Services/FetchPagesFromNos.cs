using PuppeteerSharp;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.Infrastructure.Interfaces;
using Page = TeletekstBot.Domain.Entities.Page;

namespace TeletekstBot.Infrastructure.Services;

public class FetchPagesFromNos : IFetchPagesFromNos
{
    private readonly IBrowserFactory _browserFactory;
    private readonly ITeletekstHtmlParser _teletekstHtmlParserParser;
    
    private const string NosIndexUrl = "https://nos.nl/teletekst#101";

    public FetchPagesFromNos(IBrowserFactory browserFactory, 
        ITeletekstHtmlParser teletekstHtmlParserParser)
    {
        _browserFactory = browserFactory;
        _teletekstHtmlParserParser = teletekstHtmlParserParser;
    }

    public async Task<List<Page>> GetPages()
    {
        var browserPage = await SetupBrowserPage(); 
        
        var html = await browserPage.GetContentAsync();
        _teletekstHtmlParserParser.LoadHtml(html);

        return _teletekstHtmlParserParser.RelevantPages();
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