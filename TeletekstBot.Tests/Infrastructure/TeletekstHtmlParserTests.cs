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
        const int expectedPageNumber = 110;
        teletekstHtmlParser.LoadHtml(html);
        
        // Act
        var result = teletekstHtmlParser.ToPage(expectedPageNumber);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<Page>());
            Assert.That(result.Title, Is.EqualTo(expectedTitle));
            Assert.That(result.Body, Does.StartWith(expectedBodyStart));
            Assert.That(result.Body, Does.EndWith(expectedBodyEnd));
            Assert.That(result.PageNumber, Is.EqualTo(expectedPageNumber));
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
        const int expectedPageNumber = 110;

        teletekstHtmlParser.LoadHtml(html);
        
        // Act
        var result = teletekstHtmlParser.ToPage(expectedPageNumber);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<Page>());
            Assert.That(result.Title, Is.EqualTo(expectedTitle));
            Assert.That(result.Body, Does.StartWith(expectedBodyStart));
            Assert.That(result.Body, Does.EndWith(expectedBodyEnd));
            Assert.That(result.PageNumber, Is.EqualTo(expectedPageNumber));
        });
    }
    
    [Test]
    public void ToPage_WithHtmlEntitiesInTitleReturnsCorrectPage()
    {
        // Arrange
        var teletekstHtmlParser = new TeletekstHtmlParser(new HtmlDocument());
        var html = MockFile.GetFileText("full_nos_page_110_with_html_entities_in_title.html");
        const string expectedTitle = "Miss Universe breekt met Indonesië";
        const int expectedPageNumber = 110;

        teletekstHtmlParser.LoadHtml(html);

        // Act
        var result = teletekstHtmlParser.ToPage(expectedPageNumber);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<Page>());
            Assert.That(result.Title, Is.EqualTo(expectedTitle));
            Assert.That(result.PageNumber, Is.EqualTo(expectedPageNumber));
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

    [Test]
    public void RelevantPages_ReturnsListOfPages()
    {
        var teletekstHtmlParser = new TeletekstHtmlParser(new HtmlDocument());
        var html = MockFile.GetFileText("full_nos_page_101.html");
        teletekstHtmlParser.LoadHtml(html);
        
        var expectedPages = new List<Page>
        {
            new () { PageNumber = 108, Title = "Yesilgöz officieel VVD-lijsttrekker" },
            new () { PageNumber = 104, Title = "Honderden cryptotelefoons gekraakt." },
            new () { PageNumber = 126, Title = "Junta Niger wil president vervolgen" },
            new () { PageNumber = 107, Title = "Minister Schreinemacher is zwanger." },
            new () { PageNumber = 106, Title = "Man verongelukt op N2 bij Eindhoven" },
            new () { PageNumber = 105, Title = "Kinderprogramma met Römer offline.." },
            new () { PageNumber = 136, Title = "Defecte attractie België:7 gewonden" },
            new () { PageNumber = 121, Title = "Tweede dode door ongeluk in Twente." },
            new () { PageNumber = 125, Title = "Minister:7 doden bij aanval Cherson" }
        };
        
        // Act
        var result = teletekstHtmlParser.RelevantPages();
        
        //Assert
        CollectionAssert.AreEqual(expectedPages, result);
    }
    
    [Test]
    public void RelevantPages_WithDoublePageNumbersReturnsUniqueListOfPages()
    {
        var teletekstHtmlParser = new TeletekstHtmlParser(new HtmlDocument());
        var html = MockFile.GetFileText("full_nos_page_101_with_double_numbers.html");
        teletekstHtmlParser.LoadHtml(html);
        
        var expectedPages = new List<Page>
        {
            new () { PageNumber = 108, Title = "Yesilgöz officieel VVD-lijsttrekker" },
            new () { PageNumber = 104, Title = "Honderden cryptotelefoons gekraakt." },
            new () { PageNumber = 126, Title = "Junta Niger wil president vervolgen" },
            new () { PageNumber = 107, Title = "Minister Schreinemacher is zwanger." },
            new () { PageNumber = 106, Title = "Man verongelukt op N2 bij Eindhoven" },
            new () { PageNumber = 105, Title = "Kinderprogramma met Römer offline.." },
            new () { PageNumber = 136, Title = "Defecte attractie België:7 gewonden" },
            new () { PageNumber = 125, Title = "Minister:7 doden bij aanval Cherson" }
        };
        
        // Act
        var result = teletekstHtmlParser.RelevantPages();
        
        //Assert
        CollectionAssert.AreEqual(expectedPages, result);
    }
}