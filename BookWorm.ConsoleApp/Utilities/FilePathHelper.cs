namespace BookWorm.ConsoleApp.Utilities;

/// Provides helper methods for cleaning and validating file paths.
public static class FilePathHelper
{
    /// Cleans, normalizes, and validates a raw file path string.
    /// <param name="rawPath">The raw path input from the user.</param>
    /// <returns>A full, validated file path.</returns>
    /// <exception cref="ArgumentException">Thrown if the path is empty.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the file does not exist at the path.</exception>
    public static string Prepare(string rawPath)
    {
        var cleaned = Clean(rawPath);

        if (string.IsNullOrWhiteSpace(cleaned)) throw new ArgumentException("No file path provided.", nameof(rawPath));

        // Convert to a full, absolute path.
        cleaned = Path.GetFullPath(cleaned);

        if (!File.Exists(cleaned)) throw new FileNotFoundException($"File not found: {cleaned}", cleaned);

        return cleaned;
    }


    /// Trims whitespace and surrounding quotes from a string.
    private static string Clean(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        return input.Trim().Trim('"', '\'');
    }
}