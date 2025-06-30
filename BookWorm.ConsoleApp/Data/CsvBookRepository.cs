using System.Text;
using System.Text.RegularExpressions;
using BookWorm.ConsoleApp.Models;

namespace BookWorm.ConsoleApp.Data;

/// A repository for loading book data from CSV or delimiter-separated text files.
public class CsvBookRepository : IBookRepository
{
    private const int ExpectedFieldCount = 5;

    /// <inheritdoc />
    public IEnumerable<Book> LoadBooks(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath)) throw new FileNotFoundException("The specified data file was not found.", filePath);

        // Read all lines, skip the header, and parse each line into a Book object.
        // Malformed lines will be filtered out (where ParseBookFromLine returns null).
        return File.ReadLines(filePath, Encoding.UTF8)
            .Skip(1)
            .Select(line => ParseBookFromLine(line, DetectDelimiter(filePath)))
            .Where(book => book is not null)!;
    }


    /// Detects the delimiter based on the file extension.
    private static char DetectDelimiter(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        return string.Equals(extension, ".csv", StringComparison.OrdinalIgnoreCase) ? ',' : '|';
    }


    /// Parses a single line from the data file into a
    /// <see cref="Book" />
    /// object.
    /// <returns>A <see cref="Book" /> object, or null if the line is malformed.</returns>
    private static Book? ParseBookFromLine(string line, char delimiter)
    {
        if (string.IsNullOrWhiteSpace(line)) return null;

        try
        {
            // This regex splits by the delimiter, but correctly ignores delimiters that are inside double quotes.
            var regex = new Regex($"{delimiter}(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            var values = regex.Split(line);

            if (values.Length < ExpectedFieldCount) return null;

            // Clean up each value by trimming whitespace and surrounding quotes.
            for (var i = 0; i < values.Length; i++) values[i] = values[i].Trim(' ', '"');

            return new Book
            {
                Title = values[0],
                Author = values[1],
                Genre = values[2],
                Publisher = values[3],
                Height = values.Length > 4 && int.TryParse(values[4], out var height) ? height : 0
            };
        }
        catch
        {
            // If any exception occurs during parsing, skip the malformed line by returning null.
            return null;
        }
    }
}