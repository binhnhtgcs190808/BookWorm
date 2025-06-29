using BookWorm.ConsoleApp.Data;
using BookWorm.ConsoleApp.Models;
using BookWorm.ConsoleApp.Strategies;

namespace BookWorm.ConsoleApp.Services;

public class BookService
{
    private readonly IBookRepository _bookRepository;
    private readonly List<Book> _books;
    private readonly object _lockObject = new();

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        _books = new List<Book>();
    }

    public int BookCount
    {
        get
        {
            lock (_lockObject)
            {
                return _books.Count;
            }
        }
    }

    public void LoadBooks(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        try
        {
            var books = _bookRepository.LoadBooks(filePath).ToList();

            lock (_lockObject)
            {
                _books.Clear();
                _books.AddRange(books);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load books from file: {filePath}", ex);
        }
    }

    public List<Book> GetBookList()
    {
        lock (_lockObject)
        {
            // Return a copy to prevent the internal list from being modified externally (encapsulation).
            return new List<Book>(_books);
        }
    }

    public List<Book> SearchByAuthor(string query)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        lock (_lockObject)
        {
            return _books.Where(b => b.Author.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }

    public List<Book> SearchByTitle(string query)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        lock (_lockObject)
        {
            return _books.Where(b => b.Title.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }

    public void SortBooks(ISortStrategy strategy)
    {
        ArgumentNullException.ThrowIfNull(strategy);

        lock (_lockObject)
        {
            strategy.Sort(_books);
        }
    }
}