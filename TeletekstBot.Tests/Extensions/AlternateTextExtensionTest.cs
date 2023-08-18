using TeletekstBot.Infrastructure.Extensions;

namespace TeletekstBot.Tests.Extensions;

[TestFixture]
public class AlternateTextExtensionTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void AddSpacesWhenApplicable_GivenTextWithCharactersWithoutTrailingSpaces_ReturnsTextWithSpaces()
    {
        // Arrange
        var testString =
            "This is a text without spaces after certain characters.The spaces must be added,so let's try it:are we successful? Let's find out.";
        var expectedResult =
            "This is a text without spaces after certain characters. The spaces must be added, so let's try it: are we successful? Let's find out.";
        
        // Act
        var result = testString.AddSpacesWhenApplicable();
        
        Assert.That(result, Is.EqualTo(expectedResult));
    }
}