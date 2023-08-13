using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using TeletekstBot.Domain.Entities;
using TeletekstBot.Infrastructure.Interfaces;

namespace TeletekstBot.Infrastructure.Services;

public partial class TeletekstHtmlParser : ITeletekstHtmlParser
{
    private readonly HtmlDocument _htmlDocument;

    public TeletekstHtmlParser(HtmlDocument htmlDocument)
    {
        _htmlDocument = htmlDocument;
    }

    public void LoadHtml(string html)
    {
        _htmlDocument.LoadHtml(html);
    }

    public Page ToPage()
    {
        return new Page
        {
            Title = ExtractTitle(),
            Body = ExtractBody()
        };
    }

    public bool IsANewsPage()
    {
        var containerNode = _htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"teletekst\"]");
        return containerNode == null || !containerNode.InnerText.Contains("copyright N O S");
    }
    
    private string ExtractTitle()
    {
        var titleNode = _htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"teletekst\"]/div[2]/pre/span[6]");
        if (titleNode == null)
        {
            return string.Empty;
        }
        
        var titleText = titleNode.InnerText.Trim();
            
        return WebUtility.HtmlDecode(titleText);
    }
    
    private string ExtractBody()
    {
        var sb = new StringBuilder();
        const string specialEofBodyChar = "&#xF020;";
        const int firstBodyNodeIndex = 13;

        var parentNode = _htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"teletekst\"]/div[2]/pre");
        if (parentNode == null)
        {
            return string.Empty;
        }
        
        var childNodes = parentNode.ChildNodes.Skip(firstBodyNodeIndex);
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
    

    [GeneratedRegex("<.*?>", RegexOptions.Compiled)]
    private static partial Regex HtmlTagsMyRegex();
    
    [GeneratedRegex("\\s+", RegexOptions.Compiled)]
    private static partial Regex WhitespaceRegex();
}