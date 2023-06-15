namespace BlazrCountdownApp.Data;

internal static class Extensions
{
    const int cNumberTileCount = 6;

    public static int CountOf<T>(this IList<T> list, Func<T, bool> predicate)
    {
        int count = 0;

        foreach (T item in list)
        {
            if (predicate(item))
                count++;
        }

        return count;
    }



    public static IEnumerable<int>GetSixRandomTiles(this Random random)
    {
        int[] largeTiles = { 25, 50, 75, 100 };
        int[] smallTiles = { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10 };
        int[] output = new int[cNumberTileCount];

        //randomize number of large tiles:
        double x = random.NextDouble();

        int largeTileCount = x switch
        {
            var val when val < 0.05 => 4,
            var val when val < 0.1 => 3,
            var val when val < 0.15 => 2,
            var val when val < 0.95 => 1, //most likely to return single large number 80% of the time.
            _ => 0
        };

        int smallTileCount = cNumberTileCount - largeTileCount;
        int tileIndex = 0;

        if (largeTileCount > 0)
        {
            largeTiles.Shuffle();

            for (int index = 0; index < largeTileCount; index++)
                output[tileIndex++] = largeTiles[index];
        }

        smallTiles.Shuffle();

        for (int index = 0; index < smallTileCount; index++)
            output[tileIndex++] = smallTiles[index];

        return output.OrderByDescending(x => x).ToList();
    }



    private static IList<T> Shuffle<T>(this IList<T> list)
    {
        if (list.Count > 1)
        {
            Random random = new Random();

            for (int index = list.Count - 1; index > 0; --index)
                Swap(list, index, random.Next(index + 1));
        }

        return list;
    }



    private static void Swap<T>(IList<T> list, int i1, int i2)
    {
        T temp = list[i1];
        list[i1] = list[i2];
        list[i2] = temp;
    }

}

