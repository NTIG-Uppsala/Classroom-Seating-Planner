using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using System.Diagnostics;
namespace Tests
{
    [TestClass]
    public class TestTemplate
    {
        [TestMethod]
        public void TemplateMethod()
        {
            return; // Remove this in the real tests
            // Find and run the application
            FlaUI.Core.Application app = FlaUI.Core.Application.Launch("..\\..\\..\\..\\Classroom-Seating-Planner\\bin\\Debug\\net8.0-windows\\Classroom-Seating-Planner.exe");
            using (FlaUI.UIA3.UIA3Automation automation = new())
            {
                // Find the main window for the purpose of finding elements
                Window window = app.GetMainWindow(automation);
            }
        }
    }
}