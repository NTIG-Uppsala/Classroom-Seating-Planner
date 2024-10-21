using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using System.Diagnostics;
using FlaUIElement = FlaUI.Core.AutomationElements;
namespace Tests
{
    [TestClass]
    public class TestUIExists
    {
        [TestMethod]
        public void TestListOfNames()
        {
            // Find and run the application
            FlaUI.Core.Application app = FlaUI.Core.Application.Launch("..\\..\\..\\..\\Classroom-Seating-Planner\\bin\\Debug\\net8.0-windows\\Classroom-Seating-Planner.exe");
            using FlaUI.UIA3.UIA3Automation automation = new();

            // Find the main window for the purpose of finding elements
            FlaUIElement.Window window = app.GetMainWindow(automation);
            ConditionFactory cf = new(new UIA3PropertyLibrary());


            // Find the list of names
            FlaUIElement.ListBox listBox = window.FindFirstDescendant(cf.ByAutomationId("ListBoxNames")).AsListBox();
            // Check if the list of names exists
            Assert.IsNotNull(listBox, "List of names does not exist");

            // Find the randomize button
            FlaUIElement.Button randomizeButton = window.FindFirstDescendant(cf.ByAutomationId("ButtonRandomize")).AsButton();
            // Check if the randomize button exists
            Assert.IsNotNull(randomizeButton, "Randomize button does not exist");

            // Find the whiteboard
            FlaUIElement.Label whiteboard = window.FindFirstDescendant(cf.ByAutomationId("LabelWhiteboard")).AsLabel();
            // Check if the whiteboard exists
            Assert.IsNotNull(whiteboard, "Whiteboard does not exist");

            // Find some table tables 
            FlaUIElement.GridCell table1 = window.FindFirstDescendant(cf.ByAutomationId("GridCellTable")).AsGridCell();
            FlaUIElement.GridCell table3 = window.FindChildAt(2, cf.ByAutomationId("GridCellTable")).AsGridCell();
            FlaUIElement.GridCell table6 = window.FindChildAt(5, cf.ByAutomationId("GridCellTable")).AsGridCell();

            // Check if the tables exist
            Assert.IsNotNull(table1, "Table 1 does not exist");
            Assert.IsNotNull(table3, "Table 3 does not exist");
            Assert.IsNotNull(table6, "Table 6 does not exist");

            app.Close();
        }
    }
}