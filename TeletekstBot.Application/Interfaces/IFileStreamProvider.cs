namespace TeletekstBot.Application.Interfaces;

public interface IFileStreamProvider
{
    Stream CreateStream(string path, FileMode mode, FileAccess access);
}