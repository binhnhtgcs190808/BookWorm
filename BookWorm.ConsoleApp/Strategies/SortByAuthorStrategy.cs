using BookWorm.ConsoleApp.Comparers;
using BookWorm.ConsoleApp.Models;

namespace BookWorm.ConsoleApp.Strategies;

public class SortByAuthorStrategy : ISortStrategy
{
    public void Sort(List<Book> books)
    {
        ArgumentNullException.ThrowIfNull(books);

        books.Sort(new BookPropertyComparer(b => b.Author));
    }
}