namespace Classroom_Seating_Planner.Cells
{
    public class WhiteboardCell : Cell
    {
        public WhiteboardCell(int gridX, int gridY) : base(gridX, gridY, "whiteboard")
        {
            this.backgroundColor = System.Windows.Media.Brushes.WhiteSmoke;
            this.cellText = "Tavla";
        }
    }
}
