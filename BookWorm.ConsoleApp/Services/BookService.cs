using System.Collections.ObjectModel;
using BookWorm.ConsoleApp.Data;
using BookWorm.ConsoleApp.Models;
using BookWorm.ConsoleApp.Strategies;

namespace BookWorm.ConsoleApp.Services;

/// Acts as a central facade for managing book data, including loading, sorting, and searching.
public class BookService
{
    private readonly IBookRepository _bookRepository;
    private readonly List<Book> _books;
    private readonly object _lockObject = new();


    /// Initializes a new instance of the
    /// <see cref="BookService" />
    /// class.
    /// <param name="bookRepository">The repository to use for loading books.</param>
    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        _books = new List<Book>();
    }


    /// Gets the total number of books currently loaded.

    public int BookCount => _books.Count;


    /// Gets the criteria by which the book list is currently sorted. Returns null if the list is unsorted.

    public string? CurrentSortCriteria { get; private set; }


    /// Loads books from a data file, replacing any existing books.
    /// <param name="filePath">The path to the data file.</param>
    public void LoadBooks(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        try
        {
            var loadedBooks = _bookRepository.LoadBooks(filePath).ToList();
            lock (_lockObject)
            {
                _books.Clear();
                _books.AddRange(loadedBooks);
                CurrentSortCriteria = null; // Reset sort criteria on new data load.
            }
        }
        catch (Exception ex)
        {
            // Wrap the original exception for better context.
            throw new InvalidOperationException($"Failed to load books from file: {filePath}", ex);
        }
    }


    /// Returns a thread-safe, read-only view of the book list.
    public ReadOnlyCollection<Book> GetBookList()
    {
        lock (_lockObject)
        {
            return _books.AsReadOnly();
        }
    }


    /// Searches the book list based on a given predicate.
    /// <param name="predicate">The condition to filter books by.</param>
    /// <returns>A new list containing books that match the predicate.</returns>
    public List<Book> SearchBy(Func<Book, bool> predicate)
    {
        lock (_lockObject)
        {
            return _books.Where(predicate).ToList();
        }
    }


    /// Sorts the internal list of books using a specified strategy.
    /// <param name="strategy">The sorting strategy to apply.</param>
    public void SortBooks(ISortStrategy strategy)
    {
        ArgumentNullException.ThrowIfNull(strategy);

        lock (_lockObject)
        {
            strategy.Sort(_books);

            // Update the current sort criteria based on the strategy's type name.
            CurrentSortCriteria =
                strategy.GetType().Name.Replace("SortBy", "").Replace("Strategy", "").ToLowerInvariant();
        }
    }
}