using BookWorm.ConsoleApp.Models;

namespace BookWorm.ConsoleApp.Data;

public interface IBookRepository
{
    IEnumerable<Book> LoadBooks(string filePath);
}