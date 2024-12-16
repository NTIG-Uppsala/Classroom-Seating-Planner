using System.Windows.Controls;

namespace Classroom_Seating_Planner.Src
{
    internal class ClassListElementHandler
    {
        public static void Populate(ListBox classListElement, List<string>? classList)
        {
            if (classList == null)
            {
                return;
            }

            // Clear the ListBox before populating
            classListElement.Items.Clear();

            // Populate classListElement with classList
            foreach (string name in classList)
            {
                ListBoxItem student = new()
                {
                    Content = name,
                    // These properties prevent the ListBoxItem from being selected
                    IsHitTestVisible = false,
                    Focusable = false,
                    IsSelected = false,
                    IsTabStop = false
                };
                classListElement.Items.Add(student);
            }
        }
    }
}
