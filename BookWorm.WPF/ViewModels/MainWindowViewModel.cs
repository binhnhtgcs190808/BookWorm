using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BookWorm.ConsoleApp.Data;
using BookWorm.ConsoleApp.Models;
using BookWorm.ConsoleApp.Services;
using BookWorm.ConsoleApp.Strategies;
using BookWorm.WPF.Commands;
using Microsoft.Win32;

namespace BookWorm.WPF.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly BookService _bookService;
    private string? _searchText;
    private Book? _selectedBook;
    private string _statusMessage = "Ready";

    public MainWindowViewModel()
    {
        _bookService = new BookService(new CsvBookRepository());
        LoadCommand = new RelayCommand(LoadBooks);
        SearchCommand = new RelayCommand(SearchBooks);
        SortCommand = new RelayCommand(SortBooks);
    }

    public ObservableCollection<Book> Books { get; } = new();
    public List<string> SortCriteria { get; } = new() { "Title", "Author", "Genre" };
    public string SelectedSortCriteria { get; set; } = "Title";

    public Book? SelectedBook
    {
        get => _selectedBook;
        set => SetField(ref _selectedBook, value);
    }

    public string? SearchText
    {
        get => _searchText;
        set => SetField(ref _searchText, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetField(ref _statusMessage, value);
    }

    public ICommand LoadCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand SortCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void LoadBooks()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Book Data|*.csv;*.txt|All Files|*.*",
            Title = "Load Book Data"
        };

        if (openFileDialog.ShowDialog() == true)
            try
            {
                _bookService.LoadBooks(openFileDialog.FileName);
                RefreshBookList();
                StatusMessage = $"Loaded {Books.Count} books from {openFileDialog.SafeFileName}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
    }

    private void SearchBooks()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            RefreshBookList();
            return;
        }

        var results = _bookService.SearchBy(b =>
            b.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            b.Author.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
        );

        Books.Clear();
        foreach (var book in results)
            Books.Add(book);

        StatusMessage = $"Found {results.Count} matching books";
    }

    private void SortBooks()
    {
        ISortStrategy strategy = SelectedSortCriteria switch
        {
            "Title" => new SortByTitleStrategy(),
            "Author" => new SortByAuthorStrategy(),
            "Genre" => new SortByGenreStrategy(),
            _ => throw new InvalidOperationException("Invalid sort criteria")
        };

        _bookService.SortBooks(strategy);
        RefreshBookList();
        StatusMessage = $"Books sorted by {SelectedSortCriteria}";
    }

    private void RefreshBookList()
    {
        Books.Clear();
        foreach (var book in _bookService.GetBookList())
            Books.Add(book);
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