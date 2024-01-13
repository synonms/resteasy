using Synonms.RestEasy.Testing.Extensions;

namespace Synonms.RestEasy.Testing;

public static class TestUriConverter
{
    public const string BasePath = "http://localhost/";

    public static string ToAbsolutePath(string relativePath) =>
        ToAbsolute(relativePath);

    public static Uri ToAbsoluteUri(string relativePath)
    {
        string absolutePath = ToAbsolute(relativePath);

        return absolutePath.ToAbsoluteUri();
    }
    
    private static string ToAbsolute(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath))
        {
            throw new ArgumentException("Invalid relative path.", nameof(relativePath));
        }
            
        string trimmedRelativePath = relativePath.Trim('/');
        
        return BasePath + trimmedRelativePath;
    }
}