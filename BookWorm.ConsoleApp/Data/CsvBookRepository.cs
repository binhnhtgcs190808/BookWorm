using System.Text;
using BookWorm.ConsoleApp.Models;

namespace BookWorm.ConsoleApp.Data;

public class CsvBookRepository : IBookRepository
{
    private const int ExpectedFieldCount = 5;

    public IEnumerable<Book> LoadBooks(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath)) throw new FileNotFoundException("The specified data file was not found.", filePath);

        return File.ReadLines(filePath, Encoding.UTF8)
            .Skip(1)
            .Select(line => ParseBookFromLine(line, DetectDelimiter(filePath)))
            .Where(book => book is not null)!; // Filter out nulls from failed parsing.
    }

    private static char DetectDelimiter(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        return string.Equals(extension, ".csv", StringComparison.OrdinalIgnoreCase) ? ',' : '|';
    }

    private static Book? ParseBookFromLine(string line, char delimiter)
    {
        if (string.IsNullOrWhiteSpace(line)) return null;

        try
        {
            var values = line.Split(delimiter);

            if (values.Length < ExpectedFieldCount) return null;

            // Trim whitespace and remove quotes in one go.
            for (var i = 0; i < values.Length; i++) values[i] = values[i].Trim(' ', '"');

            return new Book
            {
                Title = values[0],
                Author = values[1],
                Genre = values[2],
                Height = int.TryParse(values[3], out var height) ? height : 0,
                Publisher = values[4]
            };
        }
        catch
        {
            // Skip malformed lines by returning null.
            return null;
        }
    }
}