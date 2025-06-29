using BookWorm.ConsoleApp.Algorithms;
using BookWorm.ConsoleApp.Models;

namespace BookWorm.ConsoleApp.Strategies;

public class SortByGenreStrategy : ISortStrategy
{
    public void Sort(List<Book> books)
    {
        ArgumentNullException.ThrowIfNull(books);

        if (books.Count <= 1)
            return;

        QuickSorter.Sort(books, new BookGenreComparer());
    }

    private class BookGenreComparer : IComparer<Book>
    {
        public int Compare(Book? x, Book? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;
            return string.Compare(x.Genre, y.Genre, StringComparison.OrdinalIgnoreCase);
        }
    }
}