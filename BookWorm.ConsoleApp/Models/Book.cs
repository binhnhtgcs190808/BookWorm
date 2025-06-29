namespace BookWorm.ConsoleApp.Models;

public class Book
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int Height { get; set; }
    public string Publisher { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"Title: {Title}\n  Author: {Author}\n  Genre: {Genre}\n  Height: {Height}p\n  Publisher: {Publisher}\n";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Book other) return false;

        return Title == other.Title &&
               Author == other.Author &&
               Genre == other.Genre &&
               Height == other.Height &&
               Publisher == other.Publisher;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Title, Author, Genre, Height, Publisher);
    }
}