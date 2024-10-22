using System.Windows.Controls;
using System.Windows;

namespace Classroom_Seating_Planner.src
{
    public static class GetByTag
    {
        public static List<Label> Labels(Panel parent, string tag)
        {
            List<Label> matches = [];

            if (parent is null)
            {
                return matches;
            }

            // Helper function to recursively search inside containers
            void SearchInContainer(UIElementCollection children)
            {
                foreach (var item in children)
                {
                    // If the item is a Label save it, else search iterable children
                    if (
                        item is Label label
                        &&
                        label.Tag != null
                        &&
                        label.Tag.ToString() == tag)
                    {
                        matches.Add(label);
                    }
                    else if (item is Grid grid)
                    {
                        SearchInContainer(grid.Children);  // Recursive call
                    }
                    else if (item is Panel panel)
                    {
                        SearchInContainer(panel.Children);  // Recursive call
                    }
                    else if (item is Border border && border.Child != null)
                    {
                        SearchInSingleChild(border.Child);  // Recursive call
                    }
                }
            }

            // Helper function to handle single-child containers like Border
            void SearchInSingleChild(UIElement child)
            {
                // If the child is a Label, check the Tag and add it if it matches
                if (
                    child is Label label
                    &&
                    label.Tag != null
                    &&
                    label.Tag.ToString() == tag
                    )
                {
                    matches.Add(label);
                }
                else if (child is Grid grid)
                {
                    SearchInContainer(grid.Children);  // Recursive call for Grid
                }
                else if (child is Border border && border.Child != null)
                {
                    SearchInSingleChild(border.Child);  // Recursive call for Border's child
                }
            }

            // Start searching in the main grid
            SearchInContainer(parent.Children);

            return matches;
        }
    }
}
