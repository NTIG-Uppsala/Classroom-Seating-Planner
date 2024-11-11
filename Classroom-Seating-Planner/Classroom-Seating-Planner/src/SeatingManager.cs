﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Classroom_Seating_Planner.src
{
    internal class SeatingManager
    {
        public static void Populate(List<TextBlock> seatElements, List<string> classList)
        {
            // Ensure we don't exceed the number of available seats
            int seatCount = Math.Min(classList.Count, seatElements.Count);

            // Update the tables with the new order
            for (int index = 0; index < seatCount; index++)
            {
                // Assign the shuffled student name to the corresponding seat
                seatElements[index].Text = classList[index];
            }

            // If there are more seats than students, clear the remaining seats
            for (int index = seatCount; index < seatElements.Count; index++)
            {
                // Clear the seat if it's not occupied
                seatElements[index].Text = string.Empty;
            }
        }
    }
}
