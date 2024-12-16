using FlaUIElement = FlaUI.Core.AutomationElements;
using Tests;
using System.Diagnostics;

namespace A02_Automatic_Tests
{
    [TestClass]
    public class TestConstraints
    {
        [TestMethod]
        public void ExtremeConstraintCloseToWhiteboardTest()
        {
            List<string> testingClassroomLayout = [
                "   TTTT",
                "",
                "     B",
                "   B",
                "",
                "BBBB BBB BB",
                "",
                "BBB BBB BBB"
                ];

            List<string> testingClassList = [
                "Emma Andersson",
                "William Eriksson",
                "Olivia Karlsson: långtfrån Lucas Johansson (5)",
                "Lucas Johansson",
                "Alice Svensson: långtfrån Ella Larsson (7)",
                "Elias Nilsson",
                "Ella Larsson: nära tavlan (300)",
                "Noah Persson",
                "Alva Olsson: bredvid Noah Persson (9)",
                "Liam Lindberg",
                "Ebba Bergström: nära tavlan (700)",
                "Oscar Holmgren: bredvid Emma Andersson (4)",
                "Nora Lundqvist: intebredvid Liam Lindberg (3) / nära Ebba Bergström (8)",
                "Alexander Håkansson",
                "Wilma Strömberg",
                "Hugo Sandberg"
                ];

            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(testingClassList: testingClassList, testingClassroomLayout: testingClassroomLayout); // SetUp has optional arguments that may be useful for certain tests

            
            Utils.XAMLHandler.ClickRandomizeSeatingButton(window, cf);
            
            // Find all the tables
            List<FlaUIElement.AutomationElement>? allTables = Utils.XAMLHandler.GetAllElementsByHelpText(window, cf, key: "cellType", value: "table");
            Assert.IsNotNull(allTables);

            // Assert that the student with the highest priority is seated in the correct position
            FlaUIElement.AutomationElement? firstClosestToWhiteboardTable = allTables.Where(table => table.Name.Equals("Ebba Bergström")).ToList().FirstOrDefault();
            Assert.IsNotNull(firstClosestToWhiteboardTable, "Could not find table with Ebba Bergström");

            // Get table data from help text
            IDictionary<string, object>? firstClosestToWhiteboardTableData = Utils.XAMLHandler.ParseStringToObject(firstClosestToWhiteboardTable.HelpText);
            Assert.IsNotNull(firstClosestToWhiteboardTableData, "Could not find table with Ebba Bergström");
            Assert.IsTrue(firstClosestToWhiteboardTableData["x"].Equals(5) && firstClosestToWhiteboardTableData["y"].Equals(2), "Ebba Bergström is not seated at x=5 and y=2");

            // Assert that the student with the second highest priority is seated in the correct position
            FlaUIElement.AutomationElement? secondClosestToWhiteboardTable = allTables.Where(table => table.Name.Equals("Ella Larsson")).ToList().FirstOrDefault();
            Assert.IsNotNull(secondClosestToWhiteboardTable, "Could not find table with Ella Larsson");

            // Get table data from help text
            IDictionary<string, object>? secondClosestToWhiteboardTableData = Utils.XAMLHandler.ParseStringToObject(secondClosestToWhiteboardTable.HelpText);
            Assert.IsNotNull(secondClosestToWhiteboardTableData, "Could not find table with Ella Larsson");
            Assert.IsTrue(secondClosestToWhiteboardTableData["x"].Equals(3) && secondClosestToWhiteboardTableData["y"].Equals(3), "Ella Larsson is not seated at x=3 and y=3");


            Utils.TearDown(app);
        }

