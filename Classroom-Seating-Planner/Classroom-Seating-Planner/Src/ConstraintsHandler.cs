﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classroom_Seating_Planner.Src
{
    internal class ConstraintsHandler
    {
        public static double GetDistanceBetweenCells(Classroom_Seating_Planner.Cells.Cell cell1, Classroom_Seating_Planner.Cells.Cell cell2)
        {
            // Get the horizontal and vertical distance between the two cells
            double horizontalDistance = Math.Abs(cell1.x - cell2.x);
            double verticalDistance = Math.Abs(cell1.y - cell2.y);

            // Use the Pythagorean theorem to calculate the distance between the two cells
            return Math.Sqrt(Math.Pow(horizontalDistance, 2) + Math.Pow(verticalDistance, 2));
        }
    }
}
