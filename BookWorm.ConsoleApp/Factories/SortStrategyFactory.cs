using BookWorm.ConsoleApp.Strategies;

namespace BookWorm.ConsoleApp.Factories;

/// A factory for creating
/// <see cref="ISortStrategy" />
/// instances based on a string key.
/// This decouples the client code from concrete strategy implementations.
public static class SortStrategyFactory
{
    private static readonly Dictionary<string, Func<ISortStrategy>> StrategyMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "title", () => new SortByTitleStrategy() },
        { "author", () => new SortByAuthorStrategy() },
        { "genre", () => new SortByGenreStrategy() }
    };


    /// Creates a sort strategy based on the provided criteria.
    /// <param name="criteria">The sort criteria (e.g., "title", "author"). Case-insensitive.</param>
    /// <returns>An instance of <see cref="ISortStrategy" />.</returns>
    /// <exception cref="ArgumentException">Thrown if the criteria is invalid.</exception>
    public static ISortStrategy CreateStrategy(string criteria)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(criteria);

        if (StrategyMap.TryGetValue(criteria, out var factory)) return factory();

        throw new ArgumentException($"Invalid sort criteria: {criteria}", nameof(criteria));
    }


    /// Gets a list of all available sort criteria keys.
    /// <returns>An enumerable collection of criteria strings.</returns>
    public static IEnumerable<string> GetAvailableCriteria()
    {
        return StrategyMap.Keys;
    }
}