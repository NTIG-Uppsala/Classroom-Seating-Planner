using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Classroom_Seating_Planner.Cells
{
    public class WhiteboardCoverCell : Cell
    {
        public WhiteboardCoverCell(int x, int y, int width, int height) : base(x, y, "whiteboardCover", width, height)
        {
            this.backgroundColor = System.Windows.Media.Brushes.WhiteSmoke;
            this.cellText = "TAVLA";
        }

        public override void Style(System.Windows.Controls.Border cellElementContainer, System.Windows.Controls.TextBlock cellElement)
        {
            // Center the whiteboard text
            cellElement.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            cellElement.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            cellElement.TextAlignment = System.Windows.TextAlignment.Center;
            cellElement.FontWeight = FontWeights.SemiBold;
            cellElement.FontSize = 32;
            cellElement.TextWrapping = TextWrapping.Wrap;
        }
    }
}
