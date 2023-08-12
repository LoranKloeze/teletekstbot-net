namespace TeletekstBot.Application.Interfaces;

public interface IFetchScreenshotFromNos
{
    public Task<string> Get(int pageNr);
}