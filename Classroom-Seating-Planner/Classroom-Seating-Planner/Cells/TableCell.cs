using Classroom_Seating_Planner.Src;

namespace Classroom_Seating_Planner.Cells
{
    public class TableCell : Cell
    {
        public TableCell(int gridX, int gridY) : base(gridX, gridY, "table")
        {
            this.backgroundColor = System.Windows.Media.Brushes.LightGray;
            this.cellText = "";
        }

        public override void Style(System.Windows.Controls.Border cellElementContainer, System.Windows.Controls.TextBlock cellElement)
        {
            cellElement.TextWrapping = System.Windows.TextWrapping.WrapWithOverflow;
        }

        // Add table to tableList for Populate method
        public override void AddToLayoutManager(ClassroomLayoutManager classroomLayoutManager, System.Windows.Controls.TextBlock cellElement)
        {
            classroomLayoutManager.tableElements.Add(cellElement);
        }
    }
}
