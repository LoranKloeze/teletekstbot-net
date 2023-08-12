using Microsoft.Extensions.Configuration;
using PuppeteerSharp;
using TeletekstBot.Infrastructure.Interfaces;

namespace TeletekstBot.Infrastructure.Factories;

public class BrowserFactory : IBrowserFactory
{
    private readonly IConfiguration _configuration;
    
    public BrowserFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<IBrowser> Create()
    {
        return await Puppeteer.ConnectAsync(new ConnectOptions
        {
            BrowserWSEndpoint = _configuration["Chrome:Host"],
            IgnoreHTTPSErrors = true
        });
    }
}