using System.Windows.Controls;
using System.Windows;

namespace Classroom_Seating_Planner.src
{
    public static class Tables
    {
        public static void Make(Panel templateTable, Panel parent, int amount = 1)
        {
            if (templateTable is null || parent is null || amount < 1)
            {
                return;
            }
            // Create a new table for each table needed
            for (int index = 0; index < amount; index++)
            {
                // Clone the templateTable to create a new instance
                // TODO: Implement the Clone method
            }
        }
    }
}
