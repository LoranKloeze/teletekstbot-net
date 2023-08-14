namespace TeletekstBot.Application.Interfaces;

public interface IFetchPageDetailsFromNos
{
    public Task<(string, Domain.Entities.Page?)> GetPageAndScreenshot(int pageNr);
}