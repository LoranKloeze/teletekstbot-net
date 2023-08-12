using TeletekstBot.Domain.Entities;

namespace TeletekstBot.Application.Interfaces;

public interface IMastodonService
{
    public Task Post(Page page, Stream screenshotStream);
    
}