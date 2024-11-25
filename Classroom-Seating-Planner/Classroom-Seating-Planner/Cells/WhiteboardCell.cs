﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classroom_Seating_Planner.Cells
{
    public class WhiteboardCell : Cell
    {
        public WhiteboardCell(int x, int y) : base(x, y, "whiteboard")
        {
            this.backgroundColor = System.Windows.Media.Brushes.WhiteSmoke;
            this.cellText = "Tavla";
        }
    }
}
