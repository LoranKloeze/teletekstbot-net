using NSubstitute;
using StackExchange.Redis;
using TeletekstBot.Infrastructure.Services;

namespace TeletekstBot.Tests.Infrastructure;

public class PageStoreTests
{
    [Test]
    public void TitlePageNrExist_ReturnsTrueWhenPageExists()
    {
        // Arrange
        var (connectionMultiplexer, database) = ConstructDatabase();
        database.StringGet("page:110").Returns((RedisValue)"A title");
        
        var pageStore = new PageStore(connectionMultiplexer);
        
        // Act
        var result = pageStore.TitlePageNrExist("A title", 110);
        
        // Assert
        Assert.That(result, Is.True);
        
    }
    
    [Test]
    public void TitlePageNrExist_ReturnsFalseWhenPageNotExists()
    {
        // Arrange
        var (connectionMultiplexer, database) = ConstructDatabase();
        database.StringGet("page:110").Returns((RedisValue)"A title");
        
        var pageStore = new PageStore(connectionMultiplexer);
        
        // Act
        var result = pageStore.TitlePageNrExist("A new title", 110);
        
        // Assert
        Assert.That(result, Is.False);
        
    }
    
    [Test]
    public void SaveTitlePageNr_SavesTitleWithExpectedPageNumber()
    {
        // Arrange
        var (connectionMultiplexer, database) = ConstructDatabase();
        
        var pageStore = new PageStore(connectionMultiplexer);
        
        // Act
        pageStore.SaveTitlePageNr("My title", 110);
        
        // Assert
        database.Received().StringSet("page:110", "My title");

    }
    
    [Test]
    public void ClearAllPages_RemovesRelevantKeysFromRedis()
    {
        // Arrange
        var (connectionMultiplexer, database) = ConstructDatabase();
        var server = Substitute.For<IServer>();
        server.Keys(pattern: "page:*").Returns(new[] { (RedisKey)"page:110",(RedisKey)"page:111" });
        connectionMultiplexer.GetServers().Returns(new[] {server});
        
        var pageStore = new PageStore(connectionMultiplexer);
        
        // Act
        pageStore.ClearAllPages();
        
        // Assert
        database.Received().KeyDelete("page:110");
        database.Received().KeyDelete("page:111");

    }

    private static (IConnectionMultiplexer, IDatabase) ConstructDatabase()
    {
        var connectionMultiplexer = Substitute.For<IConnectionMultiplexer>();
        var database = Substitute.For<IDatabase>();
        connectionMultiplexer.GetDatabase().Returns(database);
        return (connectionMultiplexer, database);
    }
}