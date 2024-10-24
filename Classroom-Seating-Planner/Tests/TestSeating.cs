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

            var allElements = window.FindAllDescendants(cf.ByFrameworkId("WPF")).ToList();

            var allSeats = allElements.Where(element =>
            {
                return (
                    !string.IsNullOrEmpty(element.AutomationId)
                    &&
                    element.AutomationId.Contains("Seat")
                    &&
                    element.ControlType.Equals(FlaUI.Core.Definitions.ControlType.Text)
                );
            }).ToList();

            foreach (var seat in allSeats)
            {
                Trace.WriteLine(seat);
            }
            Trace.WriteLine(allSeats.Count);

            app.Close();
        }
    }
}