using System.Diagnostics;

namespace ExtensionMethods
{
    public static class ListExtensions
    {
        public static List<T> Shuffle<T>(this List<T> list)
        {
            if (list.Count < 2) return list;

            Random rng = new();

            List<T> oldList = list;

            // Fisher-Yates shuffle algorithm to shuffle the list in place
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }

            if (oldList.SequenceEqual(list))
            {
                (list[0], list[1]) = (list[1], list[0]);
            }

            return list;
        }
    }
}
