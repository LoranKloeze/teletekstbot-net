namespace TeletekstBot.Application.Interfaces;

public interface IFetchPageFromNos
{
    public Task<(string, Domain.Entities.Page?)> GetPageAndScreenshot(int pageNr);
}