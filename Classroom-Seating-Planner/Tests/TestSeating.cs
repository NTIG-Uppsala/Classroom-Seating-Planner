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
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp();

            int[] testIndex = [0, 10, 16, 27, 32];

            // Find all the seats
            List<FlaUIElement.AutomationElement> allSeats = Utils.XAMLManager.GetAllByAutomationId(window, cf, "Seat", FlaUI.Core.Definitions.ControlType.Text);

            // Check that (some of the) seats are empty at program start
            string errorMessage = "The seats are not empty at the start of the program";
            foreach (int index in testIndex)
            {
                Assert.IsTrue(allSeats[index].Name.Equals(string.Empty), errorMessage);
            }

            Utils.XAMLManager.ClickRandomizeSeatingButton(window, cf);

            // Get the seats again
            allSeats = Utils.XAMLManager.GetAllByAutomationId(window, cf, "Seat", FlaUI.Core.Definitions.ControlType.Text);

            // Get list of students to compare against list of seats
            List<string> allStudents = Utils.XAMLManager.GetListBoxItemsAsList(window, cf, "ClassListElement");

            // Check that the seating matches the order of the list of students
            errorMessage = "The order of the seating is not the same as the order of the class list";
            foreach (int index in testIndex)
            {
                Assert.IsTrue(allSeats[index].Name.Equals(allStudents[index]), errorMessage);
            }

            Utils.TearDown(app);
        }
    }
}