namespace TeletekstBot.Application.Interfaces;

public interface IFetchScreenshotFromNos
{
    public Task<(string, Domain.Entities.Page?)> GetPageAndScreenshot(int pageNr);
}