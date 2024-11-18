using System.Diagnostics;

namespace ExtensionMethods
{
    // Methods that extend the List class
    public static class ListExtensions
    {
        public static List<T> Shuffle<T>(this List<T> list)
        {
            // If the list is empty or has only one element, return the list as is
            if (list.Count < 2) return list;

            Random rng = new();

            // Store the list before shuffling for comparison
            List<T> oldList = list;

            // Fisher-Yates shuffle algorithm to shuffle the list in place
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }

            // If the list is the same as before shuffling, swap the first two elements
            if (oldList.SequenceEqual(list))
            {
                (list[0], list[1]) = (list[1], list[0]);
            }

            return list;
        }
    }
}
