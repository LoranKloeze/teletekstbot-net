
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.Domain.Entities;
using TeletekstBot.Infrastructure.Dtos;
using TeletekstBot.Infrastructure.Interfaces;

namespace TeletekstBot.Infrastructure.Services;

public class FetchPageFromNos : IFetchPageFromNos
{
    private readonly ILogger<FetchPageFromNos> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IParseIncomingPage _parseIncomingPage;
    
    public FetchPageFromNos(ILogger<FetchPageFromNos> logger, IHttpClientFactory httpClientFactory, IParseIncomingPage parseIncomingPage)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _parseIncomingPage = parseIncomingPage;
        
    }
    public async Task<Page?> Get(int pageNr, CancellationToken stoppingToken)
    {
        _logger.LogInformation("Fetching page {PageNr} from NOS", pageNr);
        
        var client = _httpClientFactory.CreateClient();
        
        IncomingPage? incomingPage;
        
        try
        {
            incomingPage = await
                client.GetFromJsonAsync<IncomingPage>($"https://teletekst-data.nos.nl/json/{pageNr}",
                    stoppingToken);
        }
        catch (HttpRequestException)
        {
            _logger.LogInformation("Page {Page} not found", pageNr);
            return null;
        }
        if (string.IsNullOrEmpty(incomingPage?.Content))
        {
            _logger.LogInformation("Could not cast response to IncomingPast with nr {Page}", pageNr);
            return null;
        }
        
        var page = _parseIncomingPage.ParseHtml(incomingPage.Content);
        page.PageNumber = pageNr;
        
        LogPage(page);

        return page;
    }

    private void LogPage(Page page)
    {
        _logger.LogInformation("---------------------");
        _logger.LogInformation("Page: [{PageNr}] {Title}", page.PageNumber, page.Title);
        _logger.LogInformation("Content:");
        _logger.LogInformation("{Body}", string.Concat(page.Body.AsSpan(0,25), "..."));
    }
}
