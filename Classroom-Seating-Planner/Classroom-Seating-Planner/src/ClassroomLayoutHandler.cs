using System.Windows.Input;

namespace Classroom_Seating_Planner.src
{
    public class ClassroomLayoutHandler
    {
        // Reference to the XAML grid
        private readonly System.Windows.Controls.Grid ClassroomElement;

        public List<Cells.TableCell> tableCells = [];
        public List<Cells.WhiteboardCell> whiteboardCells = [];

        // Set by the filehandler while interpreting the classroom layout
        public int rowCount;
        public int columnCount;

        public ClassroomLayoutHandler(System.Windows.Controls.Grid ClassroomElement)
        {
            this.ClassroomElement = ClassroomElement;

            // Wipe the grid clean
            this.ClassroomElement.Children.Clear();
            this.ClassroomElement.RowDefinitions.Clear();
            this.ClassroomElement.ColumnDefinitions.Clear();
        }

        public void SetRowAndColumnCount()
        {
            
        }

        public class WhiteboardHandler
        {

        }
    }
}