        [TestMethod]
        public void ExtremeConstraintFarFromWhiteboardTest()
        {
            List<string> testingClassroomLayout = [
                "   TTTT",
                "",
                "BBBB BBB BB",
                "",
                "BBB BBB BBB",
                "",
                "     B",
                "   B"
                ];

            List<string> testingClassList = [
                "Emma Andersson",
                "William Eriksson",
                "Olivia Karlsson: långtfrån Lucas Johansson (5)",
                "Lucas Johansson",
                "Alice Svensson: långtfrån Ella Larsson (7)",
                "Elias Nilsson",
                "Ella Larsson: långtfrån tavlan (300)",
                "Noah Persson",
                "Alva Olsson: bredvid Noah Persson (9)",
                "Liam Lindberg",
                "Ebba Bergström: långtfrån tavlan (700)",
                "Oscar Holmgren: bredvid Emma Andersson (4)",
                "Nora Lundqvist: intebredvid Liam Lindberg (3) / nära Ebba Bergström (8)",
                "Alexander Håkansson",
                "Wilma Strömberg",
                "Hugo Sandberg"
                ];

            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(testingClassList: testingClassList, testingClassroomLayout: testingClassroomLayout); // SetUp has optional arguments that may be useful for certain tests


            Utils.XAMLHandler.ClickRandomizeSeatingButton(window, cf);

            // Find all the tables
            List<FlaUIElement.AutomationElement>? allTables = Utils.XAMLHandler.GetAllElementsByHelpText(window, cf, key: "cellType", value: "table");
            Assert.IsNotNull(allTables);

            // Assert that the student with the highest priority is seated in the correct position
            FlaUIElement.AutomationElement? firstFarthestFromWhiteboardTable = allTables.Where(table => table.Name.Equals("Ebba Bergström")).ToList().FirstOrDefault();
            Assert.IsNotNull(firstFarthestFromWhiteboardTable, "Could not find table with Ebba Bergström");

            // Get table data from help text
            IDictionary<string, object>? firstFarthestFromWhiteboardTableData = Utils.XAMLHandler.ParseStringToObject(firstFarthestFromWhiteboardTable.HelpText);
            Assert.IsNotNull(firstFarthestFromWhiteboardTableData, "Could not find table with Ebba Bergström");
            Assert.IsTrue(firstFarthestFromWhiteboardTableData["x"].Equals(3) && firstFarthestFromWhiteboardTableData["y"].Equals(7), "Ebba Bergström is not seated at x=3 and y=7");

            // Assert that the student with the second highest priority is seated in the correct position
            FlaUIElement.AutomationElement? secondFarthestFromWhiteboardTable = allTables.Where(table => table.Name.Equals("Ella Larsson")).ToList().FirstOrDefault();
            Assert.IsNotNull(secondFarthestFromWhiteboardTable, "Could not find table with Ella Larsson");

            // Get table data from help text
            IDictionary<string, object>? secondFarthestFromWhiteboardTableData = Utils.XAMLHandler.ParseStringToObject(secondFarthestFromWhiteboardTable.HelpText);
            Assert.IsNotNull(secondFarthestFromWhiteboardTableData, "Could not find table with Ella Larsson");
            Assert.IsTrue(secondFarthestFromWhiteboardTableData["x"].Equals(5) && secondFarthestFromWhiteboardTableData["y"].Equals(6), "Ella Larsson is not seated at x=5 and y=6");


            Utils.TearDown(app);
        }

