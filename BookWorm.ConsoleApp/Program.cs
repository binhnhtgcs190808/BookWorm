using BookWorm.ConsoleApp.Data;
using BookWorm.ConsoleApp.Services;
using BookWorm.ConsoleApp.UI;

IBookRepository repository = new CsvBookRepository();
var service = new BookService(repository);

new ConsoleUI(service).Run();