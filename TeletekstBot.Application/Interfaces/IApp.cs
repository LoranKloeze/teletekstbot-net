namespace TeletekstBot.Application.Interfaces;

public interface IApp
{
    public Task Run(string[] args, CancellationToken stoppingToken);
}