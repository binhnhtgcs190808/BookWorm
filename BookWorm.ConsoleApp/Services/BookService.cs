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

    public IReadOnlyList<Book> GetBookList()
    {
        lock (_lockObject)
        {
            // Return a read-only wrapper to prevent external modification and avoid inefficient copying.
            return _books.AsReadOnly();
        }
    }

    public List<Book> SearchByAuthor(string query)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);
        // Delegate to the consolidated private search method
        return SearchBy(b => b.Author.Contains(query, StringComparison.OrdinalIgnoreCase));
    }

    public List<Book> SearchByTitle(string query)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);
        // Delegate to the consolidated private search method
        return SearchBy(b => b.Title.Contains(query, StringComparison.OrdinalIgnoreCase));
    }

    public void SortBooks(ISortStrategy strategy)
    {
        ArgumentNullException.ThrowIfNull(strategy);

        lock (_lockObject)
        {
            strategy.Sort(_books);
        }
    }
    
    private List<Book> SearchBy(Func<Book, bool> predicate)
    {
        lock (_lockObject)
        {
            return _books.Where(predicate).ToList();
        }
    }
}