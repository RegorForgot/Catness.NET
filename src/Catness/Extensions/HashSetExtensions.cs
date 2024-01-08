namespace Catness.Extensions;

public static class HashSetExtensions
{
    public static void TryAddAll<T>(this HashSet<T> hashSet, IEnumerable<T> itemsToAdd)
    {
        foreach (T var in itemsToAdd)
        {
            hashSet.Add(var);
        }
    }
}