namespace UASATE.Core;

internal static class EnumerableExtension
{
    public static int ArgMin(this IList<double> list)
    {
        if (list.Count == 0) return -1;

        var minIndex = 0;
        for(int i = 0; i < list.Count; i++)
            if(list[i] < list[minIndex])
                minIndex = i;

        return minIndex;
    }

    public static int ArgMax(this IList<double> list)
    {
        if (list.Count == 0) return -1;

        var maxIndex = 0;
        for (int i = 0; i < list.Count; i++)
            if (list[i] > list[maxIndex])
                maxIndex = i;

        return maxIndex;
    }

    public static int ArgMinBy<T>(this IList<T> list, Func<T, double> selector)
    {
        if (list.Count == 0) return -1;

        var minIndex = 0;
        for (int i = 0; i < list.Count; i++)
            if (selector(list[i]) < selector(list[minIndex]))
                minIndex = i;

        return minIndex;
    }

    public static int ArgMaxBy<T>(this IList<T> list, Func<T, double> selector)
    {
        if (list.Count == 0) return -1;

        var maxIndex = 0;
        for (int i = 0; i < list.Count; i++)
            if (selector(list[i]) > selector(list[maxIndex]))
                maxIndex = i;

        return maxIndex;
    }
}
