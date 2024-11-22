using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
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

        public void DrawChildren()
        {
            this.tableCells.ForEach((tableCell) =>
            {
                tableCell.Draw(this.ClassroomElement);
            });

            this.whiteboardCells.ForEach((whiteboardCell) =>
            {
                whiteboardCell.Draw(this.ClassroomElement);
            });
        }

        public class WhiteboardManager
        {
            // instantiates with the list of whiteboard cells as an argument
            // this calculates where the whiteboard starts and ends to be able to set its grid.column and grid.row + grid.columnspan or grid.rowspan (for vertical whiteboards)

            // finding neigbour whiteboards aglorithm
            // 1. find the smallest x and y in the list
            //     * save those coords to be used for setting the grid.row and grid.column
            // 2. find the largest x and y in the list
            //     * get the delta of these coords and use that as the grid.rowspan and grid.columnspan


            // draw() fuction that handles the drawing of all the combined whiteboard that draws over the individual cells (maybe needs z-index?

            public List<cells.WhiteboardCell> whiteboardCells;

            public WhiteboardManager(List<cells.WhiteboardCell> whiteboardCells)
            {
                this.whiteboardCells = whiteboardCells;

                // TODO - instantiate a whiteboardCellCover using the calculated min and max coordinates
            }

            public struct Coordinates // TODO - place where distance is calculated when that feature is implemented
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
                    xValues.Add(whiteboardCell.x);
                    yValues.Add(whiteboardCell.y);
                });

                smallestX = xValues.Min();
                smallestY = yValues.Min();

                // TODO - null check?

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
                    xValues.Add(whiteboardCell.x);
                    yValues.Add(whiteboardCell.y);
                });

                largestX = xValues.Max();
                largestY = yValues.Max();

                // TODO - null check?

                return new Coordinates() { x = largestX.Value, y = largestY.Value };
            }

            public void Draw()
            {

            }
        }
    }
}