using StackExchange.Redis;
using TeletekstBot.Application.Interfaces;

namespace TeletekstBot.Infrastructure.Services;

public class PageStore : IPageStore
{
    private readonly IConnectionMultiplexer _redis;
    public PageStore(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public bool TitlePageNrExist(string title, int pageNr)
    {
        var db = _redis.GetDatabase();
        var storedTitle = db.StringGet($"page:{pageNr}");

        return storedTitle == title;
    }

    public void SaveTitlePageNr(string title, int pageNr)
    {
        var db = _redis.GetDatabase();
        db.StringSet($"page:{pageNr}", title);
    }

    public void ClearAllPages()
    {
        var db = _redis.GetDatabase();
        foreach (var server in _redis.GetServers())
        {
            foreach (var key in server.Keys(pattern: "page:*"))
            {
                db.KeyDelete(key);
            }
        }
    }
}