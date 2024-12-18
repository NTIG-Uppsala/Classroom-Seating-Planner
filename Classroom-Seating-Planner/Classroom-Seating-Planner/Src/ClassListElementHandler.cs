using System.Windows.Controls;

namespace Classroom_Seating_Planner.Src
{
    internal class ClassListElementHandler
    {
        public static void Populate(ListBox classListElement, List<ConstraintsHandler.Student> students)
        {
            // Clear the ListBox before populating
            classListElement.Items.Clear();

            // Populate classListElement with classList
            students.ForEach(student =>
            {
                classListElement.Items.Add(new ListBoxItem()
                {
                    Content = student.name,
                    // These properties prevent the ListBoxItem from being selected
                    IsHitTestVisible = false,
                    Focusable = false,
                    IsSelected = false,
                    IsTabStop = false
                });
            });
        }
    }
}
