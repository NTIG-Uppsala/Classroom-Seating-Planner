using System.Dynamic;
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
            "      BBBB\n" +
            "\n" +
            " BB BB  BB BB\n" +
            "\n" +
            "B BB BB  BB";

        // TODO - this will change after file reading is implemented
        private int expectedColumns = 14;
        private int expectedRows = 10;

        // Parses a object formatted string and converts it to an object e.g. "x:1, y:5" -> { x: 1, y: 5 }
        // Currenlty supports bool and int data types (falls back to string)
        private static IDictionary<string, object> ParseStringToObject(string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString))
            {
                throw new ArgumentException("Input string cannot be null or empty.", nameof(inputString));
            }

            IDictionary<string, object> returnObject = new ExpandoObject() as IDictionary<string, object>;

            List<string> properties = inputString.Split(",")
                .Select((property) => property.Trim())
                .ToList();

            foreach (string property in properties)
            {
                List<string> keyValuePair = property.Split(":")
                    .Select((keyOrValue) => keyOrValue.Trim())
                    .ToList();

                // If the property is not in the format "key: value" or if there are any empty strings, throw an exception
                if (keyValuePair.Count != 2 || keyValuePair.Any((element) => string.IsNullOrEmpty(element)))
                {
                    throw new FormatException($"Invalid property format in '{property}', expected \"key: value\"");
                }

                string key = keyValuePair[0];
                var value = keyValuePair[1];

                // Determine the type of the value and convert it dynamically
                if (bool.TryParse(value, out bool boolValue))
                {
                    returnObject[key] = boolValue;
                }
                else if (int.TryParse(value, out int intValue))
                {
                    returnObject[key] = intValue;
                }
                else
                {
                    // Fallback, just store it as a string
                    returnObject[key] = value;
                }
            }

            return returnObject.ToDictionary();
        }

        // TODO - integrate into utils (GetAllBy(identifierType, , , )...) when refactor
        // Gets all the elements that have "cell: true" in their helpText
        private static List<FlaUIElement.AutomationElement> GetAllCells(FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf, FlaUI.Core.Definitions.ControlType? controlType = null)
        {
            // Get all elements
            List<FlaUIElement.AutomationElement> allElements = window.FindAllDescendants(cf.ByFrameworkId("WPF")).ToList();

            List<FlaUIElement.AutomationElement> allCells = allElements.Where(element =>
                    !string.IsNullOrEmpty(element.HelpText)
                    &&
                    (
                        element.HelpText.Contains("cell")
                        &&
                        TestTableLayout.ParseStringToObject(element.HelpText)["cell"].Equals(true)
                    )
                    &&
                    (controlType == null || element.ControlType.Equals(controlType))
                ).ToList();

            return allCells;
        }

        [TestMethod]
        public void ClassroomSizeTest()
        {
            // Setup/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(); // SetUp has optional arguments that may be useful for certain tests


            // Get all the cells (table and whiteboard cells)
            List<FlaUIElement.AutomationElement> allCells = GetAllCells(window, cf);

            // Make a list of all the cells data
            List<IDictionary<string, object>> cellObjectList = new();
            foreach (FlaUIElement.AutomationElement cell in allCells)
            {
                cellObjectList.Add(TestTableLayout.ParseStringToObject(cell.HelpText));
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


            // Get all the cells (table and whiteboard cells)
            List<FlaUIElement.AutomationElement> allCells = GetAllCells(window, cf);

            // Sample some random table cells from the classroomLayout


            // Check if those samples have their equivalent in the tablesGrid


            // Write your test before this comment!
            Utils.TearDown(app);
        }
    }
}



