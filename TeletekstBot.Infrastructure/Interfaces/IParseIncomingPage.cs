
using TeletekstBot.Domain.Entities;

namespace TeletekstBot.Infrastructure.Interfaces;

public interface IParseIncomingPage
{
    public Page ParseHtml(string content);
}