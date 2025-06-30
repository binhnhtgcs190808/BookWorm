using BookWorm.ConsoleApp.Models;

namespace BookWorm.ConsoleApp.Data;

/// Defines the contract for a repository that loads book data.
public interface IBookRepository
{
    /// Loads a collection of books from a specified data source.
    /// <param name="filePath">The path to the data file.</param>
    /// <returns>An enumerable collection of <see cref="Book" /> objects.</returns>
    IEnumerable<Book> LoadBooks(string filePath);
}