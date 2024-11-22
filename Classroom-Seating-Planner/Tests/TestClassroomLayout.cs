using System.Dynamic;
using FlaUIElement = FlaUI.Core.AutomationElements;

namespace Tests
{
    [TestClass]
    public class TestClassroomLayout
    {
        // TODO - when file reading is implemented, this will be in Utils.SetUp()
        private string testingClassroomLayoutString =
            "   TTTT\n" +
            "\n" +
            "BB BB BB BB BB\n" +
            "\n" +
            "BBBB       BBB\n" +
            "      BBBB\n" +
            "\n" +
            " BB BB  BB BB\n" +
            "\n" +
            "B BB BB  BB";

        // TODO - this will change after file reading is implemented - will be based on standard layout file
        private int expectedColumns = 14;
        private int expectedRows = 10;

        [TestMethod]
        public void ClassroomSizeTest()
        {
            // Setup/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(); // SetUp has optional arguments that may be useful for certain tests


            // Get all the cells (table and whiteboard cells)
            List<FlaUIElement.AutomationElement>? allCells = Utils.XAMLHandler.GetAllElementsByHelpText(window, cf, key: "cell", value: true, options: new(matchWholeString: false));
            Assert.IsNotNull(allCells);

            // Make a list of all the cells data
            List<IDictionary<string, object>> cellObjectList = new();
            foreach (FlaUIElement.AutomationElement cell in allCells)
            {
                IDictionary<string, object>? cellData = Utils.XAMLHandler.ParseStringToObject(cell.HelpText);
                Assert.IsNotNull(cellData);
                Assert.IsNotNull(cellData["cellType"].ToString());

                if ((string)cellData["cellType"] == "whiteboardCover")
                {
                    continue;
                }

                cellObjectList.Add(cellData);
            }

            // Store all the x and y values to find the max value
            List<int> xValues = cellObjectList.Select(cell => (int)cell["x"]).ToList();
            List<int> yValues = cellObjectList.Select(cell => (int)cell["y"]).ToList();

            // compare the largest x and y values to the expected values
            // -1 on the expected values because the grid is 0-indexed
            Assert.IsTrue(xValues.Max().Equals(this.expectedColumns - 1), "The amount of columns in the table-grid is not correct");
            Assert.IsTrue(yValues.Max().Equals(this.expectedRows - 1), "The amount of rows in the table-grid is not correct");


            Utils.TearDown(app);
        }

        [TestMethod]
        public void TableAtExpectedPositionTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(); // SetUp has optional arguments that may be useful for certain tests


            List<Dictionary<string, int>> testCaseCoordinatesList = [
                new() { { "x", 0 }, { "y", 2 }, }, // Should be a table cell
                new() { { "x", 8 }, { "y", 4 }, }, // Should be empty
                new() { { "x", 5 }, { "y", 0 }, }, // Should be a whiteboard cell
            ];

            // Get all the cells (table and whiteboard cells)
            List<FlaUIElement.AutomationElement>? allCells = Utils.XAMLHandler.GetAllElementsByHelpText(window, cf, key: "cell", value: true, options: new(matchWholeString: false));
            Assert.IsNotNull(allCells);

            // Make a list of all the cells data
            List<IDictionary<string, object>> cellDataList = [];
            foreach (FlaUIElement.AutomationElement cell in allCells)
            {
                IDictionary<string, object>? cellData = Utils.XAMLHandler.ParseStringToObject(cell.HelpText);
                Assert.IsNotNull(cellData);

                cellDataList.Add(cellData);
            }

            List<string> classroomLayoutMatrix = testingClassroomLayoutString.Split("\n").ToList();

            // Check that the cell types are the same between the classroom layout file and the XAML grid for each sampled coordinate
            foreach (Dictionary<string, int> testCaseCoordinates in testCaseCoordinatesList)
            {
                int xTestCaseCoordinate = testCaseCoordinates["x"];
                int yTestCaseCoordinate = testCaseCoordinates["y"];

                // Find the coresponding grid cell's data by matching it's coordinates with the test case's
                var cellData = cellDataList.Where((cellDataCandidate) =>
                    cellDataCandidate["x"].Equals(xTestCaseCoordinate)
                    &&
                    cellDataCandidate["y"].Equals(yTestCaseCoordinate)
                ).FirstOrDefault();

                char cellType = classroomLayoutMatrix[yTestCaseCoordinate][xTestCaseCoordinate];

                // Empty cell (the floor) confirmation
                if (cellType.Equals(' '))
                {
                    Assert.IsNull(cellData, "The cell at the coordinates {0}, {1} is not a floor cell", xTestCaseCoordinate, yTestCaseCoordinate);
                }
                // Table confirmation
                else if (cellType.Equals('B'))
                {
                    Assert.IsNotNull(cellData, "The cell at the coordinates {0}, {1} is null, not a table cell", xTestCaseCoordinate, yTestCaseCoordinate);
                    bool isTable = (string)cellData["cellType"] == "table";
                    Assert.IsTrue(isTable, "The cell at the coordinates {0}, {1} is not a table cell", xTestCaseCoordinate, yTestCaseCoordinate);
                }
                // Whiteboard confirmation
                else if (cellType.Equals('T'))
                {
                    Assert.IsNotNull(cellData, "The cell at the coordinates {0}, {1} is null, not a whiteboard cell", xTestCaseCoordinate, yTestCaseCoordinate);
                    bool isWhiteboard = (string)cellData["cellType"] == "whiteboard";
                    Assert.IsTrue(isWhiteboard, "The cell at the coordinates {0}, {1} is not a whiteboard cell", xTestCaseCoordinate, yTestCaseCoordinate);
                }
                // Invalid cell type
                else
                {
                    Assert.Fail("Cell type '{0}' at x:{1}, y:{2} is not a valid cell type.", cellType, xTestCaseCoordinate, yTestCaseCoordinate);
                }
            }


            Utils.TearDown(app);
        }
    }
}