        [TestMethod]
        public void ExtremeConstraintAdjacentTest()
        {
            List<string> testingClassroomLayout = [
                "   TTTT",
                "",
                "      BB",
                "   ",
                "",
                "BBBB BBB BB",
                "",
                "BBB BBB BBB"
                ];

            List<string> testingClassList = [
                "Emma Andersson",
                "William Eriksson",
                "Olivia Karlsson: långtfrån Lucas Johansson (5)",
                "Lucas Johansson",
                "Alice Svensson: långtfrån Ella Larsson (7)",
                "Elias Nilsson",
                "Ella Larsson:",
                "Noah Persson",
                "Alva Olsson: bredvid Noah Persson (9)",
                "Liam Lindberg",
                "Ebba Bergström: nära tavlan (700) / bredvid Ella Larsson (300)",
                "Oscar Holmgren:",
                "Nora Lundqvist: intebredvid Liam Lindberg (3) / nära Ebba Bergström (8)",
                "Alexander Håkansson",
                "Wilma Strömberg",
                "Hugo Sandberg"
                ];

            // Set up/ start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(testingClassList: testingClassList, testingClassroomLayout: testingClassroomLayout); // SetUp has optional arguments that may be useful for certain tests


            Utils.XAMLHandler.ClickRandomizeSeatingButton(window, cf);

            // Find all the tables
            List<FlaUIElement.AutomationElement>? allTables = Utils.XAMLHandler.GetAllElementsByHelpText(window, cf, key: "cellType", value: "table");
            Assert.IsNotNull(allTables);

            // Assert that the student with the highest priority is seated in the correct position
            FlaUIElement.AutomationElement? firstAdjacentStudentTable = allTables.Where(table => table.Name.Equals("Ebba Bergström")).ToList().FirstOrDefault();
            Assert.IsNotNull(firstAdjacentStudentTable, "Could not find table with Ebba Bergström");

            // Get table data from help text
            IDictionary<string, object>? firstAdjacentStudentTableData = Utils.XAMLHandler.ParseStringToObject(firstAdjacentStudentTable.HelpText);
            Assert.IsNotNull(firstAdjacentStudentTableData, "Could not find table with Ebba Bergström");
            Assert.IsTrue(firstAdjacentStudentTableData["x"].Equals(6) && firstAdjacentStudentTableData["y"].Equals(2), "Ebba Bergström is not seated at x=6 and y=2");

            // Assert that the student with the second highest priority is seated in the correct position
            FlaUIElement.AutomationElement? secondAdjacentStudentTable = allTables.Where(table => table.Name.Equals("Ella Larsson")).ToList().FirstOrDefault();
            Assert.IsNotNull(secondAdjacentStudentTable, "Could not find table with Ella Larsson");

            // Get table data from help text
            IDictionary<string, object>? secondAdjacentStudentTableData = Utils.XAMLHandler.ParseStringToObject(secondAdjacentStudentTable.HelpText);
            Assert.IsNotNull(secondAdjacentStudentTableData, "Could not find table with Ella Larsson");
            Assert.IsTrue(secondAdjacentStudentTableData["x"].Equals(3) && secondAdjacentStudentTableData["y"].Equals(3), "Ella Larsson is not seated at x=3 and y=3");


            Utils.TearDown(app);
        }

        [TestMethod]
        public void ExtremeConstraintNotAdjacentTest()
        {
            List<string> testingClassroomLayout = [
                "   TTTT",
                "",
                "BBB",
                ];

            List<string> testingClassList = [
                "Ella Larsson",
                "Noah Persson",
                "Ebba Bergström: nära tavlan (200) / intebredvid Ella Larsson (700)",
                ];

            // Set up/ start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(testingClassList: testingClassList, testingClassroomLayout: testingClassroomLayout); // SetUp has optional arguments that may be useful for certain tests


            Utils.XAMLHandler.ClickRandomizeSeatingButton(window, cf);

            // Find all the tables
            List<FlaUIElement.AutomationElement>? allTables = Utils.XAMLHandler.GetAllElementsByHelpText(window, cf, key: "cellType", value: "table");
            Assert.IsNotNull(allTables);

            // Assert that the student with the highest priority is seated in the correct position
            FlaUIElement.AutomationElement? firstNotAdjacentStudentTable = allTables.Where(table => table.Name.Equals("Ebba Bergström")).ToList().FirstOrDefault();
            Assert.IsNotNull(firstNotAdjacentStudentTable, "Could not find table with Ebba Bergström");

            // Get table data from help text
            IDictionary<string, object>? firstNotAdjacentStudentTableData = Utils.XAMLHandler.ParseStringToObject(firstNotAdjacentStudentTable.HelpText);
            Assert.IsNotNull(firstNotAdjacentStudentTableData, "Could not find table with Ebba Bergström");
            Assert.IsTrue(firstNotAdjacentStudentTableData["x"].Equals(2) && firstNotAdjacentStudentTableData["y"].Equals(2), "Ebba Bergström is not seated at x=2 and y=2");

            // Assert that the student with the second highest priority is seated in the correct position
            FlaUIElement.AutomationElement? secondNotAdjacentStudentTable = allTables.Where(table => table.Name.Equals("Ella Larsson")).ToList().FirstOrDefault();
            Assert.IsNotNull(secondNotAdjacentStudentTable, "Could not find table with Ella Larsson");

            // Get table data from help text
            IDictionary<string, object>? secondNotAdjacentStudentTableData = Utils.XAMLHandler.ParseStringToObject(secondNotAdjacentStudentTable.HelpText);
            Assert.IsNotNull(secondNotAdjacentStudentTableData, "Could not find table with Ella Larsson");
            Assert.IsTrue(secondNotAdjacentStudentTableData["x"].Equals(0) && secondNotAdjacentStudentTableData["y"].Equals(3), "Ella Larsson is not seated at x=0 and y=3");


            Utils.TearDown(app);
        }

