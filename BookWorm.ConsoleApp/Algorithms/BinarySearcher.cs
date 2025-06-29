using BookWorm.ConsoleApp.Models;

namespace BookWorm.ConsoleApp.Algorithms;

public static class BinarySearcher
{
    public static Book? SearchByTitle(List<Book> sortedBooks, string title)
    {
        ArgumentNullException.ThrowIfNull(sortedBooks);

        if (string.IsNullOrWhiteSpace(title))
            return null;

        var left = 0;
        var right = sortedBooks.Count - 1;

        while (left <= right)
        {
            var mid = left + (right - left) / 2;
            var comparison = string.Compare(sortedBooks[mid].Title, title, StringComparison.OrdinalIgnoreCase);

            if (comparison == 0) return sortedBooks[mid]; // Found
            if (comparison < 0)
                left = mid + 1; // Search right half
            else
                right = mid - 1; // Search left half
        }

        return null; // Not found
    }
}