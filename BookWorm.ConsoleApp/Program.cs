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

        new ConsoleUI(service).Run();
    }
}