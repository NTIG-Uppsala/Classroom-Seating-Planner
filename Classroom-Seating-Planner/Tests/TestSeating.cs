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
        public void SeatStudentsTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp();


            // Declare which seats should be checked
            List<int> seatIndexes = [0, 10, 16, 27, 32];

            // Find all the seats
            List<FlaUIElement.AutomationElement> allSeats = Utils.XAMLHandler.GetAllByAutomationId(window, cf, "Seat", FlaUI.Core.Definitions.ControlType.Text);

            // Check that (some of the) seats are empty at program start
            foreach (int index in seatIndexes)
            {
                Assert.IsTrue(allSeats[index].Name.Equals(string.Empty), "The seats are not empty at the start of the program");
            }

            Utils.XAMLHandler.ClickRandomizeSeatingButton(window, cf);

            // Get the seats again
            allSeats = Utils.XAMLHandler.GetAllByAutomationId(window, cf, "Seat", FlaUI.Core.Definitions.ControlType.Text);

            // Get list of students to compare against list of seats
            List<string> allStudents = Utils.XAMLHandler.GetClassListFromElement(window, cf);

            // Check that the seating matches the order of the list of students
            foreach (int index in seatIndexes)
            {
                Assert.IsTrue(allSeats[index].Name.Equals(allStudents[index]), "The order of the seating is not the same as the order of the class list");
            }


            Utils.TearDown(app);
        }
    }
}