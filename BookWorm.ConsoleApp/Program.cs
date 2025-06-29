using BookWorm.ConsoleApp.Data;
using BookWorm.ConsoleApp.Services;
using BookWorm.ConsoleApp.UI;
using BookWorm.ConsoleApp.Utilities;

namespace BookWorm.ConsoleApp;

public static class Program
{
    public static void Main(string[] args)
    {
        IBookRepository repository = new CsvBookRepository();
        var service = new BookService(repository);

        while (true)
        {
            try
            {
                string rawPath = args.Length == 0 ? PromptForPath() : string.Join(" ", args);
                string filePath = FilePathHelper.Prepare(rawPath);

                service.LoadBooks(filePath);
                Console.WriteLine($"\nSuccessfully loaded {service.BookCount} books from:\n{filePath}\n");
                break;          // success
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\nPlease try again.\n");
                args = Array.Empty<string>();       // force prompt on next loop
            }
        }

        new ConsoleUI(service).Run();
    }

    private static string PromptForPath()
    {
        Console.WriteLine("Enter the path to your book data file (CSV or TXT).");
        Console.WriteLine("- Drag & drop, paste or type (quotes/spaces handled automatically)");
        Console.Write("File path: ");
        return Console.ReadLine() ?? string.Empty;
    }
}