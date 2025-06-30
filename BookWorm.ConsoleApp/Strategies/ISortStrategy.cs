using BookWorm.ConsoleApp.Models;

namespace BookWorm.ConsoleApp.Strategies;

/// Defines the contract for a book sorting strategy.
public interface ISortStrategy
{
    /// Sorts a list of books in-place.
    /// <param name="books">The list of books to sort.</param>
    void Sort(List<Book> books);
}