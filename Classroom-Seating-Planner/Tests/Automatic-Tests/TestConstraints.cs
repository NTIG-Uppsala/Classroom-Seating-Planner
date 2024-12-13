using FlaUIElement = FlaUI.Core.AutomationElements;
using Tests;
using System.Diagnostics;

namespace A02_Automatic_Tests
{
    [TestClass]
    public class TestConstraints
    {
        [TestMethod]
        public void ExtremeConstraintNearWhiteboardTest()
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

            FlaUIElement.AutomationElement ebbasTable = allTables.Where(table => table.Name.Equals("Ebba Bergström")).ToList()[0];
            IDictionary<string, object> ebbasTableObj = Utils.XAMLHandler.ParseStringToObject(ebbasTable.HelpText);
            Assert.IsTrue(ebbasTableObj["x"].Equals(5) && ebbasTableObj["y"].Equals(2), "Ebba Bergström is not seated at x=5 and y=2");

            FlaUIElement.AutomationElement ellasTable = allTables.Where(table => table.Name.Equals("Ella Larsson")).ToList()[0];
            IDictionary<string, object> ellasTableObj = Utils.XAMLHandler.ParseStringToObject(ellasTable.HelpText);
            Assert.IsTrue(ellasTableObj["x"].Equals(3) && ellasTableObj["y"].Equals(3), "Ella Larsson is not seated at x=3 and y=3");


            Utils.TearDown(app);
        }

        [TestMethod]
        public void ExtremeConstraintTestAdjacent()
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

            //List<string> testingClassList = [
            //    "Emma Andersson",
            //    "William Eriksson",
            //    "Olivia Karlsson: långtfrån Lucas Johansson (5)",
            //    "Lucas Johansson",
            //    "Alice Svensson: långtfrån Ella Larsson (7)",
            //    "Elias Nilsson",
            //    "Ella Larsson: nära tavlan (300)",
            //    "Noah Persson",
            //    "Alva Olsson: bredvid Noah Persson (9)",
            //    "Liam Lindberg",
            //    "Ebba Bergström: nära tavlan (700)",
            //    "Oscar Holmgren: bredvid Emma Andersson (4)",
            //    "Nora Lundqvist: intebredvid Liam Lindberg (3) / nära Ebba Bergström (8)",
            //    "Alexander Håkansson",
            //    "Wilma Strömberg",
            //    "Hugo Sandberg"
            //    ];

            // Set up/start the test
            //(FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
            //    = Utils.SetUp(testingClassList: testingClassList, testingClassroomLayout: testingClassroomLayout); // SetUp has optional arguments that may be useful for certain tests


            //Utils.XAMLHandler.ClickRandomizeSeatingButton(window, cf);

            //// Find all the tables
            //List<FlaUIElement.AutomationElement>? allTables = Utils.XAMLHandler.GetAllElementsByHelpText(window, cf, key: "cellType", value: "table");
            //Assert.IsNotNull(allTables);

            //FlaUIElement.AutomationElement ebbasTable = allTables.Where(table => table.Name.Equals("Ebba Bergström")).ToList()[0];
            //IDictionary<string, object> ebbasTableObj = Utils.XAMLHandler.ParseStringToObject(ebbasTable.HelpText);
            //Assert.IsTrue(ebbasTableObj["x"].Equals(5) && ebbasTableObj["y"].Equals(2), "Ebba Bergström is not seated at x=5 and y=2");

            //FlaUIElement.AutomationElement ellasTable = allTables.Where(table => table.Name.Equals("Ella Larsson")).ToList()[0];
            //IDictionary<string, object> ellasTableObj = Utils.XAMLHandler.ParseStringToObject(ellasTable.HelpText);
            //Assert.IsTrue(ellasTableObj["x"].Equals(3) && ellasTableObj["y"].Equals(3), "Ella Larsson is not seated at x=3 and y=3");


            //Utils.TearDown(app);
        }

        [TestMethod]
        public void ALotOfConstraintsStressTest() // AllConstrainedStudentsTest?
        {
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