        [TestMethod]
        public void RandomnessOfConstrainedSeatingTest()
        {
            //
            // This test follows one student throughout multiple iterations of seating the class.
            // It checks that after the iterations that the student got placed at different tables.
            // The algoritm should have some degree of randomness to get more variation.
            //
            // Fail conditions:
            //  - The followed student was placed at the same table every iteration.
            //  - The followed student was placed out of bounds.
            //

            List<string> testingClassroomLayout = [
                "   TTTT",
                "",
                "    B",
                "    B",
                "    B",
                "   BBB",
                "   BBB",
                "   BBB",
                "   BBB",
                "BBB",
                "",
                "      B",
                "",
                "BBB   B BBB BB",
                "",
                "BBB   BBB BBB",
                "",
                "      B",
                "  B",
                "",
                "BBB   BBB BBB",
                "",
                "      B",
                "  B"
                ];

            List<string> testingClassList = [
                "Ziggy Stardust",
                "Frodo Baggins",
                "Darth Vader",
                "Galadriel Silverleaf",
                "Sparky McFluff",
                "Waldo B. Lost",
                "กาญจนา McSix",
                "Gandalf the Grey",
                "Ulysses 'Snakehands' McDougall",
                "Venkatanarasimharajuvaripeta Wumpus",
                "Shivankumaraswamy Krishnamurthy Raghunath",
                "الحسيني",
                "Muhammad Abdelrahman ibn Al-Mahmoud al-Farouq",
                "Papadopoulos-Alexandropoulos Firestorm",
                "明张",
                "Pipkin Puddleduck",
                "Aleksandrovich Dimitrov Petrovskaya Ivanov",
                "Per-Göran Karlsson von Heidenstam af Skånesläkten",
                "Wiggles Snickerbottom",
                "Zephyr Nightwind",
                "Doodlebug Sparklestep",
                "Sir Adrian Carton de Wiart",
                "Tinkerbell Twinkletoes",
                "Bo Li",
                "Dinglehopper Wobblesworth: nära tavlan",
                "Kǎi McQuirk",
                "Fizzlewhit Wobblebottom",
                "鈴木 健太",
                "Jo Wu",
                "Le To",
                "Örjan Johansson Florist",
                "Främling Skådespelare",
                "Émil Låås",
                "Astrid Björk",
                "Marcus Aurelius",
                "Leonardo da Vinci",
                "Newton Applebottom",
                "Athena Wiseheart",
                "Hercules Strongarm",
                "Jane Eyre",
                "Sherlock Holmes",
                "Dr. Watson",
                "Amelia Earhart",
                "Marie Curie",
                "Ada Lovelace",
                "Alan Turing",
                "Nikola Tesla",
                "Thomas Edison",
                "Cleopatra",
                "Julius Caesar",
            ];

            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(testingClassList: testingClassList, testingClassroomLayout: testingClassroomLayout); // SetUp has optional arguments that may be useful for certain tests

            List<Dictionary<string, float>> coordsList = [];

            for (int i = 0; i < 10; i++)
            {
                Utils.XAMLHandler.ClickRandomizeSeatingButton(window, cf);

                // Find all the tables
                List<FlaUIElement.AutomationElement>? allTables = Utils.XAMLHandler.GetAllElementsByHelpText(window, cf, key: "cellType", value: "table");
                Assert.IsNotNull(allTables);

                FlaUIElement.AutomationElement? constrainedStudentTable = allTables.Where(table => table.Name.Equals("Dinglehopper Wobblesworth")).ToList().FirstOrDefault();
                Assert.IsNotNull(constrainedStudentTable, "Could not find a table with Dinglehopper Wobblesworth");

                // Get table data from help text
                IDictionary<string, object>? constrainedStudentTableData = Utils.XAMLHandler.ParseStringToObject(constrainedStudentTable.HelpText);
                Assert.IsNotNull(constrainedStudentTableData);

                // Save the position
                Dictionary<string, float> coords = new() { {"x", (float)constrainedStudentTableData["x"] }, {"y", (float)constrainedStudentTableData["y"] } };
                coordsList.Add(coords);
            }

            // Filter out duplicates
            coordsList = coordsList.Distinct().ToList();

            List<float> xCoords = coordsList.Select(coordinate => coordinate["x"]).ToList();
            List<float> yCoords = coordsList.Select(coordinate => coordinate["y"]).ToList();

            // Assert that the seating is not the same every time and that the seating is not at an invalid position
            Assert.IsFalse(coordsList.Count.Equals(1), "The student was placed at the same position every time.");
            Assert.IsTrue(xCoords.Max() < 6 && xCoords.Min() > 2, "The student was placed at an invalid x coordinate.");
            Assert.IsTrue(yCoords.Max() < 9 && yCoords.Min() > 1, "The student was placed at an invalid y coordinate.");


            Utils.TearDown(app);
        }

