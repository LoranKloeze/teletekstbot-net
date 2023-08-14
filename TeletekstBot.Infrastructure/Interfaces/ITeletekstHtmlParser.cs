using TeletekstBot.Domain.Entities;

namespace TeletekstBot.Infrastructure.Interfaces;

public interface ITeletekstHtmlParser
{
    void LoadHtml(string html);
    Page ToPage();
    bool IsANewsPage();

    public List<Page> RelevantPages();
}