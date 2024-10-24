using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaUIElement = FlaUI.Core.AutomationElements;


namespace Classroom_Seating_Planner
{
    public class ListActions
    {
        // Public method for shuffling lists
        public static List<string> Shuffle(List<string> list)
        {
            Random rng = new();
            List<string> newList = [.. list.OrderBy(item => rng.Next())];
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
    }
}
