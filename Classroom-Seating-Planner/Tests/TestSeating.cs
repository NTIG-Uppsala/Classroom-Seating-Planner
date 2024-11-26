using System.Diagnostics;
using FlaUIElement = FlaUI.Core.AutomationElements;

namespace Tests
{
    [TestClass]
    public class TestSeating
    {
        // Declare which tables should be checked
        private List<int> testingTablesIndexes = [0, 10, 16, 27, 32];

        [TestMethod]
        public void SeatStudentsTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp();


            // Find all the tables
            List<FlaUIElement.AutomationElement>? allTables = Utils.XAMLHandler.GetAllElementsByHelpText(window, cf, key: "cellType", value: "table");
            Assert.IsNotNull(allTables);

            // Check that (some of) the tables are empty at program start
            foreach (int index in this.testingTablesIndexes)
            {
                Assert.IsTrue(allTables[index].Name.Equals(string.Empty), "The tables are not empty at the start of the program");
            }

            Utils.XAMLHandler.ClickRandomizeSeatingButton(window, cf);

            // Get the tables again
            allTables = Utils.XAMLHandler.GetAllElementsByHelpText(window, cf, key: "cellType", value: "table");
            Assert.IsNotNull(allTables);

            // Get list of students to compare against list of tables
            List<string> allStudents = Utils.XAMLHandler.GetClassListFromElement(window, cf);

            // Check that the seating matches the order of the list of students
            foreach (int index in this.testingTablesIndexes)
            {
                Assert.IsTrue(allTables[index].Name.Equals(allStudents[index]), "The order of the seating is not the same as the order of the class list");
            }


            Utils.TearDown(app);
        }
    }
}