namespace BookWorm.ConsoleApp.Algorithms;

public static class QuickSorter
{
    private const int InsertionSortThreshold = 10;

    public static void Sort<T>(List<T> list, IComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(comparer);

        if (list.Count <= 1) return;

        QuickSortRecursive(list, 0, list.Count - 1, comparer);
    }

    private static void QuickSortRecursive<T>(List<T> list, int left, int right, IComparer<T> comparer)
    {
        while (left < right)
        {
            // Use insertion sort for small arrays
            if (right - left < InsertionSortThreshold)
            {
                InsertionSort(list, left, right, comparer);
                break;
            }

            var pivotIndex = Partition(list, left, right, comparer);

            // Recursively sort the smaller partition first (tail call optimization)
            if (pivotIndex - left < right - pivotIndex)
            {
                QuickSortRecursive(list, left, pivotIndex - 1, comparer);
                left = pivotIndex + 1;
            }
            else
            {
                QuickSortRecursive(list, pivotIndex + 1, right, comparer);
                right = pivotIndex - 1;
            }
        }
    }

    private static int Partition<T>(List<T> list, int left, int right, IComparer<T> comparer)
    {
        // Use median-of-three pivot selection for better performance
        var mid = left + (right - left) / 2;
        if (comparer.Compare(list[mid], list[left]) < 0)
            Swap(list, left, mid);
        if (comparer.Compare(list[right], list[left]) < 0)
            Swap(list, left, right);
        if (comparer.Compare(list[right], list[mid]) < 0)
            Swap(list, mid, right);

        Swap(list, mid, right);
        var pivotValue = list[right];

        var i = left - 1;
        for (var j = left; j < right; j++)
            if (comparer.Compare(list[j], pivotValue) <= 0)
            {
                i++;
                Swap(list, i, j);
            }

        Swap(list, i + 1, right);
        return i + 1;
    }

    private static void InsertionSort<T>(List<T> list, int left, int right, IComparer<T> comparer)
    {
        for (var i = left + 1; i <= right; i++)
        {
            var key = list[i];
            var j = i - 1;

            while (j >= left && comparer.Compare(list[j], key) > 0)
            {
                list[j + 1] = list[j];
                j--;
            }

            list[j + 1] = key;
        }
    }

    private static void Swap<T>(List<T> list, int i, int j)
    {
        if (i != j) (list[i], list[j]) = (list[j], list[i]);
    }
}