using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using System.Diagnostics;
using FlaUIElement = FlaUI.Core.AutomationElements;
using ListActions = Classroom_Seating_Planner.src.ListActions;

namespace Tests
{
    [TestClass]
    public class TestSeating
    {
        [TestMethod]
        public void TestSeatingIsCorrect()
        {
            // Find and run the application
            FlaUI.Core.Application app = FlaUI.Core.Application.Launch("..\\..\\..\\..\\Classroom-Seating-Planner\\bin\\Debug\\net8.0-windows\\win-x64\\Classroom-Seating-Planner.exe");
            using FlaUI.UIA3.UIA3Automation automation = new();

            // Find the main window for the purpose of finding elements
            FlaUIElement.Window window = app.GetMainWindow(automation);
            ConditionFactory cf = new(new UIA3PropertyLibrary());

            int[] testIndex = [0, 10, 16, 27, 32];

            // Find all the seats
            List<FlaUIElement.AutomationElement> allSeats = Utils.GetAllByAutomationId(window, cf, "Seat", FlaUI.Core.Definitions.ControlType.Text);

            // Check that (some of the) seats are empty at program start
            string errorMessage = "The seats are not empty at the start of the program";
            foreach (int index in testIndex)
            {
                Assert.IsTrue(allSeats[index].Name.Equals(string.Empty), errorMessage);
            }

            Utils.ClickRandomizeSeatingButton(window, cf);

            // Get the seats again
            allSeats = Utils.GetAllByAutomationId(window, cf, "Seat", FlaUI.Core.Definitions.ControlType.Text);

            // Get array of names to compare against list of seats
            string[] allNames = ListActions.GetListBoxItemsAsArray(window, cf, "StudentList");

            // Check that the seating matches the order of the list of student names
            errorMessage = "The order of the seating is not the same as the order of the student list";
            foreach (int index in testIndex)
            {
                Assert.IsTrue(allSeats[index].Name.Equals(allNames[index]), errorMessage);
            }

            app.Close();
        }
    }
}