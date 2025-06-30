using BookWorm.ConsoleApp.Comparers;
using BookWorm.ConsoleApp.Factories;
using BookWorm.ConsoleApp.Models;
using BookWorm.ConsoleApp.Strategies;

namespace BookWorm.Tests;

[TestClass]
public class SortingTests
{
    private readonly List<Book> _books = new()
    {
        new Book { Title = "Zulu", Author = "Charlie", Genre = "Yankee", Publisher = "P", Height = 1 },
        new Book { Title = "Alpha", Author = "Bravo", Genre = "X-Ray", Publisher = "P", Height = 1 },
        new Book { Title = "Golf", Author = "Delta", Genre = "Whiskey", Publisher = "P", Height = 1 }
    };

    [TestMethod]
    public void BookPropertyComparer_ByTitle_ComparesCorrectly()
    {
        // Arrange
        var comparer = new BookPropertyComparer(b => b.Title);

        // Act
        _books.Sort(comparer);

        // Assert
        Assert.AreEqual("Alpha", _books[0].Title);
        Assert.AreEqual("Golf", _books[1].Title);
        Assert.AreEqual("Zulu", _books[2].Title);
    }

    [TestMethod]
    public void SortStrategyFactory_WithValidCriteria_CreatesCorrectStrategy()
    {
        // Arrange & Act
        var titleStrategy = SortStrategyFactory.CreateStrategy("title");
        var authorStrategy = SortStrategyFactory.CreateStrategy("AUTHOR"); // Test case-insensitivity
        var genreStrategy = SortStrategyFactory.CreateStrategy("Genre");

        // Assert
        Assert.IsInstanceOfType(titleStrategy, typeof(SortByTitleStrategy));
        Assert.IsInstanceOfType(authorStrategy, typeof(SortByAuthorStrategy));
        Assert.IsInstanceOfType(genreStrategy, typeof(SortByGenreStrategy));
    }

    [TestMethod]
    public void SortStrategyFactory_WithInvalidCriteria_ThrowsArgumentException()
    {
        // Arrange, Act & Assert
        Assert.ThrowsException<ArgumentException>(() => SortStrategyFactory.CreateStrategy("invalid"));
    }
}