using TeletekstBot.Domain.Entities;

namespace TeletekstBot.Application.Interfaces;

public interface IBlueSkyService
{
    public Task Post(Page page, string screenshotPath, CancellationToken stoppingToken);
}