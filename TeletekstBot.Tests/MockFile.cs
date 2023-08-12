using System.Diagnostics;

namespace TeletekstBot.Tests;

public static class MockFile
{
    public static string GetFileText(string mockFile)
    {
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        
        var projectDirectory = Directory.GetParent(currentDirectory)?.Parent?.Parent?.Parent?.FullName;
        Debug.Assert(projectDirectory != null);
        
        var filePath = Path.Combine(projectDirectory, "MockFiles", mockFile);
        return File.ReadAllText(filePath);
    }
}