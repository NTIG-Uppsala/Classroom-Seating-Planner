using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using FlaUIElement = FlaUI.Core.AutomationElements;
using ListActions = Classroom_Seating_Planner.ListActions;

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

            // Find all the seats
            List<FlaUIElement.AutomationElement> allSeats = GetAllSeats(window, cf);

            // Read that some of them are empty
            string errorMessage = "Some of the seats are not empty before hitting the randomize button";
            Assert.IsTrue(allSeats[0].Name.Equals(string.Empty), errorMessage);
            Assert.IsTrue(allSeats[16].Name.Equals(string.Empty), errorMessage);
            Assert.IsTrue(allSeats[32].Name.Equals(string.Empty), errorMessage);

            // Find Randomize Seating Button
            FlaUIElement.Button randomizeButton = window.FindFirstDescendant(cf.ByAutomationId("RandomizeSeatingButton")).AsButton();

            randomizeButton.Click();

            // Get the seats again
            allSeats = GetAllSeats(window, cf);

            // Read that some of the seats are not empty
            errorMessage = "Some of the seats are empty after hitting the randomize button";
            Assert.IsFalse(allSeats[0].Name.Equals(string.Empty), errorMessage);
            Assert.IsFalse(allSeats[16].Name.Equals(string.Empty), errorMessage);
            Assert.IsFalse(allSeats[32].Name.Equals(string.Empty), errorMessage); // Get index by length of student list? assuming the empty seates will always be the last ones

            app.Close();
        }

        private static List<FlaUIElement.AutomationElement> GetAllSeats(Window window, ConditionFactory cf)
        {
            // Find all element
            var allElements = window.FindAllDescendants(cf.ByFrameworkId("WPF")).ToList();

            // Find all seats
            List<FlaUIElement.AutomationElement> allSeats = allElements.Where(element =>
                !string.IsNullOrEmpty(element.AutomationId)
                &&
                element.AutomationId.Contains("Seat")
                &&
                element.ControlType.Equals(FlaUI.Core.Definitions.ControlType.Text)
            ).ToList();

            return allSeats;
        }
    }
}