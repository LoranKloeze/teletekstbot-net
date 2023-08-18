namespace TeletekstBot.Infrastructure.Extensions;

public static class AlternateTextExtension
{
    // Define the characters that need a space afterwards
    private const string AddSpacesAfterChars = ".,:";
    
    public static string AddSpacesWhenApplicable(this string str)
    {
        foreach (var character in AddSpacesAfterChars)
        {
            str = str.Replace(character + " ", character.ToString())    // In case there is a character with a space after it, replace it with the character only
                .Replace(character.ToString(), character + " ");        // Replace the character with the character and a space    
        }

        return str.TrimEnd();
    }
}