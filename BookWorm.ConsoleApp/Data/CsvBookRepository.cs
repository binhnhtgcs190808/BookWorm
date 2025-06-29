using BookWorm.ConsoleApp.Models;

namespace BookWorm.ConsoleApp.Data;

public class CsvBookRepository : IBookRepository
{
    private const int ExpectedFieldCount = 5;
    private const int TitleIndex = 0;
    private const int AuthorIndex = 1;
    private const int GenreIndex = 2;
    private const int HeightIndex = 3;
    private const int PublisherIndex = 4;

    public IEnumerable<Book> LoadBooks(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath)) throw new FileNotFoundException("The specified data file was not found.", filePath);

        // Move the try-catch outside the iterator method
        return LoadBooksInternal(filePath);
    }

    private IEnumerable<Book> LoadBooksInternal(string filePath)
    {
        StreamReader? reader = null;

        try
        {
            // Decide delimiter based on file extension
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            var delimiter = extension == ".csv" ? ',' : '|';

            reader = new StreamReader(filePath);

            // Skip the header row
            if (!reader.EndOfStream) reader.ReadLine();

            while (!reader.EndOfStream)
            {
                string? line = null;

                try
                {
                    line = reader.ReadLine();
                }
                catch (Exception ex)
                {
                    throw new InvalidDataException($"Error reading book data from file: {filePath}", ex);
                }

                if (string.IsNullOrWhiteSpace(line)) continue;

                var book = ParseBookFromLine(line.AsSpan(), delimiter);
                if (book != null)
                    yield return book;
            }
        }
        finally
        {
            // Clean up resources in finally block
            reader?.Dispose();
        }
    }

    private Book? ParseBookFromLine(ReadOnlySpan<char> line, char delimiter)
    {
        try
        {
            var values = delimiter == ','
                ? ParseCsvLine(line)
                : ParseDelimitedLine(line, delimiter);

            if (values.Length < ExpectedFieldCount)
                return null;

            // Trim whitespace from all values
            for (var i = 0; i < values.Length; i++) values[i] = values[i].Trim();

            if (!int.TryParse(values[HeightIndex], out var height))
                height = 0;

            return new Book
            {
                Title = values[TitleIndex],
                Author = values[AuthorIndex],
                Genre = values[GenreIndex],
                Height = height,
                Publisher = values[PublisherIndex]
            };
        }
        catch
        {
            // Skip malformed lines
            return null;
        }
    }

    // Optimized CSV parsing using ReadOnlySpan
    private string[] ParseCsvLine(ReadOnlySpan<char> line)
    {
        var values = new List<string>();
        var inQuotes = false;
        var start = 0;

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    i++; // Skip escaped quote
                else
                    inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                var value = line[start..i].ToString();
                values.Add(UnescapeQuotes(value));
                start = i + 1;
            }
        }

        // Add the last value
        var lastValue = line[start..].ToString();
        values.Add(UnescapeQuotes(lastValue));

        return values.ToArray();
    }

    private string[] ParseDelimitedLine(ReadOnlySpan<char> line, char delimiter)
    {
        var values = new List<string>();
        var start = 0;

        for (var i = 0; i < line.Length; i++)
            if (line[i] == delimiter)
            {
                values.Add(line[start..i].ToString());
                start = i + 1;
            }

        // Add the last value
        values.Add(line[start..].ToString());

        return values.ToArray();
    }

    private static string UnescapeQuotes(string value)
    {
        if (value.StartsWith('"') && value.EndsWith('"')) value = value[1..^1]; // Remove surrounding quotes
        return value.Replace("\"\"", "\""); // Unescape double quotes
    }
}