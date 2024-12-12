using System.Windows;

namespace Classroom_Seating_Planner.Cells
{
    public class WhiteboardCoverCell : Cell
    {
        public WhiteboardCoverCell(int gridX, int gridY, int width, int height) : base(gridX, gridY, "whiteboardCover", width, height)
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
