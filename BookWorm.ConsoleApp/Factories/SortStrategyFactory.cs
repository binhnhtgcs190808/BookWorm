using BookWorm.ConsoleApp.Strategies;

namespace BookWorm.ConsoleApp.Factories;

public static class SortStrategyFactory
{
    public static ISortStrategy CreateStrategy(string criteria)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(criteria);

        return criteria.ToLowerInvariant() switch
        {
            "title" => new SortByTitleStrategy(),
            "author" => new SortByAuthorStrategy(),
            "genre" => new SortByGenreStrategy(),
            _ => throw new ArgumentException($"Invalid sort criteria: {criteria}", nameof(criteria))
        };
    }

    public static IEnumerable<string> GetAvailableCriteria()
    {
        return new[] { "title", "author", "genre" };
    }
}