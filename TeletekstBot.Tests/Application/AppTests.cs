using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.Application.Services;

namespace TeletekstBot.Tests.Application;

public class AppTests
{
    private ILogger<App> _logger = null!;
    private IHost _host = null!;
    private ITheBot _theBot = null!;
    private IAdminTool _adminTool = null!;
    private App _app = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<App>>();
        _host = Substitute.For<IHost>();
        _theBot = Substitute.For<ITheBot>();
        _adminTool = Substitute.For<IAdminTool>();
        _app = new App(_logger, _host, _theBot, _adminTool);
    }

    [Test]
    public async Task Run_WithSingleArg_ShouldStartWorker()
    {
        var args = new[] { "arg1" };
        var stoppingToken = new CancellationToken();

        await _app.Run(args, stoppingToken);

        await _theBot.Received().Run(Arg.Any<int>(), true, stoppingToken);
    }

    [Test]
    public async Task Run_WithMultipleArgs_ShouldStartAdminTool()
    {
        var args = new[] { "arg1", "arg2" };
        var stoppingToken = new CancellationToken();

        await _app.Run(args, stoppingToken);

        _adminTool.Received().Run(args);
        await _host.Received().StopAsync(stoppingToken);
    }
}