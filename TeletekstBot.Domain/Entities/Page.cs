namespace TeletekstBot.Domain.Entities;

public class Page
{
    public int PageNumber { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }

    private bool Equals(Page other)
    {
        return PageNumber == other.PageNumber && Title == other.Title && Body == other.Body;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Page)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PageNumber, Title, Body);
    }
}