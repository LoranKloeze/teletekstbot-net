using PuppeteerSharp;

namespace TeletekstBot.Infrastructure.Interfaces;

public interface IBrowserFactory
{
    public Task<IBrowser> Create();
}