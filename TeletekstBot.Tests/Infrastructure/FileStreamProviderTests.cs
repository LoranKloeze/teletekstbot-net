using TeletekstBot.Infrastructure.Services;

namespace TeletekstBot.Tests.Infrastructure;

public class FileStreamProviderTests
{

    [Test]
    public void CreateStream_CreatesExpectedStream()
    {
        // Arrange
        var fileStreamProvider = new FileStreamProvider();
        var testFilePath = Path.GetTempFileName();
            
        // Act
        using (var stream = fileStreamProvider.CreateStream(testFilePath, FileMode.Open, FileAccess.Read))
        {
            // Assert
            Assert.That(stream, Is.Not.Null);
            Assert.That(stream.CanRead, Is.True);
        }

        // Clean up
        File.Delete(testFilePath);
    }
}