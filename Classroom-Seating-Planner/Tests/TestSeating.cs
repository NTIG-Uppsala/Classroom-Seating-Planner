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

            int[] testIndex = { 1, 16, 32 }; // Get indexes by length of student list? assuming the empty seates will always be the last ones

            // Find all the seats
            List<FlaUIElement.AutomationElement> allSeats = Utils.GetAllByAutomationId(window, cf, "Seat", FlaUI.Core.Definitions.ControlType.Text);

            // Read that some of them are empty
            string errorMessage = "Some of the seats are not empty before hitting the randomize button";
            foreach (int index in testIndex)
            {
                Assert.IsTrue(allSeats[index].Name.Equals(string.Empty), errorMessage);
            }
            //Assert.IsTrue(allSeats[0].Name.Equals(string.Empty), errorMessage);
            //Assert.IsTrue(allSeats[16].Name.Equals(string.Empty), errorMessage);
            //Assert.IsTrue(allSeats[32].Name.Equals(string.Empty), errorMessage);

            Utils.ClickRandomizeSeatingButton(window, cf);

            // Get the seats again
            allSeats = Utils.GetAllByAutomationId(window, cf, "Seat", FlaUI.Core.Definitions.ControlType.Text);

            // Get array of names to compare agains list of seats
            string[] allNames = ListActions.GetListBoxItemsAsArray(window, cf, "StudentList");

            // Read that some of the seats are not empty
            errorMessage = "Some of the seats are empty after hitting the randomize button";
            foreach (int index in testIndex)
            {
                Assert.IsTrue(allSeats[index].Name.Equals(allNames[index]), errorMessage);
            }
            //Assert.IsTrue(allSeats[0].Name.Equals(allNames[0]), errorMessage);
            //Assert.IsTrue(allSeats[16].Name.Equals(allNames[16]), errorMessage);
            //Assert.IsTrue(allSeats[32].Name.Equals(allNames[32]), errorMessage); 

            app.Close();
        }
    }
}