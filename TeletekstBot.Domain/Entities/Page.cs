namespace TeletekstBot.Domain.Entities;

public class Page
{
    public int PageNumber { get; set; }
    public string? Title { get; init; }
    public string? Body { get; init; }
}