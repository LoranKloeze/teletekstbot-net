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
        var adminTool = new AdminTool(logger, pageStore);
        
        var cmdArgs = new[] { "someArg", "clear" };

        // Act
        adminTool.Run(cmdArgs);

        // Assert
        pageStore.Received(1).ClearAllPages();
    }
    
    [Test]
    public void Run_UnknownCommand()
    {
        // Arrange
        var logger = Substitute.For<ILogger<AdminTool>>();
        var pageStore = Substitute.For<IPageStore>();
        var adminTool = new AdminTool(logger, pageStore);
        
        var cmdArgs = new[] { "someArg", "nonexistingcmd" };
        
        // Act and Assert
        Assert.DoesNotThrow(() => adminTool.Run(cmdArgs));
    }


}