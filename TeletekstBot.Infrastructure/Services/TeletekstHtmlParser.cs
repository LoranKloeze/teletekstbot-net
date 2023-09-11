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

    // Define the characters that need a space afterwards
    private const string CharsRequiringTrailingSpace = ".,:";

    public TeletekstHtmlParser(HtmlDocument htmlDocument)
    {
        _htmlDocument = htmlDocument;
    }

    public void LoadHtml(string html)
    {
        _htmlDocument.LoadHtml(html);
    }

    public Page ToPage(int pageNr)
    {
        return new Page
        {
            PageNumber = pageNr,
            Title = ExtractTitle(),
            Body = ExtractBody()
        };
    }

    public List<Page> RelevantPages()
    {
        var containerNode = _htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"teletekst\"]");
        
        var pageLinkElements = containerNode.SelectNodes("//a[@class='yellow' and not(@id)]");
        
        var pages = new List<Page>();
        var nrsSeen = new List<int>();

        foreach (var pageLinkElement in pageLinkElements)
        {
            if (!int.TryParse(pageLinkElement.InnerHtml, out var pageNumber)) continue;
            if (pageNumber is < 104 or > 199) continue;
            if (nrsSeen.Contains(pageNumber)) continue;

            var titleSpan = pageLinkElement.ParentNode?.PreviousSibling;
            if (titleSpan == null || string.IsNullOrEmpty(titleSpan.InnerHtml)) continue;
            
            var title = CleanTitle(titleSpan.InnerHtml);
            pages.Add(new Page
            {
                PageNumber = pageNumber,
                Title = title
            });
            nrsSeen.Add(pageNumber);
        }

        return pages;
    }

    public bool IsANewsPage()
    {
        var containerNode = _htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"teletekst\"]");
        return containerNode == null || !containerNode.InnerText.Contains("copyright N O S");
    }

    private string ExtractTitle()
    {
        var titleNode = _htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"teletekst\"]/div[2]/pre/span[6]");
        return titleNode == null ? string.Empty : CleanTitle(titleNode.InnerText);
    }

    private string ExtractBody()
    {
        var sb = new StringBuilder();
        string[] specialEofBodyChars = { "\xF020", "&#xF020;" };
        const int firstBodyNodeIndex = 13;

        var parentNode = _htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"teletekst\"]/div[2]/pre");
        if (parentNode == null)
        {
            return string.Empty;
        }

        var childNodes = parentNode.ChildNodes.Skip(firstBodyNodeIndex);
        foreach (var node in childNodes)
        {
            if (specialEofBodyChars.Any(chr => node.InnerHtml.StartsWith(chr)))
            {
                break;
            }

            sb.Append(node.InnerHtml);
        }

        var sanitized = WebUtility.HtmlDecode(sb.ToString().Trim());
        sanitized = sanitized.Replace("\r", "").Replace("\n", "");
        sanitized = RemoveHtmlEntities(sanitized);

        sanitized = WhitespaceRegex().Replace(sanitized, " ");

        sanitized = AddSpacesWhenApplicable(sanitized);
        
        return sanitized;
    }

    private static string RemoveHtmlEntities(string html)
    {
        return HtmlTagsMyRegex().Replace(html, string.Empty);
    }

    private static string AddSpacesWhenApplicable(string str)
    {
        var sb = new StringBuilder(str, str.Length * 2);
        foreach (var character in CharsRequiringTrailingSpace)
        {
            sb.Replace(character + " ", character.ToString())       // In case there is a character with a space after it, replace it with the character only
                .Replace(character.ToString(), character + " ");    // Replace the character with the character and a space    
        }

        return sb.ToString().TrimEnd();
    }

    private static string CleanTitle(string title)
    {
        var cleanedTitle = WebUtility.HtmlDecode(title.Trim());
        cleanedTitle = RemoveHtmlEntities(cleanedTitle);
        cleanedTitle = cleanedTitle.TrimEnd('.').Trim();

        return cleanedTitle;
    }

    [GeneratedRegex("<.*?>", RegexOptions.Compiled)]
    private static partial Regex HtmlTagsMyRegex();

    [GeneratedRegex("\\s+", RegexOptions.Compiled)]
    private static partial Regex WhitespaceRegex();
}