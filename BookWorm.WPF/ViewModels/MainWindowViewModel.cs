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

/// <summary>
///     ViewModel for the main window, handling UI logic and data binding.
/// </summary>
public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly BookService _bookService;

    private ObservableCollection<Book> _books = new();
    private string? _searchText;
    private Book? _selectedBook;
    private string _selectedSortCriteria = "Title";
    private string _statusMessage = "Ready. Please load a data file.";

    /// <summary>
    ///     Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
    /// </summary>
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

    /// <summary>
    ///     Gets the collection of books to display in the UI.
    /// </summary>
    public ObservableCollection<Book> Books
    {
        get => _books;
        private set => SetField(ref _books, value);
    }

    /// <summary>
    ///     Gets the list of available criteria for sorting.
    /// </summary>
    public List<string> SortCriteria { get; }

    /// <summary>
    ///     Gets or sets the currently selected sorting criteria from the UI.
    /// </summary>
    public string SelectedSortCriteria
    {
        get => _selectedSortCriteria;
        set => SetField(ref _selectedSortCriteria, value);
    }

    /// <summary>
    ///     Gets or sets the currently selected book in the DataGrid.
    /// </summary>
    public Book? SelectedBook
    {
        get => _selectedBook;
        set => SetField(ref _selectedBook, value);
    }

    /// <summary>
    ///     Gets or sets the text used for searching books.
    /// </summary>
    public string? SearchText
    {
        get => _searchText;
        set => SetField(ref _searchText, value);
    }

    /// <summary>
    ///     Gets or sets the message displayed in the status bar.
    /// </summary>
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetField(ref _statusMessage, value);
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
        // If search text is empty, show all books.
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

    /// <summary>
    ///     Refreshes the `Books` collection from the service. Creates a new ObservableCollection
    ///     for an efficient, single UI update.
    /// </summary>
    private void RefreshBookList()
    {
        Books = new ObservableCollection<Book>(_bookService.GetBookList());
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}