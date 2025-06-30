using BookWorm.ConsoleApp.Models;

namespace BookWorm.ConsoleApp.Comparers;

/// A generic comparer for the
/// <see cref="Book" />
/// class that compares based on a selected string property.
public class BookPropertyComparer : IComparer<Book>
{
    private readonly Func<Book, string> _propertySelector;


    /// Initializes a new instance of the
    /// <see cref="BookPropertyComparer" />
    /// class.
    /// <param name="propertySelector">A function to select the string property to compare (e.g., b => b.Title).</param>
    public BookPropertyComparer(Func<Book, string> propertySelector)
    {
        _propertySelector = propertySelector;
    }


    /// Compares two books based on the selected property. The comparison is case-insensitive.
    public int Compare(Book? x, Book? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        var valueX = _propertySelector(x);
        var valueY = _propertySelector(y);

        return string.Compare(valueX, valueY, StringComparison.OrdinalIgnoreCase);
    }
}