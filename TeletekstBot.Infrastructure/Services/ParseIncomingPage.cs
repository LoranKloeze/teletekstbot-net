using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using TeletekstBot.Domain.Entities;
using TeletekstBot.Infrastructure.Interfaces;

namespace TeletekstBot.Infrastructure.Services;

public partial class ParseIncomingPage : IParseIncomingPage
{
    private const string TitleXPath = "//span[6]";

    private readonly HtmlDocument _htmlDocument;

    public ParseIncomingPage(HtmlDocument htmlDocument)
    {
        _htmlDocument = htmlDocument;
    }

    public Page ParseHtml(string html)
    {
        _htmlDocument.LoadHtml(html);

        return new Page
        {
            Title = ExtractTitle(),
            Body = ExtractBody()
        };
    }

    private string ExtractTitle()
    {
        var node = _htmlDocument.DocumentNode.SelectSingleNode(TitleXPath);
        return node != null ? node.InnerHtml.Trim() : string.Empty;
    }

    private string ExtractBody()
    {
        var sb = new StringBuilder();
        const string specialEofBodyChar = "&#xF020;";
        const int firstBodyNodeIndex = 13;

        var childNodes = _htmlDocument.DocumentNode.ChildNodes.Skip(firstBodyNodeIndex);
        foreach (var node in childNodes)
        {
            if (node.InnerHtml.StartsWith(specialEofBodyChar))
            {
                break;
            }
                
            sb.Append(node.InnerHtml);
        }

        var sanitized = WebUtility.HtmlDecode(sb.ToString().Trim());
        sanitized = sanitized.Replace("\r", "").Replace("\n", "");
        sanitized = RemoveHtmlEntities(sanitized);
        
        sanitized = WhitespaceRegex().Replace(sanitized, " ");
        
        return sanitized;
    }
    
    private static string RemoveHtmlEntities(string html)
    {
        return HtmlTagsMyRegex().Replace(html, string.Empty);
    }

    [GeneratedRegex("<.*?>")]
    private static partial Regex HtmlTagsMyRegex();
    
    [GeneratedRegex("\\s+")]
    private static partial Regex WhitespaceRegex();
}