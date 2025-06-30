using BookWorm.ConsoleApp.Data;

namespace BookWorm.Tests;

[TestClass]
public class CsvBookRepositoryTests
{
    private string? _tempCsvPath;
    private string? _tempTxtPath;

    [TestCleanup]
    public void Cleanup()
    {
        // Ensure temporary files are deleted after each test.
        if (_tempCsvPath != null && File.Exists(_tempCsvPath)) File.Delete(_tempCsvPath);
        if (_tempTxtPath != null && File.Exists(_tempTxtPath)) File.Delete(_tempTxtPath);
    }

    [TestMethod]
    public void LoadBooks_FromValidCsvFile_ShouldParseCorrectly()
    {
        // Arrange
        // FIX: Create a temporary file and give it the correct .csv extension
        _tempCsvPath = Path.ChangeExtension(Path.GetTempFileName(), ".csv");
        File.WriteAllText(_tempCsvPath,
            "Title,Author,Genre,Publisher,Height\n\"Dune\",\"Frank Herbert\",\"Sci-Fi\",\"Chilton Books\",412\n\"1984\",\"George Orwell\",\"Dystopian\",\"Secker & Warburg\",328");
        var repository = new CsvBookRepository();

        // Act
        var books = repository.LoadBooks(_tempCsvPath).ToList();

        // Assert
        Assert.AreEqual(2, books.Count);
        Assert.AreEqual("Dune", books[0].Title);
        Assert.AreEqual("Frank Herbert", books[0].Author);
        Assert.AreEqual(412, books[0].Height);
    }

    [TestMethod]
    public void LoadBooks_FromValidPipeDelimitedFile_ShouldParseCorrectly()
    {
        // Arrange
        _tempTxtPath = Path.ChangeExtension(Path.GetTempFileName(), ".txt");
        File.WriteAllText(_tempTxtPath,
            "Title|Author|Genre|Publisher|Height\nDune|Frank Herbert|Sci-Fi|Chilton Books|412");
        var repository = new CsvBookRepository();

        // Act
        var books = repository.LoadBooks(_tempTxtPath).ToList();

        // Assert
        Assert.AreEqual(1, books.Count);
        Assert.AreEqual("Dune", books[0].Title);
        Assert.AreEqual("Frank Herbert", books[0].Author);
    }

    [TestMethod]
    public void LoadBooks_WithMalformedLines_ShouldSkipThem()
    {
        // Arrange
        // FIX: Create a temporary file and give it the correct .csv extension
        _tempCsvPath = Path.ChangeExtension(Path.GetTempFileName(), ".csv");
        File.WriteAllText(_tempCsvPath,
            "Title,Author,Genre,Publisher,Height\nDune,Frank Herbert,Sci-Fi,Chilton Books,412\n1984,George Orwell,Dystopian"); // Last line is missing fields
        var repository = new CsvBookRepository();

        // Act
        var books = repository.LoadBooks(_tempCsvPath).ToList();

        // Assert
        Assert.AreEqual(1, books.Count, "Should skip the malformed line and load only the valid one.");
        Assert.AreEqual("Dune", books[0].Title);
    }

    [TestMethod]
    public void LoadBooks_FromNonExistentFile_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var repository = new CsvBookRepository();
        var nonExistentPath = "C:\\non_existent_file_12345.csv";

        // Act & Assert
        Assert.ThrowsException<FileNotFoundException>(() => repository.LoadBooks(nonExistentPath));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public void LoadBooks_WithInvalidPath_ShouldThrowArgumentException(string path)
    {
        // Arrange
        var repository = new CsvBookRepository();

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => repository.LoadBooks(path));
    }
}