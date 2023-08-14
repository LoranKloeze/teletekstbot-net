using TeletekstBot.Domain.Entities;

namespace TeletekstBot.Application.Interfaces;

public interface IFetchPagesFromNos
{
    public Task<List<Page>> GetPages();
}