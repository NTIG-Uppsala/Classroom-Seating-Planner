using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Classroom_Seating_Planner.Src
{
    internal class DynamicClassroomSizeHandler
    {
        public static void UpdateClassroomLayoutSize(System.Windows.Size windowSize, MainWindow mainWindow) // TODO - Use only main window and derive window size from it
        {
            // Number of columns and rows inside the classroom layout grid
            int columnCount = mainWindow.ClassroomLayoutGridElement.ColumnDefinitions.Count;
            int rowCount = mainWindow.ClassroomLayoutGridElement.RowDefinitions.Count;

            // Avoid division by zero
            if (rowCount.Equals(0) || columnCount.Equals(0)) return;

            // Get the visible width of the grid from the window size 
            double gridViewportWidth = windowSize.Width - Math.Max(windowSize.Width * ((double)70 / 400), 5 * (96 / 2.54));
            double visibleWidth = gridViewportWidth - 15 - System.Windows.SystemParameters.VerticalScrollBarWidth - mainWindow.ClassroomLayoutGridElement.Margin.Left - mainWindow.ClassroomLayoutGridElement.Margin.Right;

            // Define the minimum width of the tables in pixles
            double minWidth = 60;

            // Calculate the optimal width for the tables based on available space and minimum value
            double columnWidth = Math.Max(minWidth, (double)visibleWidth / columnCount);

            // Update all column definitions with the new width
            foreach (System.Windows.Controls.ColumnDefinition columnDefinition in mainWindow.ClassroomLayoutGridElement.ColumnDefinitions)
            {
                columnDefinition.Width = new System.Windows.GridLength(columnWidth);
            }

            // Get the visible height of the grid from the window
            double gridViewportHeight = windowSize.Height - 1 * (96 / 2.54);
            double visibleHeight = gridViewportHeight - 45 - System.Windows.SystemParameters.HorizontalScrollBarHeight - mainWindow.ClassroomLayoutGridElement.Margin.Top - mainWindow.ClassroomLayoutGridElement.Margin.Bottom;

            // Define minimum and maximum height for the tables in pixels
            double minHeight = 12 + 1 * (22); // 1 row = 34px
            double maxHeight = 12 + 4 * (22); // 4 rows = 100px

            // Calculate the optimal height for the tables based on available space, mimimum value, and maximum value
            double rowHeight = Math.Max(minHeight, Math.Min((double)visibleHeight / rowCount, maxHeight));

            // Update all row definitions with the new height
            foreach (System.Windows.Controls.RowDefinition rowDefinition in mainWindow.ClassroomLayoutGridElement.RowDefinitions)
            {
                rowDefinition.Height = new System.Windows.GridLength(rowHeight);
            }
        }
    }
}