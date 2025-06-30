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
        // FIX 1: Consistently throw ArgumentException for all invalid path cases
        // to match the expectation of the existing unit test.
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("The file path cannot be null or whitespace.", nameof(filePath));

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
            // FIX 2: Replaced the fragile Regex.Split with a more robust Regex.Matches.
            // This regex correctly handles quoted fields and prevents common parsing errors.
            // It matches: a quoted field OR a field not containing the delimiter.
            var regex = new Regex($"\"([^\"]*)\"|(?<=[{delimiter}]|^)([^{delimiter}]*)");
            var matches = regex.Matches(line);

            var values = matches
                .Select(m => m.Value.Trim(' ', '"'))
                .ToList();

            if (values.Count < ExpectedFieldCount) return null;

            return new Book
            {
                Title = values[0],
                Author = values[1],
                Genre = values[2],
                Publisher = values[3],
                Height = values.Count > 4 && int.TryParse(values[4], out var height) ? height : 0
            };
        }
        catch
        {
            // If any exception occurs during parsing, skip the malformed line by returning null.
            return null;
        }
    }
}