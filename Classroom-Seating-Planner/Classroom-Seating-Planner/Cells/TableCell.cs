﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classroom_Seating_Planner.Cells
{
    public class TableCell : Cell
    {
        public TableCell(int x, int y) : base(x, y, "table")
        {
            this.backgroundColor = System.Windows.Media.Brushes.LightGray;
            this.cellText = "";
        }
    }
}
