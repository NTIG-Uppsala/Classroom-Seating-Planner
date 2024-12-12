using System.Diagnostics;
using System.Dynamic;
using FlaUIElement = FlaUI.Core.AutomationElements;

namespace Tests
{
    [TestClass]
    public class TestClassroomLayout
    {
        [TestMethod]
        public void ClassroomSizeTest()
        {
            int expectedColumns = Utils.testingClassroomLayout.Select((row) => row.Length).Max();
            int expectedRows = Utils.testingClassroomLayout.Count;

            // Setup/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(); // SetUp has optional arguments that may be useful for certain tests


            // Get all the cells (table and whiteboard cells)
            List<FlaUIElement.AutomationElement>? allCells = Utils.XAMLHandler.GetAllElementsByHelpText(window, cf, key: "cell", value: true, options: new(matchWholeString: false));
            Assert.IsNotNull(allCells);

            // Make a list of all the cells data
            List<IDictionary<string, object>> cellDataList = [];
            foreach (FlaUIElement.AutomationElement cell in allCells)
            {
                IDictionary<string, object>? cellData = Utils.XAMLHandler.ParseStringToObject(cell.HelpText);
                Assert.IsNotNull(cellData);
                Assert.IsNotNull(cellData["cellType"].ToString());

                cellDataList.Add(cellData);
            }

            // Store all the x and y values to find the max value
            List<float> xValues = cellDataList.Select(cell => (float)cell["gridX"]).ToList();
            List<float> yValues = cellDataList.Select(cell => (float)cell["gridY"]).ToList();

            // Compare the largest x and y values to the expected values
            //  -1 on the expected values because the grid is 0 indexed
            Assert.IsTrue(xValues.Max().Equals(expectedColumns - 1), "The amount of columns in the table grid is not correct");
            Assert.IsTrue(yValues.Max().Equals(expectedRows - 1), "The amount of rows in the table grid is not correct");


            Utils.TearDown(app);
        }

        [TestMethod]
        public void CellsAtExpectedPositionsTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(); // SetUp has optional arguments that may be useful for certain tests


            List<Dictionary<string, int>> testCaseCoordinatesList = [
                new() { { "gridX", 0 }, { "gridY", 2 }, }, // Should be a table cell
                new() { { "gridX", 8 }, { "gridY", 4 }, }, // Should be empty
                new() { { "gridX", 5 }, { "gridY", 0 }, }, // Should be a whiteboard cell
                new() { { "gridX", 3 }, { "gridY", 0 }, }, // Should be a whiteboard cell
                new() { { "gridX", 6 }, { "gridY", 0 }, }, // Should be a whiteboard cell
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

            // Check that the cell types are the same between the classroom layout file and the XAML grid for each sampled coordinate
            foreach (Dictionary<string, int> testCaseCoordinates in testCaseCoordinatesList)
            {
                int xTestCaseCoordinate = testCaseCoordinates["gridX"];
                int yTestCaseCoordinate = testCaseCoordinates["gridY"];

                char cellLetter = Utils.testingClassroomLayout[yTestCaseCoordinate][xTestCaseCoordinate];

                IDictionary<string, object>? cellData = cellDataList.Where((cellDataCandidate) =>
                    (float)cellDataCandidate["gridX"] <= xTestCaseCoordinate
                    &&
                    xTestCaseCoordinate <= (float)cellDataCandidate["gridX"] + (float)cellDataCandidate["width"] - 1
                    &&
                    (float)cellDataCandidate["gridY"] <= yTestCaseCoordinate
                    &&
                    yTestCaseCoordinate <= (float)cellDataCandidate["gridY"] + (float)cellDataCandidate["height"] - 1
                ).FirstOrDefault();

                // Whiteboard confirmation
                if (cellLetter.Equals('T'))
                {
                    Assert.IsNotNull(cellData, "The cell at the coordinates {0}, {1} is null, not a whiteboard cell", xTestCaseCoordinate, yTestCaseCoordinate);
                    bool isWhiteboard = ((string)cellData["cellType"]).Equals("whiteboardCover");
                    Assert.IsTrue(isWhiteboard, "The cell at the coordinates {0}, {1} is not a whiteboard cell", xTestCaseCoordinate, yTestCaseCoordinate);
                }

                // Empty cell (the floor) confirmation
                else if (cellLetter.Equals(' '))
                {
                    Assert.IsNull(cellData, "The cell at the coordinates {0}, {1} is not a floor cell", xTestCaseCoordinate, yTestCaseCoordinate);
                }

                // Table confirmation
                else if (cellLetter.Equals('B'))
                {
                    Assert.IsNotNull(cellData, "The cell at the coordinates {0}, {1} is null, not a table cell", xTestCaseCoordinate, yTestCaseCoordinate);
                    bool isTable = ((string)cellData["cellType"]).Equals("table");
                    Assert.IsTrue(isTable, "The cell at the coordinates {0}, {1} is not a table cell", xTestCaseCoordinate, yTestCaseCoordinate);
                }

                // Invalid cell type
                else
                {
                    Assert.Fail("Cell type '{0}' at x:{1}, y:{2} is not a valid cell type.", cellLetter, xTestCaseCoordinate, yTestCaseCoordinate);
                }
            }


            Utils.TearDown(app);
        }

        [TestMethod]
        public void NoTablesTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(testingClassroomLayout: new List<string>(["   TTTT"]));


            // Assert that the correct popup is shown when the default list is loaded
            Utils.PopupHandler.AnyPopupWindowContainsText(app, automation, cf, Utils.PopupHandler.badFilePopupName, "Det finns inga bord");


            Utils.TearDown(app);
        }

        [TestMethod]
        public void NoWhiteboardTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(testingClassroomLayout: new List<string>(["BBBB BBBB BBBB", "", "BBBB BBBB BBBB", "", "BBBB BBBB BBBB"]));


            // Assert that the correct popup is shown when the default list is loaded
            Utils.PopupHandler.AnyPopupWindowContainsText(app, automation, cf, Utils.PopupHandler.badFilePopupName, "Det finns ingen tavla");


            Utils.TearDown(app);
        }

        [TestMethod]
        public void MissingClassroomLayoutFileTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(ignoreTestingClassroomLayout: true, deleteClassroomLayoutFile: true);


            // Assert that the correct popup is shown when the file is missing
            Utils.PopupHandler.AnyPopupWindowContainsText(app, automation, cf, Utils.PopupHandler.missingFilePopupName, "Alla filer hittades inte");


            Utils.TearDown(app);
        }

        [TestMethod]
        public void EmptyClassroomLayoutFileTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(testingClassroomLayout: new List<string>([]));


            // Assert that the correct popup is shown when the file is empty
            Utils.PopupHandler.AnyPopupWindowContainsText(app, automation, cf, Utils.PopupHandler.badFilePopupName, "Bordskartan är tom");


            Utils.TearDown(app);
        }

        [TestMethod]
        public void WhitespaceOnlyClassroomLayoutFileTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(testingClassroomLayout: new List<string>(["  ", "", "    ", ""]));


            // Assert that the correct popup is shown when the file is empty
            Utils.PopupHandler.AnyPopupWindowContainsText(app, automation, cf, Utils.PopupHandler.badFilePopupName, "Bordskartan är tom");


            Utils.TearDown(app);
        }
    }
}



