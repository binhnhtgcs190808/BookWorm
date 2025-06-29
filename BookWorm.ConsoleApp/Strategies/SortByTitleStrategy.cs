using BookWorm.ConsoleApp.Algorithms;
using BookWorm.ConsoleApp.Comparers;
using BookWorm.ConsoleApp.Models;

namespace BookWorm.ConsoleApp.Strategies;

public class SortByTitleStrategy : ISortStrategy
{
    public void Sort(List<Book> books)
    {
        ArgumentNullException.ThrowIfNull(books);

        if (books.Count <= 1) return;
        
        QuickSorter.Sort(books, new BookPropertyComparer(b => b.Title));
    }
}