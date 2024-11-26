using System.Windows.Controls;

namespace Classroom_Seating_Planner.Src
{
    internal class SeatingHandler
    {
        public static void Populate(List<TextBlock> tableElements, List<string> classList)
        {
            // Ensure we don't exceed the number of available tables
            int numberOfStudentsToBePlaced = Math.Min(classList.Count, tableElements.Count);

            // Update the tables with the new order
            for (int index = 0; index < numberOfStudentsToBePlaced; index++)
            {
                // Assign the student name to the corresponding table
                tableElements[index].Text = classList[index];
            }

            // If there are more tables than students, clear the remaining tables
            for (int index = numberOfStudentsToBePlaced; index < tableElements.Count; index++)
            {
                // Clear the table if it's not occupied
                tableElements[index].Text = string.Empty;
            }
        }
    }
}
