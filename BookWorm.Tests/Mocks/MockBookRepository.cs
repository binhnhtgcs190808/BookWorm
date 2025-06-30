using BookWorm.ConsoleApp.Data;
using BookWorm.ConsoleApp.Models;

namespace BookWorm.Tests.Mocks;

/// <summary>
/// A mock implementation of IBookRepository for testing purposes.
/// It allows simulating different data loading scenarios without file I/O.
/// </summary>
public class MockBookRepository : IBookRepository
{
    private readonly List<Book>? _booksToReturn;
    private readonly bool _throwException;

    /// <summary>
    /// Initializes a new instance of the mock repository.
    /// </summary>
    /// <param name="booksToReturn">The list of books to return when LoadBooks is called.
    /// If null, an empty list is returned.</param>
    /// <param name="throwException">If true, the mock will throw an exception when LoadBooks is called.</param>
    public MockBookRepository(List<Book>? booksToReturn = null, bool throwException = false)
    {
        _booksToReturn = booksToReturn;
        _throwException = throwException;
    }

    /// <summary>
    /// Simulates loading books, returning the predefined list or throwing an exception.
    /// </summary>
    public IEnumerable<Book> LoadBooks(string filePath)
    {
        if (_throwException)
        {
            throw new InvalidDataException("Simulated repository failure.");
        }

        return _booksToReturn ?? new List<Book>();
    }
}