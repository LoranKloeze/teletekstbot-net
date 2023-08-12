using TeletekstBot.Domain.Entities;

namespace TeletekstBot.Application.Interfaces;

public interface IFetchPageFromNos
{
    public Task<Page?> Get(int pageNr, CancellationToken stoppingToken);

}