        [TestMethod]
        public void ALotOfConstraintsStressTest()
        {
            //
            // This test is a stress test to see if the program can handle 
            // an extreme amount of constraints at the same time.
            //
            // Fail conditions:
            //  - Not all students were seated.
            //

            List<string> testingClassList = [
                "Ziggy Stardust: långtfrån tavlan (4)",
                "Frodo Baggins: bredvid Doodlebug Sparklestep (9)",
                "Darth Vader: bredvid Fizzlewhit Wobblebottom (2)",
                "Galadriel Silverleaf: intebredvid Zephyr Nightwind (7)",
                "Sparky McFluff: långtfrån tavlan (6)",
                "Waldo B. Lost: intebredvid Wiggles Snickerbottom (1)",
                "กาญจนา McSix: intebredvid Fizzlewhit Wobblebottom (5)",
                "Gandalf the Grey: bredvid Örjan Johansson Florist (3)",
                "Ulysses 'Snakehands' McDougall: långtfrån tavlan (8)",
                "Venkatanarasimharajuvaripeta Wumpus: långtfrån tavlan (10)",
                "Shivankumaraswamy Krishnamurthy Raghunath: nära Venkatanarasimharajuvaripeta Wumpus (2)",
                "الحسيني: nära Papadopoulos-Alexandropoulos Firestorm (6)",
                "Muhammad Abdelrahman ibn Al-Mahmoud al-Farouq: bredvid กาญจนา McSix (7)",
                "Papadopoulos-Alexandropoulos Firestorm: intebredvid Sparky McFluff",
                "明张: långtfrån tavlan (9)",
                "Pipkin Puddleduck: långtfrån tavlan (3)",
                "Aleksandrovich Dimitrov Petrovskaya Ivanov: långtfrån tavlan (1)",
                "Per-Göran Karlsson von Heidenstam af Skånesläkten: långtfrån tavlan",
                "Wiggles Snickerbottom: långtfrån tavlan (8)",
                "Zephyr Nightwind: nära Ulysses 'Snakehands' McDougall (2)",
                "Doodlebug Sparklestep: nära กาญจนา McSix (10)",
                "Sir Adrian Carton de Wiart: intebredvid Gandalf the Grey (6)",
                "Tinkerbell Twinkletoes: långtfrån tavlan (9)",
                "Bo Li: nära tavlan (3)",
                "Dinglehopper Wobblesworth: bredvid Venkatanarasimharajuvaripeta Wumpus (4)",
                "Kǎi McQuirk: bredvid 明张 (5)",
                "Fizzlewhit Wobblebottom: intebredvid Frodo Baggins (7)",
                "鈴木 健太: nära Bo Li (8)",
                "Jo Wu: bredvid 明张",
                "Le To: långtfrån tavlan (1)",
                "Örjan Johansson Florist: intebredvid Waldo B.Lost (9)",
                "Främling Skådespelare: bredvid Sir Adrian Carton de Wiart (2)",
                "Émil Låås: bredvid Zephyr Nightwind (10)"
            ];

            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(testingClassList: testingClassList); // SetUp has optional arguments that may be useful for certain tests


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
    }
}