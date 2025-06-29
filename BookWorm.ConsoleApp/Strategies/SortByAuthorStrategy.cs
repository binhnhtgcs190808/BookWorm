using BookWorm.ConsoleApp.Algorithms;
using BookWorm.ConsoleApp.Models;

namespace BookWorm.ConsoleApp.Strategies;

public class SortByAuthorStrategy : ISortStrategy
{
    public void Sort(List<Book> books)
    {
        ArgumentNullException.ThrowIfNull(books);

        if (books.Count <= 1) return;

        // Use optimized QuickSort for better performance
        QuickSorter.Sort(books, new BookAuthorComparer());
    }

    private class BookAuthorComparer : IComparer<Book>
    {
        public int Compare(Book? x, Book? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            return y is null ? 1 : string.Compare(x.Author, y.Author, StringComparison.OrdinalIgnoreCase);
        }
    }
}