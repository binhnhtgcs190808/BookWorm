using BookWorm.ConsoleApp.Algorithms;
using BookWorm.ConsoleApp.Factories;
using BookWorm.ConsoleApp.Services;
using BookWorm.ConsoleApp.Strategies;
using BookWorm.ConsoleApp.Utilities;

namespace BookWorm.ConsoleApp.UI;

public class ConsoleUI(BookService bookService)
{
    private readonly BookService _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));

    public void Run()
    {
        Console.WriteLine("=== BookWorm Console Application ===\n");

        while (_bookService.BookCount == 0)
            try
            {
                LoadDataFile(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Please try again or press Ctrl+C to exit.\n");
            }

        ShowMainMenu();
    }

    private void LoadDataFile(bool isInitialLoad = false)
    {
        if (isInitialLoad) Console.WriteLine("Enter the path to your book data file (CSV or TXT).");
        Console.Write("File path: ");

        var rawInput = Console.ReadLine();
        var path = FilePathHelper.Prepare(rawInput ?? string.Empty);

        _bookService.LoadBooks(path);
        Console.WriteLine($"Successfully loaded {_bookService.BookCount} books.\n");
    }

    private void ShowMainMenu()
    {
        while (true)
        {
            Console.WriteLine("=== Main Menu ===");
            Console.WriteLine("1. List all books");
            Console.WriteLine("2. Search books (linear)");
            Console.WriteLine("3. Sort books");
            Console.WriteLine("4. Binary search by title");
            Console.WriteLine("5. Load different data file");
            Console.WriteLine("6. Exit");
            Console.Write("Choose an option (1-6): ");

            var choice = Console.ReadLine();
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case "1": ListAllBooks(); break;
                    case "2": SearchBooks(); break;
                    case "3": SortBooks(); break;
                    case "4": BinarySearchByTitle(); break;
                    case "5": LoadDataFile(); break;
                    case "6":
                        Console.WriteLine("Thank you for using BookWorm!");
                        return;
                    default: Console.WriteLine("Invalid option. Please choose 1-6."); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    private void ListAllBooks()
    {
        var books = _bookService.GetBookList();
        if (books.Count == 0)
        {
            Console.WriteLine("No books found.");
            return;
        }

        Console.WriteLine($"=== All Books ({books.Count}) ===");

        foreach (var book in books) Console.WriteLine(book);
    }

    private void SearchBooks()
    {
        Console.WriteLine("=== Search Books ===");
        Console.WriteLine("1. Search by title");
        Console.WriteLine("2. Search by author");
        Console.Write("Choose search type (1-2): ");

        var type = Console.ReadLine();
        Console.Write("Enter search query: ");
        var query = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(query))
        {
            Console.WriteLine("Search query cannot be empty.");
            return;
        }

        var results = type switch
        {
            "1" => _bookService.SearchBy(b => b.Author.Contains(query, StringComparison.OrdinalIgnoreCase)),
            "2" => _bookService.SearchBy(b => b.Title.Contains(query, StringComparison.OrdinalIgnoreCase)),
            _ => throw new ArgumentException("Invalid search type.")
        };

        if (results.Count == 0)
        {
            Console.WriteLine("No books found matching your search.");
            return;
        }

        Console.WriteLine($"=== Search Results ({results.Count}) ===");
        foreach (var result in results) Console.WriteLine(result);
    }

    private void SortBooks()
    {
        Console.WriteLine("=== Sort Books ===");
        var criteria = SortStrategyFactory.GetAvailableCriteria().ToList();
        for (var i = 0; i < criteria.Count; i++)
            Console.WriteLine($"{i + 1}. Sort by {char.ToUpper(criteria[i][0]) + criteria[i][1..]}");

        Console.Write($"Choose sort criteria (1-{criteria.Count}): ");
        if (!int.TryParse(Console.ReadLine(), out var index) || index < 1 || index > criteria.Count)
        {
            Console.WriteLine("Invalid choice.");
            return;
        }

        var selected = criteria[index - 1];
        _bookService.SortBooks(SortStrategyFactory.CreateStrategy(selected));

        Console.WriteLine($"Books sorted successfully by {selected}!");
    }

    private void BinarySearchByTitle()
    {
        // Check if the list is sorted by title. If not, inform the user and sort it.
        if (_bookService.CurrentSortCriteria != "title")
        {
            Console.WriteLine("Binary search requires the list to be sorted by title.");
            Console.WriteLine("Sorting the list by title now...");
            _bookService.SortBooks(new SortByTitleStrategy());
            Console.WriteLine("List sorted. Proceeding with search.\n");
        }

        Console.Write("Enter book title to search: ");
        var title = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("Title cannot be empty.");
            return;
        }

        var bookList = _bookService.GetBookList().ToList();
        var result = BinarySearcher.SearchByTitle(bookList, title);

        Console.WriteLine(result is null
            ? "Book not found."
            : $"=== Book Found ===\n{result}");
    }
}