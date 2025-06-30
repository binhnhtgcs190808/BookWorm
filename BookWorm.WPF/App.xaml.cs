using System.Windows;
using BookWorm.ConsoleApp.Data;
using BookWorm.ConsoleApp.Services;
using BookWorm.WPF.ViewModels;

namespace BookWorm.WPF;

/// Main application class for the WPF project.
public partial class App : Application
{
    /// Overrides the startup event to set up the application's dependencies (Dependency Injection).
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // This is the "Composition Root" of the application.
        // It's where the application's object graph is composed.
        IBookRepository repository = new CsvBookRepository();
        var bookService = new BookService(repository);
        var viewModel = new MainWindowViewModel(bookService);

        var mainWindow = new MainWindow
        {
            DataContext = viewModel
        };

        mainWindow.Show();
    }
}