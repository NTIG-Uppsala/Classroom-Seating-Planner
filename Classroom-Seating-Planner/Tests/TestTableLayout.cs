using FlaUIElement = FlaUI.Core.AutomationElements;

namespace Tests
{
    [TestClass]
    public class TestTableLayout
    {
        // TODO - when file reading is implemented, this will be in Utils
        private string classroomLayout =
            "   TTTT\n" +
            "\n" +
            "BB BB BB BB BB\n" +
            "\n" +
            "BBBB       BBB\n" +
            "      BBBB \n" +
            "\n" +
            " BB BB  BB BB\n" +
            "\n" +
            "B BB BB  BB";

        // TODO - this will change after file reading is implemented
        private int expextedColumns = 14;
        private int expextedRows = 10;

        private FlaUIElement.AutomationElement FindClassroomGrid(FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
        {
            return window.FindFirstDescendant(cf.ByAutomationId("TablesGrid"));
        }

        [TestMethod]
        public void ColumnAndRowCountTest()
        {
            return; // Remove this when the test isn't broken
            // Setup/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(); // SetUp has optional arguments that may be useful for certain tests

            // Write your test after this comment!


            // find the tablesGrid - helper function in class
            FlaUIElement.AutomationElement classroomGrid = FindClassroomGrid(window, cf);

            // get the number of columns and rows
            //int gridRows = classroomGrid.g;
            //int gridColumns = classroomGrid.Columns.Count;

            // compare the number of columns and rows to the expected values


            // Write your test before this comment!
            Utils.TearDown(app);
        }

        [TestMethod]
        public void TableAtExpectedPositionTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(); // SetUp has optional arguments that may be useful for certain tests

            // Write your test after this comment!


            // Find the tablesGrid - helper function in class

            // sample some random table cells from the classroomLayout

            // check if those samples have their equivalent in the tablesGrid


            // Write your test before this comment!
            Utils.TearDown(app);
        }
    }
}



