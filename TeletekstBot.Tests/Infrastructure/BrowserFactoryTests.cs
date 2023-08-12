using Microsoft.Extensions.Configuration;
using NSubstitute;
using PuppeteerSharp;
using TeletekstBot.Infrastructure.Factories;
// ReSharper disable StringLiteralTypo

namespace TeletekstBot.Tests.Infrastructure;

public class BrowserFactoryTests
{
    [Test]
    public void Create_ShouldReturnBrowser()
    {
        /*
         *  In this test we don't want to connect to the Chrome instance so
         *  we provide a bogus host. Puppeteer will throw an exception but
         *  we will use that exception to verify that the BrowserFactory
         *  actually tried to create a connected browser. That's enough
         *  for now.
         */
        var configuration = Substitute.For<IConfiguration>();
        const string bogusHost = "nonexisting3fg78g8.localhost";
        configuration["Chrome:Host"].Returns($"ws://{bogusHost}");
        
        var browserFactory = new BrowserFactory(configuration);

        var exception = Assert.ThrowsAsync<ProcessException>(CreateBrowser);
        Assert.That(exception?.ToString(), Does.Contain($"({bogusHost}:80)"));
        return;

        async Task CreateBrowser() => await browserFactory.Create();
        
    }
    


}