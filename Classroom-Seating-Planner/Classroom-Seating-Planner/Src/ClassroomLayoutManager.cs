using Classroom_Seating_Planner.Cells;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;

namespace Classroom_Seating_Planner.Src
{
    public class ClassroomLayoutManager
    {
        // Reference to the XAML grid
        private readonly System.Windows.Controls.Grid classroomElement;

        public int columnCount;
        public int rowCount;
        public List<Cells.TableCell> tableCells = [];
        public List<Cells.WhiteboardCell> whiteboardCells = [];
        public List<System.Windows.Controls.TextBlock> tableElements = [];
        public Src.ClassroomLayoutManager.WhiteboardManager? whiteboardManager;

        public ClassroomLayoutManager(System.Windows.Controls.Grid classroomElement)
        {
            this.classroomElement = classroomElement;

            // Wipe the grid clean
            this.classroomElement.Children.Clear();
            this.classroomElement.ColumnDefinitions.Clear();
            this.classroomElement.RowDefinitions.Clear();
        }

        public void Initialize(Src.FileHandler.ClassroomLayoutData dataFromFileHandler)
        {
            this.SetProperties(dataFromFileHandler);

            this.whiteboardManager = new(this.whiteboardCells);

            this.SetGridColumnsAndRows();
        }

        private void SetGridColumnsAndRows()
        {
            // Add a ColumnDefinition for every column
            for (int column = 0; column < this.columnCount; column++)
            {
                classroomElement.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Add a RowDefinition for every row
            for (int row = 0; row < this.rowCount; row++)
            {
                classroomElement.RowDefinitions.Add(new RowDefinition());
            }
        }

        private void SetProperties(Src.FileHandler.ClassroomLayoutData dataFromFileHandler)
        {
            this.columnCount = dataFromFileHandler.columnCount;
            this.rowCount = dataFromFileHandler.rowCount;
            this.tableCells = dataFromFileHandler.tableCells;
            this.whiteboardCells = dataFromFileHandler.whiteboardCells;
        }

        public void DrawChildren()
        {
            if (this.whiteboardManager == null)
            {
                return;
            }

            // Draw all tables
            this.tableCells.ForEach((tableCell) =>
            {
                tableCell.Draw(this.classroomElement, this);
            });

            if (this.whiteboardManager.whiteboardCoverCell == null)
            {
                return;
            }

            // Draw whiteboard
            this.whiteboardManager.whiteboardCoverCell.Draw(this.classroomElement, this);
        }

        public class WhiteboardManager
        {
            // Instantiates with the list of whiteboard cells as an argument
            // This calculates where the whiteboard starts and ends to be able to set its grid.column and grid.row + grid.columnspan or grid.rowspan (for vertical whiteboards)

            // Reference to the XAML grid
            public List<Cells.WhiteboardCell> whiteboardCells;
            public Cells.WhiteboardCoverCell? whiteboardCoverCell;

            // TODO - (low prio?) refactor
            // TODO - remove after implementing the same functionality in constraints
            public WhiteboardManager(List<Cells.WhiteboardCell> whiteboardCells)
            {
                this.whiteboardCells = whiteboardCells;

                if (this.whiteboardCells.Count.Equals(0))
                {
                    return;
                }

                // TODO - these coords are not referring to center coords but are not specified as anything else??
                // Get coordinates from all whiteboardCells to use when drawing the cover cell
                Coordinates smallestCoordinates = GetSmallestCoordinates();
                Coordinates largestCoordinates = GetLargestCoordinates();

                int smallestX = (int)smallestCoordinates.x;
                int smallestY = (int)smallestCoordinates.y;
                int largestX = (int)largestCoordinates.x;
                int largestY = (int)largestCoordinates.y;
                int width = largestX - smallestX + 1;
                int height = largestY - smallestY + 1;

                Cells.WhiteboardCoverCell whiteboardCoverCell = new(smallestX, smallestY, width, height);

                this.whiteboardCoverCell = whiteboardCoverCell;
            }

            public struct Coordinates // TODO - Check with Viggo why this is necessary. "It ain't" - viggo
            {
                public float x;
                public float y;
            }

            private Coordinates GetSmallestCoordinates()
            {
                List<int> xValues = [];
                List<int> yValues = [];

                this.whiteboardCells.ForEach(whiteboardCell =>
                {
                    xValues.Add((int)whiteboardCell.gridX);
                    yValues.Add((int)whiteboardCell.gridY);
                });

                int? smallestX = xValues.Min();
                int? smallestY = yValues.Min();

                return new Coordinates() { x = smallestX.Value, y = smallestY.Value };
            }

            private Coordinates GetLargestCoordinates()
            {
                List<int> xValues = [];
                List<int> yValues = [];

                this.whiteboardCells.ForEach(whiteboardCell =>
                {
                    xValues.Add((int)whiteboardCell.gridX);
                    yValues.Add((int)whiteboardCell.gridY);
                });

                int? largestX = xValues.Max();
                int? largestY = yValues.Max();

                return new Coordinates() { x = largestX.Value, y = largestY.Value };
            }
        }
    }
}