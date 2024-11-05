using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using System.IO;
using FlaUIElement = FlaUI.Core.AutomationElements;

namespace Classroom_Seating_Planner.src
{
    public class ListActions
    {
        // Public method for shuffling lists
        public static List<string> Shuffle(List<string> list)
        {
            // Can't shuffle 0 or 1 elements
            if (list.Count < 2) return list;

            Random rng = new();
            List<string> newList = list;

            // Shuffle until the list has a new order
            while (newList.SequenceEqual(list))
            {
                newList = [.. list.OrderBy(item => rng.Next())];
            }
            return newList;
        }

        // Public method for fetching ListBox items as array
        public static string[] GetListBoxItemsAsArray(FlaUIElement.Window window, ConditionFactory cf, string listBoxAutomaitonId)
        {
            FlaUIElement.ListBox listBox = window.FindFirstDescendant(cf.ByAutomationId(listBoxAutomaitonId)).AsListBox();
            ListBoxItem[] listBoxItemsList = listBox.Items;
            string[] listItemsArray = listBoxItemsList.Select(listItem => listItem.Text).ToArray();

            return listItemsArray;
        }

        // Public method for fetching student names from an external file
        public static List<string> GetStudentNamesFromFile()
        {
            // Get the paths to the app's directory and the names file
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsFolder, "Bordsplaceringsgeneratorn", "klasslista.txt");

            // Read the names from the file and return them as a list
            using StreamReader reader = new(filePath);
            List<string> names = reader
                .ReadToEnd()
                .Split("\n")
                .Select(name => name.Trim())
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList();
            return names;
        }
    }
}
