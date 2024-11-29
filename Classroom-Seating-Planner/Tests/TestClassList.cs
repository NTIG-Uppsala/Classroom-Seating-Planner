using FlaUIElement = FlaUI.Core.AutomationElements;

namespace Tests
{
    [TestClass]
    public class TestClassList
    {
        [TestMethod]
        public void RandomizeTestingClassListTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp();


            bool hasClassListOrderChanged(List<string> classListOld, List<string> classListNew)
            {
                return !classListOld.SequenceEqual(classListNew);
            }

            bool hasClassListContentChanged(List<string> classListOld, List<string> classListNew)
            {
                // Check that every student in the old list exists in the new list
                foreach (string student in classListOld)
                {
                    if (!classListNew.Contains(student))
                    {
                        return true;
                    }
                }

                return false;
            }

            // Get class list before shuffle
            List<string> classListOld = Utils.XAMLHandler.GetClassListFromElement(window, cf);

            // Click randomize-button and get class list after shuffle
            Utils.XAMLHandler.ClickRandomizeSeatingButton(window, cf);
            List<string> classListNew = Utils.XAMLHandler.GetClassListFromElement(window, cf);

            // Trigger the randomizing function and assert that a new, randomized, class list is generated and make sure all the names are the same
            Assert.IsTrue(hasClassListOrderChanged(classListOld, classListNew), "Test failed because the order of the class list has not changed.");
            Assert.IsFalse(hasClassListContentChanged(classListOld, classListNew), "Test failed because the content of the generated class list is not the same as the list before generating");

            // Get class list before shuffle
            classListOld = Utils.XAMLHandler.GetClassListFromElement(window, cf);

            // Click randomize-button and get class list after shuffle
            Utils.XAMLHandler.ClickRandomizeSeatingButton(window, cf);
            classListNew = Utils.XAMLHandler.GetClassListFromElement(window, cf);

            // Test one more time to make sure that the list can be scrambled again
            Assert.IsTrue(hasClassListOrderChanged(classListOld, classListNew), "Test failed because the order of the class list has not changed.");
            Assert.IsFalse(hasClassListContentChanged(classListOld, classListNew), "Test failed because the content of the generated class list is not the same as the list before generating");


            Utils.TearDown(app);
        }

        [TestMethod, Timeout(3000)]
        public void RandomizeClassListOfZeroTest()
        {
            // Set up/start the test
            List<string> testListLengthZero = [];
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(testListLengthZero);


            // Randomize the list and check that it works
            Utils.XAMLHandler.ClickRandomizeSeatingButton(window, cf);
            List<string> testListLengthZeroShuffled = Utils.XAMLHandler.GetClassListFromElement(window, cf);
            Assert.IsNotNull(testListLengthZeroShuffled);


            Utils.TearDown(app);
        }

        [TestMethod, Timeout(3000)]
        public void RandomizeClassListOfOneTest()
        {
            // Set up/start the test
            List<string> testListLengthOne = ["Name1"];
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(testListLengthOne);


            // Randomize the list and check that it works
            Utils.XAMLHandler.ClickRandomizeSeatingButton(window, cf);
            List<string> testListLengthOneShuffled = Utils.XAMLHandler.GetClassListFromElement(window, cf);
            Assert.IsNotNull(testListLengthOneShuffled);


            Utils.TearDown(app);
        }

        [TestMethod, Timeout(3000)]
        public void RandomizeClassListOfTwoTest()
        {
            // Set up/start the test
            List<string> testListLengthTwo = ["Name1", "Name2"];
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(testListLengthTwo);


            // Randomize the list and check that it works
            Utils.XAMLHandler.ClickRandomizeSeatingButton(window, cf);
            List<string> testListLengthTwoShuffled = Utils.XAMLHandler.GetClassListFromElement(window, cf);
            Assert.IsNotNull(testListLengthTwoShuffled);


            Utils.TearDown(app);
        }

        [TestMethod]
        public void MissingClassListFileTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(ignoreTestingClassList: true, deleteClassListFile: true);

            // Assert that the correct popup is shown when the file is missing
            Utils.PopupHandler.PopupWindowContainsText(app, automation, cf, Utils.PopupHandler.missingFilePopupName, "Klasslista hittades inte");


            Utils.TearDown(app);
        }

        // Test that the application gives a popup warning when loading an empty list
        [TestMethod]
        public void EmptyClassListFileTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp([]);


            // Assert that the correct popup is shown when the empty list is loaded
            Utils.PopupHandler.PopupWindowContainsText(app, automation, cf, Utils.PopupHandler.badFilePopupName, "Klasslistan är tom");


            Utils.TearDown(app);
        }

        // Test that the application gives a popup warning when loading a default list
        [TestMethod]
        public void DefaultClassListFileTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(Utils.defaultClassList);


            // Assert that the correct popup is shown when the default list is loaded
            Utils.PopupHandler.PopupWindowContainsText(app, automation, cf, Utils.PopupHandler.badFilePopupName, "klasslistan inte har uppdaterats");


            Utils.TearDown(app);
        }

        [TestMethod]
        public void TooManyStudentsTest()
        {
            // This is defined to be independent of the default testing list to always have a list of 33 names
            // to test against 24 tables to make sure that the program displays a warning when it should
            List<string> testClassList =
            [
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
                "Dinglehopper Wobblesworth",
                "Kǎi McQuirk",
                "Fizzlewhit Wobblebottom",
                "鈴木 健太",
                "Jo Wu",
                "Le To",
                "Örjan Johansson Florist",
                "Främling Skådespelare",
                "Émil Låås",
            ];

            List<string> testClassroomLayout =
            [
                "     TTTT",
                "",
                "BBBB BBBB BBBB",
                "",
                "BBBB BBBB BBBB",
            ];

            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(testClassList: testClassList, testClassroomLayout: testClassroomLayout);


            // Assert that the correct popup is shown when the default list is loaded
            Utils.PopupHandler.PopupWindowContainsText(app, automation, cf, Utils.PopupHandler.badFilePopupName, "Det finns fler elever än bord");


            Utils.TearDown(app);
        }
    }
}