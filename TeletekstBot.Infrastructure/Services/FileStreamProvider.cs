using TeletekstBot.Application.Interfaces;

namespace TeletekstBot.Infrastructure.Services;

public class FileStreamProvider : IFileStreamProvider
{
#pragma warning disable CA1822
    public Stream CreateStream(string path, FileMode mode, FileAccess access)
#pragma warning restore CA1822
    {
        return new FileStream(path, mode, access);
    }
}