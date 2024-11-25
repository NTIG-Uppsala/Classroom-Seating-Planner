using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Classroom_Seating_Planner.Cells
{
    public class Cell
    {
        public int x, y;
        public int width, height;
        public float centerX, centerY;

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
            this.centerX = x + (width - 1) / 2;
            this.centerY = y + (height - 1) / 2;
        }

        public void Draw(System.Windows.Controls.Grid parent)
        {
            // Make the XAML element that will be the visual representation of the cell
            System.Windows.Controls.TextBlock cellElement = new()
            {
                Text = this.cellText,
                Background = this.backgroundColor,
                Foreground = this.textColor,
            };

            // Center whiteboard text
            if (cellElement.Text.Equals("TAVLA"))
            {
                cellElement.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                cellElement.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                cellElement.TextAlignment = System.Windows.TextAlignment.Center;
                cellElement.FontWeight = FontWeights.SemiBold;
                cellElement.FontSize = 48;
                cellElement.TextWrapping = TextWrapping.Wrap;
            }

            // Create a Border to contain the TextBlock
            System.Windows.Controls.Border cellContainer = new()
            {
                Background = this.backgroundColor, // Set the background color on the Border
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                Child = cellElement
            };

            // Give element a helptext that the tests can read
            System.Windows.Automation.AutomationProperties
                .SetHelpText(cellElement,
                $"cell:true" +
                $"|" +
                $"cellType:{this.cellType}" +
                $"|" +
                $"x:{this.x}" +
                $"|" +
                $"y:{this.y}" +
                $"|" +
                $"width:{this.width}" +
                $"|" +
                $"height:{this.height}" +
                $"|" +
                $"centerX:{this.centerX}" +
                $"|" +
                $"centerY:{this.centerY}"
                );

            // Position this cell according to its coordinates
            System.Windows.Controls.Grid.SetColumn(cellContainer, (int)this.x);
            System.Windows.Controls.Grid.SetRow(cellContainer, (int)this.y);
            System.Windows.Controls.Grid.SetColumnSpan(cellContainer, (int)this.width);
            System.Windows.Controls.Grid.SetRowSpan(cellContainer, (int)this.height);

            // Add this cell to the parent grid
            parent.Children.Add(cellContainer);
        }
    }
}
