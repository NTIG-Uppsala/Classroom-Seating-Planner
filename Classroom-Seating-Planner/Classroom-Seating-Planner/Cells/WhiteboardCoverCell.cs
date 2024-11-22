using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classroom_Seating_Planner.cells
{
    public class WhiteboardCoverCell : Cell
    {
        public WhiteboardCoverCell(int x, int y, int width, int height) : base(x, y, "whiteboardCover", width, height)
        {
            this.backgroundColor = System.Windows.Media.Brushes.WhiteSmoke;
            this.cellText = "HELA TAVLAN :)";
        }
    }
}
