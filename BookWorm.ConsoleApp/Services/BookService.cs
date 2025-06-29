using BookWorm.ConsoleApp.Data;
using BookWorm.ConsoleApp.Models;
using BookWorm.ConsoleApp.Strategies;
using System.Collections.ObjectModel;

namespace BookWorm.ConsoleApp.Services;

public class BookService
{
    private readonly IBookRepository _bookRepository;
    private readonly List<Book> _books;
    private readonly object _lockObject = new();
    
    private string? _currentSortCriteria;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        _books = new List<Book>();
    }

    public int BookCount => _books.Count;

    public string? CurrentSortCriteria => _currentSortCriteria;

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
                _currentSortCriteria = null; // Reset sort criteria on new data load.
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load books from file: {filePath}", ex);
        }
    }
    
    public ReadOnlyCollection<Book> GetBookList()
    {
        lock (_lockObject)
        {
            return _books.AsReadOnly();
        }
    }

    public List<Book> SearchBy(Func<Book, bool> predicate)
    {
        lock (_lockObject)
        {
            return _books.Where(predicate).ToList();
        }
    }

    public void SortBooks(ISortStrategy strategy)
    {
        ArgumentNullException.ThrowIfNull(strategy);

        lock (_lockObject)
        {
            strategy.Sort(_books);
            
            _currentSortCriteria = strategy.GetType().Name.Replace("SortBy", "").Replace("Strategy", "").ToLowerInvariant();
        }
    }
}