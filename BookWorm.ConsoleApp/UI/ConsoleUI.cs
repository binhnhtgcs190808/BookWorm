using BookWorm.ConsoleApp.Algorithms;
using BookWorm.ConsoleApp.Factories;
using BookWorm.ConsoleApp.Services;
using BookWorm.ConsoleApp.Utilities;

namespace BookWorm.ConsoleApp.UI;

public class ConsoleUI(BookService bookService)
{
    private readonly BookService _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));

    public void Run()
    {
        Console.WriteLine("=== BookWorm Console Application ===\n");

        try
        {
            if (_bookService.BookCount == 0) LoadDataFile();
            ShowMainMenu();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Application error: {ex.Message}");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    private void LoadDataFile()
    {
        Console.WriteLine("Enter the path to your book data file (CSV or TXT).");
        Console.WriteLine("- Drag & drop, paste or type (quotes/spaces handled automatically)");
        Console.Write("File path (default 'books.csv'): ");

        var raw    = Console.ReadLine();
        var path   = string.IsNullOrWhiteSpace(raw) ? "books.csv" : FilePathHelper.Prepare(raw!);

        _bookService.LoadBooks(path);
        Console.WriteLine($"Successfully loaded {_bookService.BookCount} books.\n");
    }

    private void ShowMainMenu()
    {
        while (true)
        {
            Console.WriteLine("=== Main Menu ===");
            Console.WriteLine("1. List all books");
            Console.WriteLine("2. Search books");
            Console.WriteLine("3. Sort books");
            Console.WriteLine("4. Binary search by title (requires sorting by title first)");
            Console.WriteLine("5. Load different data file");
            Console.WriteLine("6. Exit");
            Console.Write("Choose an option (1-6): ");

            var choice = Console.ReadLine();
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case "1": ListAllBooks();         break;
                    case "2": SearchBooks();          break;
                    case "3": SortBooks();            break;
                    case "4": BinarySearchBooks();    break;
                    case "5": LoadDataFile();         break;
                    case "6": Console.WriteLine("Thank you for using BookWorm!"); return;
                    default : Console.WriteLine("Invalid option. Please choose 1-6."); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    private void ListAllBooks()
    {
        var books = _bookService.GetBookList();
        if (books.Count == 0) { Console.WriteLine("No books found."); return; }

        Console.WriteLine($"=== All Books ({books.Count}) ===");
        for (var i = 0; i < books.Count; i++) Console.WriteLine($"{i + 1}. {books[i]}");
    }

    private void SearchBooks()
    {
        Console.WriteLine("=== Search Books ===");
        Console.WriteLine("1. Search by title");
        Console.WriteLine("2. Search by author");
        Console.Write("Choose search type (1-2): ");

        var type  = Console.ReadLine();
        Console.Write("Enter search query: ");
        var query = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(query)) { Console.WriteLine("Search query cannot be empty."); return; }

        var results = type switch
        {
            "1" => _bookService.SearchByTitle(query),
            "2" => _bookService.SearchByAuthor(query),
            _   => throw new ArgumentException("Invalid search type.")
        };

        if (results.Count == 0) { Console.WriteLine("No books found matching your search."); return; }

        Console.WriteLine($"=== Search Results ({results.Count}) ===");
        for (var i = 0; i < results.Count; i++) Console.WriteLine($"{i + 1}. {results[i]}");
    }

    private void SortBooks()
    {
        Console.WriteLine("=== Sort Books ===");
        var criteria = SortStrategyFactory.GetAvailableCriteria().ToList();
        for (var i = 0; i < criteria.Count; i++) Console.WriteLine($"{i + 1}. Sort by {criteria[i]}");

        Console.Write($"Choose sort criteria (1-{criteria.Count}): ");
        if (!int.TryParse(Console.ReadLine(), out var index) || index < 1 || index > criteria.Count)
        {
            Console.WriteLine("Invalid choice."); return;
        }

        var selected = criteria[index - 1];
        _bookService.SortBooks(SortStrategyFactory.CreateStrategy(selected));

        Console.WriteLine("Books sorted successfully!");
    }

    private void BinarySearchBooks()
    {
        Console.Write("Enter book title to search: ");
        var title = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(title)) { Console.WriteLine("Title cannot be empty."); return; }

        var result = BinarySearcher.SearchByTitle(_bookService.GetBookList(), title);
        Console.WriteLine(result is null
            ? "Book not found. Make sure the list is sorted by title first."
            : $"=== Book Found ===\n{result}");
    }
}
