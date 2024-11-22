using System.Windows.Controls;
using System.Windows.Input;

namespace Classroom_Seating_Planner.src
{
    public class ClassroomLayoutManager
    {
        // Reference to the XAML grid
        private readonly System.Windows.Controls.Grid ClassroomElement;

        public int rowCount;
        public int columnCount;
        public List<cells.TableCell> tableCells = [];
        public List<cells.WhiteboardCell> whiteboardCells = [];

        public ClassroomLayoutManager(System.Windows.Controls.Grid ClassroomElement)
        {
            this.ClassroomElement = ClassroomElement;

            // Wipe the grid clean
            this.ClassroomElement.Children.Clear();
            this.ClassroomElement.RowDefinitions.Clear();
            this.ClassroomElement.ColumnDefinitions.Clear();
        }

        public void Initialize(src.FileHandler.ClassroomLayoutData dataFromFileHandler)
        {
            this.SetProperties(dataFromFileHandler);

            this.SetGridRowsAndColumns();
        }

        private void SetGridRowsAndColumns()
        {
            // Add a RowDefinition for every row
            for (int row = 0; row < this.rowCount; row++)
            {
                ClassroomElement.RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            }

            // Add a ColumnDefinition for every column
            for (int column = 0; column < this.columnCount; column++)
            {
                ClassroomElement.ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            }
        }

        private void SetProperties(src.FileHandler.ClassroomLayoutData dataFromFileHandler)
        {
            this.rowCount = dataFromFileHandler.rowCount;
            this.columnCount = dataFromFileHandler.columnCount;
            this.tableCells = dataFromFileHandler.tableCells;
            this.whiteboardCells = dataFromFileHandler.whiteboardCells;
        }

        public class WhiteboardHandler
        {

        }
    }
}