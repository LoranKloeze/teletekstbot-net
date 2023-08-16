using Microsoft.Extensions.Logging;
using NSubstitute;
using TeletekstBot.Application.Interfaces;
using TeletekstBot.Application.Services;
// ReSharper disable StringLiteralTypo

namespace TeletekstBot.Tests.Application;

public class AdminToolTests
{

    [Test]
    public void Run_ClearCommand_CallsClearAllPages()
    {
        // Arrange
        var logger = Substitute.For<ILogger<AdminTool>>();
        var pageStore = Substitute.For<IPageStore>();
        var theBot = Substitute.For<ITheBot>();
        var adminTool = new AdminTool(logger, pageStore, theBot);
        
        var cmdArgs = new[] { "someArg", "cache-clear" };

        // Act
        adminTool.Run(cmdArgs, CancellationToken.None);

        // Assert
        pageStore.Received(1).ClearAllPages();
    }
    
    [Test]
    public void Run_WarmupCommand_CallsTheBotWithoutPublishing()
    {
        // Arrange
        var logger = Substitute.For<ILogger<AdminTool>>();
        var pageStore = Substitute.For<IPageStore>();
        var theBot = Substitute.For<ITheBot>();
        var adminTool = new AdminTool(logger, pageStore, theBot);
        
        var cmdArgs = new[] { "someArg", "cache-warmup" };

        const int expectedDelay = 200;
        const bool expectedDoPublish = false;

        // Act
        adminTool.Run(cmdArgs, CancellationToken.None);

        // Assert
        theBot.Received(1).Run(expectedDelay, false, expectedDoPublish, CancellationToken.None);
    }
    
    [Test]
    public void Run_UnknownCommand()
    {
        // Arrange
        var logger = Substitute.For<ILogger<AdminTool>>();
        var pageStore = Substitute.For<IPageStore>();        
        var theBot = Substitute.For<ITheBot>();
        var adminTool = new AdminTool(logger, pageStore, theBot);
        
        var cmdArgs = new[] { "someArg", "nonexistingcmd" };
        
        // Act and Assert
        Assert.DoesNotThrow(() => adminTool.Run(cmdArgs, CancellationToken.None));
    }


}