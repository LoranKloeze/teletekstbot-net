using TeletekstBot.Domain.Entities;

namespace TeletekstBot.Tests.Domain;

[TestFixture]
public class PageTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void PageProperties_SetProperties_PropertiesAreSetCorrectly()
    {
        const int pageNumber = 123;
        const string title = "Page Title";
        const string body = "Page Body";

        var page = new Page
        {
            PageNumber = pageNumber,
            Title = title,
            Body = body
        };
        Assert.Multiple(() =>
        {
            Assert.That(page.PageNumber, Is.EqualTo(pageNumber));
            Assert.That(page.Title, Is.EqualTo(title));
            Assert.That(page.Body, Is.EqualTo(body));
        });
    }

    [Test]
    public void PageProperties_SetTitleAndBodyToNull_PropertiesAreSetToNull()
    {
        // Arrange
        const int pageNumber = 123;

        // Act
        var page = new Page
        {
            PageNumber = pageNumber,
            Title = "",
            Body = ""
        };
        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(page.PageNumber, Is.EqualTo(pageNumber));
            Assert.That(page.Title, Is.Empty);
            Assert.That(page.Body, Is.Empty);
        });
    }
}