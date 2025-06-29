using System.Diagnostics.CodeAnalysis;
using BookWorm.ConsoleApp.Models;

namespace BookWorm.ConsoleApp.Comparers;

public class BookPropertyComparer : IComparer<Book>
{
    private readonly Func<Book, string> _propertySelector;

    public BookPropertyComparer(Func<Book, string> propertySelector)
    {
        _propertySelector = propertySelector;
    }

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