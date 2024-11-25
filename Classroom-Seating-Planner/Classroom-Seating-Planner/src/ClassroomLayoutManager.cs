using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;

namespace Classroom_Seating_Planner.src
{
    public class ClassroomLayoutManager
    {
        // Reference to the XAML grid
        private readonly System.Windows.Controls.Grid classroomElement;

        public int rowCount;
        public int columnCount;
        public List<cells.TableCell> tableCells = [];
        public List<cells.WhiteboardCell> whiteboardCells = [];

        public ClassroomLayoutManager(System.Windows.Controls.Grid classroomElement)
        {
            this.classroomElement = classroomElement;

            // Wipe the grid clean
            this.classroomElement.Children.Clear();
            this.classroomElement.RowDefinitions.Clear();
            this.classroomElement.ColumnDefinitions.Clear();
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
                classroomElement.RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            }

            // Add a ColumnDefinition for every column
            for (int column = 0; column < this.columnCount; column++)
            {
                classroomElement.ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            }
        }

        private void SetProperties(src.FileHandler.ClassroomLayoutData dataFromFileHandler)
        {
            this.rowCount = dataFromFileHandler.rowCount;
            this.columnCount = dataFromFileHandler.columnCount;
            this.tableCells = dataFromFileHandler.tableCells;
            this.whiteboardCells = dataFromFileHandler.whiteboardCells;
        }

        public void DrawChildren()
        {
            this.tableCells.ForEach((tableCell) =>
            {
                tableCell.Draw(this.classroomElement);
            });

            //this.whiteboardCells.ForEach((whiteboardCell) =>
            //{
            //    whiteboardCell.Draw(this.classroomElement);
            //});

            WhiteboardManager whiteboardManager = new(this.whiteboardCells, this.classroomElement);
            //whiteboardManager.coverCell.Draw();
        }

        public class WhiteboardManager
        {
            // instantiates with the list of whiteboard cells as an argument
            // this calculates where the whiteboard starts and ends to be able to set its grid.column and grid.row + grid.columnspan or grid.rowspan (for vertical whiteboards)

            // Reference to the XAML grid
            private readonly System.Windows.Controls.Grid classroomElement;
            public List<cells.WhiteboardCell> whiteboardCells;

            public WhiteboardManager(List<cells.WhiteboardCell> whiteboardCells, System.Windows.Controls.Grid classroomElement)
            {
                this.whiteboardCells = whiteboardCells;
                this.classroomElement = classroomElement;

                // TODO - instantiate a whiteboardCellCover using the calculated min and max coordinates

                Coordinates smallestCoordinates = GetSmallestCoordinates();
                Coordinates largestCoordinates = GetLargestCoordinates();

                int smallestX = (int)smallestCoordinates.x;
                int smallestY = (int)smallestCoordinates.y;
                int largestX = (int)largestCoordinates.x;
                int largestY = (int)largestCoordinates.y;
                int width = largestX - smallestX + 1;
                int height = largestY - smallestY + 1;

                cells.WhiteboardCoverCell whiteboardCoverCell = new(smallestX, smallestY, width, height);

                whiteboardCoverCell.Draw(this.classroomElement);
            }

            public struct Coordinates // TODO - Move this struct to where distance is calculated when that functionality is implemented
            {
                public float x;
                public float y;
            }

            private Coordinates GetSmallestCoordinates()
            {
                int? smallestX = null;
                int? smallestY = null;

                List<int> xValues = [];
                List<int> yValues = [];
                
                this.whiteboardCells.ForEach(whiteboardCell =>
                {
                    xValues.Add((int)whiteboardCell.x);
                    yValues.Add((int)whiteboardCell.y);
                });

                smallestX = xValues.Min();
                smallestY = yValues.Min();

                return new Coordinates() { x = smallestX.Value, y = smallestY.Value };
            }

            private Coordinates GetLargestCoordinates()
            {
                int? largestX = null;
                int? largestY = null;

                List<int> xValues = [];
                List<int> yValues = [];

                this.whiteboardCells.ForEach(whiteboardCell =>
                {
                    xValues.Add((int)whiteboardCell.x);
                    yValues.Add((int)whiteboardCell.y);
                });

                largestX = xValues.Max();
                largestY = yValues.Max();

                return new Coordinates() { x = largestX.Value, y = largestY.Value };
            }

            public void Draw(System.Windows.Controls.Grid parent)
            {

            }
        }
    }
}