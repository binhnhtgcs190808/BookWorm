using BookWorm.ConsoleApp.Algorithms;
using BookWorm.ConsoleApp.Models;

namespace BookWorm.ConsoleApp.Strategies;

public class SortByTitleStrategy : ISortStrategy
{
    public void Sort(List<Book> books)
    {
        ArgumentNullException.ThrowIfNull(books);

        if (books.Count <= 1) return;

        // Use optimized QuickSort for better performance
        QuickSorter.Sort(books, new BookTitleComparer());
    }

    private class BookTitleComparer : IComparer<Book>
    {
        public int Compare(Book? x, Book? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            return string.Compare(x.Title, y.Title, StringComparison.OrdinalIgnoreCase);
        }
    }
}