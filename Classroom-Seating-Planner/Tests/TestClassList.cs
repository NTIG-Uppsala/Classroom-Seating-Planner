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
    }
}