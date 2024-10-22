using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classroom_Seating_Planner
{
    internal class ListActions
    {
        // Public method for shuffling lists
        public static List<string> Shuffle(List<string> list, Random rng)
        {
            List<string> newList = [.. list.OrderBy(item => rng.Next())];
            return newList;
        }
    }
}
