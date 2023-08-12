using HtmlAgilityPack;
using TeletekstBot.Domain.Entities;
using TeletekstBot.Infrastructure.Services;
// ReSharper disable StringLiteralTypo

namespace TeletekstBot.Tests.Infrastructure;

public class ParseIncomingPageTests
{
    [Test]
    public void ParseHtml_WithCorrectHtmlReturnsCorrectPage()
    {
        // Arrange
        var parseIncomingPage = new ParseIncomingPage(new HtmlDocument());
        var html = MockFile.GetFileText("page110.html");
        const string expectedTitle = "NS verliest alleenrecht buitenland";
        const string expectedBodyStart = "Het demissionaire kabinet wil naast NS ook andere";
        const string expectedBodyEnd = "twee stappen met nog 7 procent kan verhogen.";
        
        // Act
        var result = parseIncomingPage.ParseHtml(html);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<Page>());
            Assert.That(result.Title, Is.EqualTo(expectedTitle));
            Assert.That(result.Body, Does.StartWith(expectedBodyStart));
            Assert.That(result.Body, Does.EndWith(expectedBodyEnd));
        });
    }
    [Test]
    public void ParseHtml_WithWrongHtmlReturnsCorrectPage()
    {
        // Arrange
        var parseIncomingPage = new ParseIncomingPage(new HtmlDocument());
        const string html = "<span>Wrong html</span>";
        const string expectedTitle = "";
        const string expectedBodyStart = "";
        const string expectedBodyEnd = "";
        
        // Act
        var result = parseIncomingPage.ParseHtml(html);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<Page>());
            Assert.That(result.Title, Is.EqualTo(expectedTitle));
            Assert.That(result.Body, Does.StartWith(expectedBodyStart));
            Assert.That(result.Body, Does.EndWith(expectedBodyEnd));
        });
    }
}