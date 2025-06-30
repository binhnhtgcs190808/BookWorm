using BookWorm.ConsoleApp.Comparers;
using BookWorm.ConsoleApp.Models;

namespace BookWorm.ConsoleApp.Strategies;

/// A strategy for sorting books by their genre.
public class SortByGenreStrategy : ISortStrategy
{
    public void Sort(List<Book> books)
    {
        ArgumentNullException.ThrowIfNull(books);
        // Uses the built-in List.Sort with a custom comparer for performance and simplicity.
        books.Sort(new BookPropertyComparer(b => b.Genre));
    }
}