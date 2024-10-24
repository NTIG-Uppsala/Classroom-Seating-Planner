using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using System.Diagnostics;
using FlaUIElement = FlaUI.Core.AutomationElements;

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

            // Find Randomize Seating Button
            FlaUIElement.Button randomizeButton = window.FindFirstDescendant(cf.ByAutomationId("ButtonRandomizeSeating")).AsButton();

            app.Close();
        }
    }
}