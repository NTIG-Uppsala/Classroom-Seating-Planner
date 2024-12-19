using System.Diagnostics;
using FlaUIElement = FlaUI.Core.AutomationElements;
using Tests;

namespace A02_Automatic_Tests
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
            Assert.IsTrue(studentCount.Equals(tableCount), "The amount of students is not equal to the amount of tables where a student is seated");


            Utils.TearDown(app);
        }

        [TestMethod]
        public void RandomizeSeatingTest()
        {
            // Sample indexes of tables to check if they are empty at the start of the test
            List<int> testingTablesIndexes = [0, 10, 16, 27, 32];

            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp();


            // Find all the tables to check that some are empty at the start of the program
            List<FlaUIElement.AutomationElement>? allTables = Utils.XAMLHandler.GetAllElementsByHelpText(window, cf, key: "cellType", value: "table");
            Assert.IsNotNull(allTables);

            // Check that the sampled tables are empty at program start
            foreach (int index in testingTablesIndexes)
            {
                Assert.IsTrue(allTables[index].Name.Equals(string.Empty), "The tables are not empty at the start of the program");
            }

            for (int i = 0; i < 2; i++)
            {
                // Get the tables again to make sure the that the refrences are updated
                allTables = Utils.XAMLHandler.GetAllElementsByHelpText(window, cf, key: "cellType", value: "table");
                Assert.IsNotNull(allTables);

                // Get the seating arrangement as a list of strings (including empty spaces) before randomizing
                List<string> seatingArrangementBeforeRandomize = allTables.Select(table => table.Name).ToList();

                // Randomize seating arrangement
                Utils.XAMLHandler.ClickRandomizeSeatingButton(window, cf);

                // Get the tables again to make sure the that the refrences are updated
                allTables = Utils.XAMLHandler.GetAllElementsByHelpText(window, cf, key: "cellType", value: "table");
                Assert.IsNotNull(allTables);

                List<string> seatingArrangementAfterRandomize = allTables.Select(table => table.Name).ToList();

                // Check if the seating arrangement has changed
                Assert.IsFalse(seatingArrangementAfterRandomize.SequenceEqual(seatingArrangementBeforeRandomize), $"The seating arrangement did not change during run {i+1}");
            }


            Utils.TearDown(app);
        }
    }
}