using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.Application.Notifications;
using TeletekstBot.Domain.Entities;

namespace TeletekstBot.Application.Services;

public class TheBot : ITheBot
{
    private readonly IPageStore _pageStore;
    private readonly ILogger<TheBot> _logger;
    private readonly IFetchPageDetailsFromNos _fetchPageDetailsFromNos;
    private readonly IHostEnvironment _env;
    private readonly IFetchPagesFromNos _fetchPagesFromNos;

    private readonly IMediator _mediator;

    public TheBot(IMediator mediator, IPageStore pageStore, ILogger<TheBot> logger,
        IFetchPageDetailsFromNos fetchPageDetailsFromNos, IFetchPagesFromNos fetchPagesFromNos, IHostEnvironment env)
    {
        _fetchPageDetailsFromNos = fetchPageDetailsFromNos;
        _pageStore = pageStore;
        _logger = logger;
        _env = env;
        _mediator = mediator;
        _fetchPagesFromNos = fetchPagesFromNos;
    }

    public async Task Run(int delayBetweenPageFetching, bool runForever, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var relevantPages = await _fetchPagesFromNos.GetPages();
            LogPageOverview(relevantPages);
            
            foreach (var page in relevantPages)
            {
                
                var checkIfPagesExistsInStore = _env.IsProduction();

                // Retrieve screenshot and page from NOS
                var (screenshotPath, pageWithBody) = await _fetchPageDetailsFromNos.GetPageAndScreenshot(page.PageNumber);
                if (pageWithBody == null || string.IsNullOrEmpty(pageWithBody.Body))
                {
                    _logger.LogInformation("Page {PageNr} with title '{Title}' has no body, skipping...",
                        page.PageNumber, page.Title);
                    // Wait a bit to prevent any rate limiting
                    await Task.Delay(delayBetweenPageFetching, stoppingToken);
                    continue;
                }
                
                page.Body = pageWithBody.Body;
                
                // Check if page already is posted by checking for it in the store
                if (checkIfPagesExistsInStore && _pageStore.TitlePageNrExist(page.Title, page.PageNumber))
                {
                    _logger.LogInformation("Page {PageNr} already exists with title '{Title}', skipping...",
                        page.PageNumber, page.Title);
                    // Wait a bit to prevent any rate limiting
                    await Task.Delay(delayBetweenPageFetching, stoppingToken);
                    continue;
                }

                _logger.LogInformation("Page {PageNr} with title '{Title}' is new", page.PageNumber, page.Title);

                // Page is new, let's save it in the store
                _pageStore.SaveTitlePageNr(page.Title, page.PageNumber);

                // Publish the page
                await _mediator.Publish(new NewPageEvent(page, screenshotPath), stoppingToken);
                _logger.LogInformation("Done publishing page {PageNr} with title '{Title}'", page.PageNumber, page.Title);

                // Wait a bit to prevent any rate limiting
                await Task.Delay(delayBetweenPageFetching, stoppingToken);

            }
            
            if (!runForever)
            {
                break;
            }
        }
    }

    private void LogPageOverview(List<Page> pages)
    {
        _logger.LogInformation("Relevant pages found:");
        foreach (var page in pages)
        {
            _logger.LogInformation("[{PageNr}] '{Title}'", page.PageNumber, page.Title);
        }
    }
}