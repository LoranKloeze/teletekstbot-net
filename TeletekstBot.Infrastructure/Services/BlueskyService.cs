using System.Text;
using FishyFlip;
using FishyFlip.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.Domain.Entities;
using TeletekstBot.Infrastructure.Extensions;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace TeletekstBot.Infrastructure.Services;

public class BlueSkyService : IBlueSkyService
{
    private readonly ILogger<BlueSkyService> _logger;
    private readonly IConfiguration _configuration;
    private readonly ATProtocol _atProtocol;

    public BlueSkyService(ILogger<BlueSkyService> logger, IConfiguration configuration, ATProtocol atProtocol)
    {
        _logger = logger;
        _configuration = configuration;
        _atProtocol = atProtocol;
    }
    public async Task Post(Page page, string screenshotPath, CancellationToken stoppingToken)
    {
        _logger.LogInformation("[BlueSky] Posting page {PageNr} with title '{Title}'", page.PageNumber, page.Title);
        
        var (handle, password) = GetHandleAndPassword();
        await _atProtocol.Server.CreateSessionAsync(handle, password, stoppingToken);
        
        var blobResult = await UploadScreenshot(screenshotPath, stoppingToken);
        
        await blobResult.SwitchAsync(
            async success =>
            {
                await CreatePost( success.Blob.ToImage(), page, stoppingToken);
            }, error =>
            {
              _logger.LogError("Error uploading image to Bluesky: {StatusCode} {Detail}", 
                  error.StatusCode, error.Detail);
                return Task.CompletedTask;
            }
        );
        
    }
    private (string, string) GetHandleAndPassword()
    {
        var handle = _configuration["Bluesky:Handle"];
        var password = _configuration["Bluesky:Password"];
        
        if (string.IsNullOrEmpty(handle) || string.IsNullOrEmpty(password))
        {
            throw new Exception("No handle and/or password configured for Bluesky");
        }

        return (handle, password);
    }

    private async Task<Result<UploadBlobResponse>> UploadScreenshot(string path, 
        CancellationToken stoppingToken)
    {
        var stream = File.OpenRead(path);
        var content = new StreamContent(stream);
        content.Headers.ContentLength = stream.Length;
        content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        return await _atProtocol.Repo.UploadBlobAsync(content, stoppingToken);
    }

    private async Task CreatePost(Image screenshot, Page page, CancellationToken stoppingToken)
    {
        var url = $"https://nos.nl/teletekst#{page.PageNumber}";
        var body = $"[{page.PageNumber}] {page.Title}";
        if (string.IsNullOrEmpty(page.Title))
        {
            throw new Exception("Page cannot have empty title");
        }
        var promptStart = body.IndexOf(page.Title, StringComparison.InvariantCulture);
        var promptEnd = promptStart + Encoding.Default.GetBytes(page.Title).Length;
        var index = new FacetIndex(promptStart, promptEnd);
        var link = FacetFeature.CreateLink(url);
        var facet = new Facet(index, link);

        await _atProtocol.Repo.CreatePostAsync(body, new[] { facet },
            new ImagesEmbed(screenshot, page.Body.AddSpacesWhenApplicable()), cancellationToken: stoppingToken);
    }
}