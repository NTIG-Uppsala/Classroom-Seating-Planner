using System.Diagnostics;
using FlaUIElement = FlaUI.Core.AutomationElements;

namespace Tests
{
    [TestClass]
    public class TestSeating
    {
        [TestMethod]
        public void StudentCountTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(); // SetUp has optional arguments that may be useful for certain tests


            // Seat all students
            Utils.XAMLHandler.ClickRandomizeSeatingButton(window, cf);

            // Find all the tables
            List<FlaUIElement.AutomationElement>? allTables = Utils.XAMLHandler.GetAllElementsByHelpText(window, cf, key: "cellType", value: "table");
            Assert.IsNotNull(allTables);
            // Find all the tables that have a student seated
            allTables = allTables.Where(table => !string.IsNullOrEmpty(table.Name)).ToList();

            // Count the amount of tables with students seated
            int? tableCount = allTables.Count;
            Assert.IsNotNull(tableCount);

            // Count the amount of students in the class list
            List<string> allStudents = Utils.XAMLHandler.GetClassListFromElement(window, cf);
            int studentCount = allStudents.Count;

            // Check that the amount of students is equal to the amount of tables
            Assert.IsTrue(studentCount.Equals(tableCount), "The amount of students is not equal to the amount of tables");


            Utils.TearDown(app);
        }

        [TestMethod]
        public void SeatStudentsTest()
        {
            // Sample indexes of tables to test
            List<int> testingTablesIndexes = [0, 10, 16, 27, 32];

            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp();


            // Find all the tables
            List<FlaUIElement.AutomationElement>? allTables = Utils.XAMLHandler.GetAllElementsByHelpText(window, cf, key: "cellType", value: "table");
            Assert.IsNotNull(allTables);

            // Check that the sampled tables are empty at program start
            foreach (int index in testingTablesIndexes)
            {
                Assert.IsTrue(allTables[index].Name.Equals(string.Empty), "The tables are not empty at the start of the program");
            }

            // Seating students
            Utils.XAMLHandler.ClickRandomizeSeatingButton(window, cf);

            // Get the tables again to make sure the that the refrences are updated
            allTables = Utils.XAMLHandler.GetAllElementsByHelpText(window, cf, key: "cellType", value: "table");
            Assert.IsNotNull(allTables);

            // Find all students in the class list
            List<string> allStudents = Utils.XAMLHandler.GetClassListFromElement(window, cf);

            // Check that the seating matches the order of the list of students
            foreach (int index in testingTablesIndexes)
            {
                Assert.IsTrue(allTables[index].Name.Equals(allStudents[index]), "The order of the seating is not the same as the order of the class list");
            }


            Utils.TearDown(app);
        }
    }
}