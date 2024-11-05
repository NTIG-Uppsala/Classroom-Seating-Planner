using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using System.IO;
using FlaUIElement = FlaUI.Core.AutomationElements;

namespace Classroom_Seating_Planner.src
{
    public class ListActions
    {
        // Public method for shuffling lists
        public static List<string> Shuffle(List<string> list)
        {
            // Can't shuffle 0 or 1 elements
            if (list.Count < 2) return list;

            Random rng = new();
            List<string> newList = list;

            // Shuffle until the list has a new order
            while (newList.SequenceEqual(list))
            {
                newList = [.. list.OrderBy(item => rng.Next())];
            }
            return newList;
        }
    }
}
