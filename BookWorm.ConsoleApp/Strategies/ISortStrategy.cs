using BookWorm.ConsoleApp.Models;

namespace BookWorm.ConsoleApp.Strategies;

public interface ISortStrategy
{
    void Sort(List<Book> books);
}