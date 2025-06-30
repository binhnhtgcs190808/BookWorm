using BookWorm.ConsoleApp.Data;
using BookWorm.ConsoleApp.Services;
using BookWorm.ConsoleApp.UI;

// This is the Composition Root for the Console Application.
// It's responsible for creating and wiring up the application's dependencies.
IBookRepository repository = new CsvBookRepository();
var service = new BookService(repository);
var consoleUI = new ConsoleUI(service);

consoleUI.Run();