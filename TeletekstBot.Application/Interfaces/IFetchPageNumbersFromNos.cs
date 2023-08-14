namespace TeletekstBot.Application.Interfaces;

public interface IFetchPageNumbersFromNos
{
    public Task<List<int>> GetNumbers();
}