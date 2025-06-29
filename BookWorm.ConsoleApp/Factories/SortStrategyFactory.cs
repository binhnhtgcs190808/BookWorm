using BookWorm.ConsoleApp.Strategies;

namespace BookWorm.ConsoleApp.Factories;

public static class SortStrategyFactory
{
    private static readonly Dictionary<string, Func<ISortStrategy>> _strategyMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "title", () => new SortByTitleStrategy() },
        { "author", () => new SortByAuthorStrategy() },
        { "genre", () => new SortByGenreStrategy() }
    };

    public static ISortStrategy CreateStrategy(string criteria)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(criteria);

        if (_strategyMap.TryGetValue(criteria, out var factory))
        {
            return factory();
        }

        throw new ArgumentException($"Invalid sort criteria: {criteria}", nameof(criteria));
    }

    public static IEnumerable<string> GetAvailableCriteria() => _strategyMap.Keys;
}