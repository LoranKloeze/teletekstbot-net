namespace TeletekstBot.Application.Interfaces;

public interface ITheBot
{
    public Task Run(int delayBetweenPageFetching, bool runForever, CancellationToken stoppingToken);
    public Task Run(int delayBetweenPageFetching, bool runForever, bool doPublish, CancellationToken stoppingToken);
}