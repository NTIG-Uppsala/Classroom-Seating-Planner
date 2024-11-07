using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using System.IO;
using FlaUIElement = FlaUI.Core.AutomationElements;

namespace Classroom_Seating_Planner.src
{
    public class ListActions
    {
        // Returns a shuffled list from the list that is passed
        public static List<string> Shuffle(List<string> list)
        {
            // Can't shuffle 0 or 1 elements
            if (list.Count < 2) return list;

            Random rng = new();
            List<string> newList = list.OrderBy(item => rng.Next()).ToList();

            // If the list is the same as the original, swap the first two elements to ensure a new order
            if (newList.SequenceEqual(list))
            {
                (newList[0], newList[1]) = (newList[1], newList[0]);
            }
            return newList;
        }
    }
}
