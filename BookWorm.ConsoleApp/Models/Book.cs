namespace BookWorm.ConsoleApp.Models;

public class Book
{
    public required string Title { get; init; }
    public required string Author { get; init; }
    public required string Genre { get; init; }
    public int Height { get; init; }
    public required string Publisher { get; init; }

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