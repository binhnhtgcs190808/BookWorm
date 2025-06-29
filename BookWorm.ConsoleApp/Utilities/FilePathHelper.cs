namespace BookWorm.ConsoleApp.Utilities;

public static class FilePathHelper
{
    public static string Prepare(string rawPath)
    {
        var cleaned = Clean(rawPath);

        if (string.IsNullOrWhiteSpace(cleaned)) throw new ArgumentException("No file path provided.", nameof(rawPath));

        cleaned = Path.GetFullPath(cleaned);

        if (!File.Exists(cleaned)) throw new FileNotFoundException($"File not found: {cleaned}", cleaned);

        return cleaned;
    }

    private static string Clean(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        return input.Trim().Trim('"', '\'');
    }
}