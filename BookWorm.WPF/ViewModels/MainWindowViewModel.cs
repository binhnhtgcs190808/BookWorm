using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BookWorm.ConsoleApp.Factories;
using BookWorm.ConsoleApp.Models;
using BookWorm.ConsoleApp.Services;
using BookWorm.WPF.Commands;
using Microsoft.Win32;

namespace BookWorm.WPF.ViewModels;

/// ViewModel for the main window, handling UI logic and data binding.
public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly BookService _bookService;

    private ObservableCollection<Book> _books = new();
    private string? _searchText;
    private Book? _selectedBook;
    private string _selectedSortCriteria = "Title";
    private string _statusMessage = "Ready. Please load a data file.";


    /// Initializes a new instance of the
    /// <see cref="MainWindowViewModel" />
    /// class.
    /// <param name="bookService">The book service dependency.</param>
    public MainWindowViewModel(BookService bookService)
    {
        _bookService = bookService;
        LoadCommand = new RelayCommand(LoadBooks);
        SearchCommand = new RelayCommand(SearchBooks);
        SortCommand = new RelayCommand(SortBooks, () => _bookService.BookCount > 0);

        // Populate sort criteria from the factory to ensure DRY principle.
        // Capitalize the first letter for user-friendly display in the ComboBox.
        SortCriteria = SortStrategyFactory.GetAvailableCriteria()
            .Select(c => char.ToUpper(c[0]) + c[1..])
            .ToList();
    }

    public ICommand LoadCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand SortCommand { get; }


    /// Gets the collection of books to display in the UI.

    public ObservableCollection<Book> Books
    {
        get => _books;
        private set => SetField(ref _books, value);
    }


    /// Gets the list of available criteria for sorting.

    public List<string> SortCriteria { get; }


    /// Gets or sets the currently selected sorting criteria from the UI.

    public string SelectedSortCriteria
    {
        get => _selectedSortCriteria;
        set => SetField(ref _selectedSortCriteria, value);
    }


    /// Gets or sets the currently selected book in the DataGrid.

    public Book? SelectedBook
    {
        get => _selectedBook;
        set => SetField(ref _selectedBook, value);
    }


    /// Gets or sets the text used for searching books.

    public string? SearchText
    {
        get => _searchText;
        set => SetField(ref _searchText, value);
    }


    /// Gets or sets the message displayed in the status bar.

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetField(ref _statusMessage, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void LoadBooks()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Book Data Files (*.csv, *.txt)|*.csv;*.txt|All Files (*.*)|*.*",
            Title = "Load Book Data"
        };

        if (openFileDialog.ShowDialog() != true) return;

        try
        {
            _bookService.LoadBooks(openFileDialog.FileName);
            RefreshBookList();
            StatusMessage = $"Successfully loaded {_bookService.BookCount} books from {openFileDialog.SafeFileName}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    private void SearchBooks()
    {
        // If the search text is empty, show all books.
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            RefreshBookList();
            StatusMessage = $"Displaying all {_bookService.BookCount} books.";
            return;
        }

        var results = _bookService.SearchBy(b =>
            b.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            b.Author.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
        );

        // Create a new collection from the results for the UI.
        Books = new ObservableCollection<Book>(results);
        StatusMessage = $"Found {Books.Count} book(s) matching your search.";
    }

    private void SortBooks()
    {
        try
        {
            // Convert UI criteria (e.g., "Title") to factory criteria (e.g., "title").
            var criteria = SelectedSortCriteria.ToLowerInvariant();
            var strategy = SortStrategyFactory.CreateStrategy(criteria);

            _bookService.SortBooks(strategy);
            RefreshBookList(); // Re-fetch the sorted list from the service.
            StatusMessage = $"Books sorted by {SelectedSortCriteria}.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error sorting books: {ex.Message}";
        }
    }


    /// Refreshes the `Books` collection from the service. Creates a new ObservableCollection
    /// for an efficient, single UI update.
    private void RefreshBookList()
    {
        Books = new ObservableCollection<Book>(_bookService.GetBookList());
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        OnPropertyChanged(propertyName);
    }
}