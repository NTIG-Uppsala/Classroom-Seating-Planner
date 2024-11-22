using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classroom_Seating_Planner.cells
{
    public class Cell // TODO - make sure columnspan is before rowspan where possible
    {
        public int x, y;
        public int width, height;
        public int centerX, centerY;

        public string cellType;
        public string cellText = "!!CELL!!"; // This should never be seen O.O

        // Colors
        public System.Windows.Media.Brush backgroundColor = System.Windows.Media.Brushes.Yellow; // Yellow, so it's easier to spot. A base cell should never be drawn.
        public System.Windows.Media.Brush textColor = System.Windows.Media.Brushes.Black;

        public Cell(int x, int y, string cellType, int width = 1, int height = 1)
        {
            this.cellType = cellType;

            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.centerX = x + width / 2;
            this.centerY = y + height / 2;
        }

        public void Draw(System.Windows.Controls.Grid parent)
        {
            // Make the XAML element that will be the visual representation of the cell
            System.Windows.Controls.TextBlock cellElement = new System.Windows.Controls.TextBlock()
            {
                Text = this.cellText,
                Background = this.backgroundColor,
                Foreground = this.textColor,
            };

            // Give element a helptext that the tests can read
            System.Windows.Automation.AutomationProperties.SetHelpText(cellElement, $"cell:true, cellType:{this.cellType},x:{this.x},y:{this.y},centerX:{this.centerX},centerY:{this.centerY}");

            // Position this cell according to its coordinates
            System.Windows.Controls.Grid.SetRow(cellElement, this.y);
            System.Windows.Controls.Grid.SetColumn(cellElement, this.x);
            System.Windows.Controls.Grid.SetRowSpan(cellElement, this.width);
            System.Windows.Controls.Grid.SetColumnSpan(cellElement, this.height);

            // Add this cell to the parent grid
            parent.Children.Add(cellElement);
        }
    }
}
