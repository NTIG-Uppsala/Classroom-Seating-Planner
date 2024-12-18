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
        private readonly System.Windows.Controls.Grid gridElement;

        public ClassroomLayoutManager(System.Windows.Controls.Grid gridElement)
        {
            this.gridElement = gridElement;
        }

        public void Render(List<Cells.Cell> classroomElements)
        {
            this.UpdateGridSize(classroomElements);

            // Draw every classroom element
            classroomElements.ForEach(cell =>
            {
                cell.Draw(this.gridElement, this);
            });
        }

        // TODO - Modify to update the grid or start from scratch
        private void UpdateGridSize(List<Cells.Cell> classroomElements)
        {
            // Clear the grid
            this.gridElement.Children.Clear();
            this.gridElement.ColumnDefinitions.Clear();
            this.gridElement.RowDefinitions.Clear();

            // Find the row and column count
            List<int> allGridXValues = [];
            List<int> allGridYValues = [];
            classroomElements.ForEach(cell =>
            {
                allGridXValues.Add(cell.gridX);
                allGridYValues.Add(cell.gridY);
            });
            int columnCount = allGridXValues.Max() + 1;
            int rowCount = allGridYValues.Max() + 1;

            // Add a ColumnDefinition for every column
            for (int i = 0; i < columnCount; i++)
            {
                gridElement.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Add a RowDefinition for every row
            for (int i = 0; i < rowCount; i++)
            {
                gridElement.RowDefinitions.Add(new RowDefinition());
            }
        }
    }
}