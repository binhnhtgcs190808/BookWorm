using BookWorm.ConsoleApp.Data;
using BookWorm.ConsoleApp.Models;
using BookWorm.ConsoleApp.Services;
using BookWorm.ConsoleApp.Strategies;
using BookWorm.Tests.Mocks;

namespace BookWorm.Tests;

[TestClass]
public class BookServiceTests
{
    private List<Book> _testBooks = new List<Book>
    {
        new Book { Title = "The Hobbit", Author = "J.R.R. Tolkien", Genre = "Fantasy", Publisher = "Allen & Unwin", Height = 310 },
        new Book { Title = "Dune", Author = "Frank Herbert", Genre = "Sci-Fi", Publisher = "Chilton Books", Height = 412 },
        new Book { Title = "1984", Author = "George Orwell", Genre = "Dystopian", Publisher = "Secker & Warburg", Height = 328 },
        new Book { Title = "Brave New World", Author = "Aldous Huxley", Genre = "Dystopian", Publisher = "Chatto & Windus", Height = 288 }
    };

    [TestMethod]
    public void Constructor_WithNullRepository_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => new BookService(null!));
    }

    [TestMethod]
    public void LoadBooks_WithValidData_ShouldUpdateBookListAndCount()
    {
        // Arrange
        var repository = new MockBookRepository(_testBooks);
        var service = new BookService(repository);

        // Act
        service.LoadBooks("any_path.csv");

        // Assert
        Assert.AreEqual(4, service.BookCount);
        Assert.IsNull(service.CurrentSortCriteria, "Sort criteria should be reset after loading.");
        CollectionAssert.AreEqual(_testBooks, service.GetBookList().ToList());
    }
    
    [TestMethod]
    public void LoadBooks_WhenRepositoryThrowsException_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var repository = new MockBookRepository(throwException: true);
        var service = new BookService(repository);

        // Act & Assert
        var ex = Assert.ThrowsException<InvalidOperationException>(() => service.LoadBooks("failing_path.csv"));
        Assert.IsInstanceOfType(ex.InnerException, typeof(InvalidDataException));
    }

    [TestMethod]
    public void SearchBy_Title_ShouldReturnMatchingBooksCaseInsensitive()
    {
        // Arrange
        var repository = new MockBookRepository(_testBooks);
        var service = new BookService(repository);
        service.LoadBooks("any_path.csv");

        // Act
        var results = service.SearchBy(b => b.Title.Contains("hobbit", StringComparison.OrdinalIgnoreCase));

        // Assert
        Assert.AreEqual(1, results.Count);
        Assert.AreEqual("The Hobbit", results[0].Title);
    }
    
    [TestMethod]
    public void SearchBy_Author_WithNoMatches_ShouldReturnEmptyList()
    {
        // Arrange
        var repository = new MockBookRepository(_testBooks);
        var service = new BookService(repository);
        service.LoadBooks("any_path.csv");

        // Act
        var results = service.SearchBy(b => b.Author.Contains("Unknown Author"));

        // Assert
        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public void SortBooks_ByTitle_ShouldSortListCorrectly()
    {
        // Arrange
        var repository = new MockBookRepository(_testBooks);
        var service = new BookService(repository);
        service.LoadBooks("any_path.csv");
        var strategy = new SortByTitleStrategy();

        // Act
        service.SortBooks(strategy);
        var sortedBooks = service.GetBookList();
        
        // Assert
        Assert.AreEqual("title", service.CurrentSortCriteria);
        Assert.AreEqual("1984", sortedBooks[0].Title);
        Assert.AreEqual("Brave New World", sortedBooks[1].Title);
        Assert.AreEqual("Dune", sortedBooks[2].Title);
        Assert.AreEqual("The Hobbit", sortedBooks[3].Title);
    }
    
    [TestMethod]
    public void SortBooks_ByAuthor_ShouldSortListCorrectly()
    {
        // Arrange
        var repository = new MockBookRepository(_testBooks);
        var service = new BookService(repository);
        service.LoadBooks("any_path.csv");
        var strategy = new SortByAuthorStrategy();

        // Act
        service.SortBooks(strategy);
        var sortedBooks = service.GetBookList();
        
        // Assert
        Assert.AreEqual("author", service.CurrentSortCriteria);
        Assert.AreEqual("Aldous Huxley", sortedBooks[0].Author);
        Assert.AreEqual("Frank Herbert", sortedBooks[1].Author);
        Assert.AreEqual("George Orwell", sortedBooks[2].Author);
        Assert.AreEqual("J.R.R. Tolkien", sortedBooks[3].Author);
    }
    
    [TestMethod]
    public void GetBookList_ShouldReturnReadOnlyCollection()
    {
        // Arrange
        var repository = new MockBookRepository(_testBooks);
        var service = new BookService(repository);
        service.LoadBooks("any_path.csv");

        // Act
        var books = service.GetBookList();

        // Assert
        Assert.IsInstanceOfType(books, typeof(System.Collections.ObjectModel.ReadOnlyCollection<Book>));
    }
}