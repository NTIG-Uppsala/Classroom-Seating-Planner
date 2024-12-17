namespace Classroom_Seating_Planner.Cells
{
    public class TableCell : Cell
    {
        //Src.ConstraintsHandler.Student? student; 

        public Src.ConstraintsHandler.Student? student { get; internal set; }
        public double score;

        public TableCell(int gridX, int gridY) : base(gridX, gridY, "table")
        {
            this.backgroundColor = System.Windows.Media.Brushes.LightGray;
            this.cellText = "";

            this.student = null;

            this.score = 0;
        }

        public override void Style(System.Windows.Controls.Border cellElementContainer, System.Windows.Controls.TextBlock cellElement)
        {
            cellElement.TextWrapping = System.Windows.TextWrapping.WrapWithOverflow;
        }

        // Add table to tableList for Populate method
        public override void AddToLayoutManager(Src.ClassroomLayoutManager classroomLayoutManager, System.Windows.Controls.TextBlock cellElement)
        {
            classroomLayoutManager.tableElements.Add(cellElement);
        }
    }
}
