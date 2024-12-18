namespace Classroom_Seating_Planner.Cells
{
    public class TableCell : Cell
    {
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

            // When drawn, a table with a seated student will display their name
            if (this.student != null)
            {
                this.cellText = this.student?.name;
            }
            cellElement.Text = this.cellText;
        }

        // Add table to tableList for Populate method
        public override void AddToLayoutManager(Src.ClassroomLayoutManager classroomLayoutManager, System.Windows.Controls.TextBlock cellElement)
        {
            classroomLayoutManager.tableElements.Add(cellElement);
        }
    }
}
