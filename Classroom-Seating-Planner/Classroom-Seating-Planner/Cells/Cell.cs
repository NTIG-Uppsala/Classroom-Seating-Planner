using Classroom_Seating_Planner.Src;
using System.Windows;

namespace Classroom_Seating_Planner.Cells
{
    public class Cell
    {
        public int gridX, gridY;
        public int width, height;
        public float x, y;

        public string cellType;
        public string cellText = "!!CELL!!"; // This should never be seen O.O

        // Colors
        public System.Windows.Media.Brush backgroundColor = System.Windows.Media.Brushes.Yellow; // Yellow, so it's easier to spot. A base cell should never be drawn.
        public System.Windows.Media.Brush textColor = System.Windows.Media.Brushes.Black;

        public Cell(int gridX, int gridY, string cellType, int width = 1, int height = 1)
        {
            this.cellType = cellType;

            this.gridX = gridX;
            this.gridY = gridY;
            this.width = width;
            this.height = height;
            this.x = gridX + (width - 1) / 2;
            this.y = gridY + (height - 1) / 2;
        }

        public virtual void Style(System.Windows.Controls.Border cellElementContainer, System.Windows.Controls.TextBlock cellElement) { }

        public virtual void AddToLayoutManager(ClassroomLayoutManager classroomLayoutManager, System.Windows.Controls.TextBlock cellElement) { }

        public void Draw(System.Windows.Controls.Grid parent, ClassroomLayoutManager classroomLayoutManager)
        {
            // Make the XAML element that will be the visual representation of the cell
            // The text that will be shown in the XAML element
            System.Windows.Controls.TextBlock cellElement = new()
            {
                Text = this.cellText,
                Background = this.backgroundColor,
                Foreground = this.textColor,
            };

            // The container for the text element that will also store data about the cell
            System.Windows.Controls.Border cellElementContainer = new()
            {
                Background = this.backgroundColor, // Set the background color on the Border
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                Child = cellElement,

                // Give the student names some room to breathe
                Padding = new System.Windows.Thickness(3, 3, 3, 3),
                BorderThickness = new System.Windows.Thickness(1, 1, 1, 1),
            };

            // TODO - Set help text to the container
            // Give element a helptext that the tests can read
            System.Windows.Automation.AutomationProperties
                .SetHelpText(cellElement,
                $"cell:true" +
                $";" +
                $"cellType:{this.cellType}" +
                $";" +
                $"gridX:{this.gridX}" +
                $";" +
                $"gridY:{this.gridY}" +
                $";" +
                $"width:{this.width}" +
                $";" +
                $"height:{this.height}" +
                $";" +
                $"x:{this.x}" +
                $";" +
                $"y:{this.y}"
                );

            // Position this cell according to its coordinates
            System.Windows.Controls.Grid.SetColumn(cellElementContainer, this.gridX);
            System.Windows.Controls.Grid.SetRow(cellElementContainer, this.gridY);
            System.Windows.Controls.Grid.SetColumnSpan(cellElementContainer, this.width);
            System.Windows.Controls.Grid.SetRowSpan(cellElementContainer, this.height);

            this.Style(cellElementContainer, cellElement);
            // TODO - Make this more general by pushing to classroomElements
            this.AddToLayoutManager(classroomLayoutManager, cellElement);

            // Add this cell to the parent grid
            parent.Children.Add(cellElementContainer);
        }
    }
}
