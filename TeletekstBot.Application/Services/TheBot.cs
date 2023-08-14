using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.Application.Notifications;

namespace TeletekstBot.Application.Services;

public class TheBot : ITheBot
{
    private readonly IPageStore _pageStore;
    private readonly ILogger<TheBot> _logger;
    private readonly IFetchPageFromNos _fetchPageFromNos;
    private readonly IHostEnvironment _env;
    private readonly IFetchPageNumbersFromNos _fetchPageNumbersFromNos;

    private readonly IMediator _mediator;

    private const int PageNumberStart = 104;
    private const int PageNumberEnd = 150;

    public TheBot(IMediator mediator, IPageStore pageStore, ILogger<TheBot> logger,
        IFetchPageFromNos fetchPageFromNos, IFetchPageNumbersFromNos fetchPageNumbersFromNos, IHostEnvironment env)
    {
        _fetchPageFromNos = fetchPageFromNos;
        _pageStore = pageStore;
        _logger = logger;
        _env = env;
        _mediator = mediator;
        _fetchPageNumbersFromNos = fetchPageNumbersFromNos;
    }

    public async Task Run(int delayBetweenPageFetching, bool runForever, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var relevantPageNumbers = await _fetchPageNumbersFromNos.GetNumbers();
            
            LogPageNumbers(relevantPageNumbers);
            
            foreach (var pageNr in relevantPageNumbers)
            {
                if (pageNr is > PageNumberEnd or < PageNumberStart)
                {
                    continue;
                }
                
                var checkIfPagesExistsInStore = _env.IsProduction();

                // Retrieve screenshot and page from NOS
                var (screenshotPath, page) = await _fetchPageFromNos.GetPageAndScreenshot(pageNr);
                
                if (page == null || string.IsNullOrEmpty(page.Title))
                {
                    _logger.LogInformation("Page {PageNr} is not a valid news page at the moment, skipping...", pageNr);
                    // Wait a bit to prevent any rate limiting
                    await Task.Delay(delayBetweenPageFetching, stoppingToken);
                    continue;
                }

                // Check if page already is posted by checking for it in the store
                if (checkIfPagesExistsInStore && _pageStore.TitlePageNrExist(page.Title, pageNr))
                {
                    _logger.LogInformation("Page {PageNr} already exists with title '{Title}', skipping...",
                        pageNr, page.Title);
                    // Wait a bit to prevent any rate limiting
                    await Task.Delay(delayBetweenPageFetching, stoppingToken);
                    continue;
                }

                _logger.LogInformation("Page {PageNr} with title '{Title}' is new", pageNr, page.Title);

                // Page is new, let's save it in the store
                _pageStore.SaveTitlePageNr(page.Title, pageNr);

                // Publish the page
                await _mediator.Publish(new NewPageEvent(page, screenshotPath), stoppingToken);
                _logger.LogInformation("Done publishing page {PageNr} with title '{Title}'", pageNr, page.Title);

                // Wait a bit to prevent any rate limiting
                await Task.Delay(delayBetweenPageFetching, stoppingToken);

            }
            
            if (!runForever)
            {
                break;
            }
        }
    }

    private void LogPageNumbers(IEnumerable<int> numbers)
    {
        var strNumbers = string.Join(", ", numbers);
        _logger.LogInformation("Relevant page numbers found: {Numbers}", strNumbers);
    }
}