using HtmlAgilityPack;
using TeletekstBot.Domain.Entities;
using TeletekstBot.Infrastructure.Services;
// ReSharper disable StringLiteralTypo

namespace TeletekstBot.Tests.Infrastructure;

public class TeletekstHtmlParserTests
{
    [Test]
    public void ToPage_WithCorrectHtmlReturnsCorrectPage()
    {
        // Arrange
        var teletekstHtmlParser = new TeletekstHtmlParser(new HtmlDocument());
        var html = MockFile.GetFileText("full_nos_page_110.html");
        const string expectedTitle = "NS verliest alleenrecht buitenland";
        const string expectedBodyStart = "Het demissionaire kabinet wil naast NS ook andere";
        const string expectedBodyEnd = "twee stappen met nog 7 procent kan verhogen.";
        teletekstHtmlParser.LoadHtml(html);
        
        // Act
        var result = teletekstHtmlParser.ToPage();
        
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
    public void ToPage_WithWrongHtmlReturnsCorrectPage()
    {
        // Arrange
        var teletekstHtmlParser = new TeletekstHtmlParser(new HtmlDocument());
        const string html = "<span>Wrong html</span>";
        const string expectedTitle = "";
        const string expectedBodyStart = "";
        const string expectedBodyEnd = "";
        teletekstHtmlParser.LoadHtml(html);
        
        // Act
        var result = teletekstHtmlParser.ToPage();
        
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
    public void ToPage_WithHtmlEntitiesInTitleReturnsCorrectPage()
    {
        // Arrange
        var teletekstHtmlParser = new TeletekstHtmlParser(new HtmlDocument());
        var html = MockFile.GetFileText("full_nos_page_110_with_html_entities_in_title.html");
        const string expectedTitle = "Miss Universe breekt met Indonesië";
        teletekstHtmlParser.LoadHtml(html);

        // Act
        var result = teletekstHtmlParser.ToPage();
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<Page>());
            Assert.That(result.Title, Is.EqualTo(expectedTitle));
        });
    }

    [Test]
    public void IsANewsPage_WithNewsHtmlReturnsTrue()
    {
        // Arrange
        var teletekstHtmlParser = new TeletekstHtmlParser(new HtmlDocument());
        var html = MockFile.GetFileText("full_nos_page_110.html");
        teletekstHtmlParser.LoadHtml(html);
        
        // Act
        var result = teletekstHtmlParser.IsANewsPage();
        
        // Assert
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void IsANewsPage_WithNonNewsHtmlReturnsFalse()
    {
        // Arrange
        var teletekstHtmlParser = new TeletekstHtmlParser(new HtmlDocument());
        var html = MockFile.GetFileText("full_nos_page_100.html");
        teletekstHtmlParser.LoadHtml(html);
        
        // Act
        var result = teletekstHtmlParser.IsANewsPage();
        
        // Assert
        Assert.That(result, Is.False);
    }
}