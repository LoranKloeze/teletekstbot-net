namespace TeletekstBot.Application.Interfaces;

public interface IAdminTool
{
    public Task Run(string[] cmdArgs, CancellationToken stoppingToken);
}