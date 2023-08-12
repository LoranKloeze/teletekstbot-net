namespace TeletekstBot.Application.Interfaces;

public interface IPageStore
{
    public bool TitlePageNrExist(string title, int pageNr);
    public void SaveTitlePageNr(string title, int pageNr);
    public void ClearAllPages();
}