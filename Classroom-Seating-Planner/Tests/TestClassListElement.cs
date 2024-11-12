using FlaUIElement = FlaUI.Core.AutomationElements;

namespace Tests
{
    [TestClass]
    public class TestClassListElement
    {
        [TestMethod]
        public void RandomizeTestingClassListTest()
        {
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

            List<string> getClassListFromClassListElementBeforeShuffle(FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
            {
                // Extract an list of students from the ListBox element in the UI
                return Utils.XAMLManager.GetClassListFromElement(window, cf);
            }

            List<string> clickRandomizeButtonAndGetNewList(FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
            {
                Utils.XAMLManager.ClickRandomizeSeatingButton(window, cf);

                // Extract an list of students from the ListBox element in the UI
                List<string> classListNew = Utils.XAMLManager.GetClassListFromElement(window, cf);

                return classListNew;
            }

            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp();


            List<string> classListOld = getClassListFromClassListElementBeforeShuffle(window, cf);
            List<string> classListNew = clickRandomizeButtonAndGetNewList(window, cf);

            // Custom error messages for asserts
            string errorMessageClassListOrderUnchanged = "Test failed because the order of the class list has not changed.";
            string errorMessageClassListIsDifferent = "Test failed because the content of the generated class list is not the same as the list before generating";

            // Trigger the randomizing function and assert that a new, randomized, class list is generated and make sure all the names are the same
            Assert.IsTrue(hasClassListOrderChanged(classListOld, classListNew), errorMessageClassListOrderUnchanged);
            Assert.IsFalse(hasClassListContentChanged(classListOld, classListNew), errorMessageClassListIsDifferent);

            classListOld = getClassListFromClassListElementBeforeShuffle(window, cf);
            classListNew = clickRandomizeButtonAndGetNewList(window, cf);

            // Test one more time to make sure that the list can be scrambled again
            Assert.IsTrue(hasClassListOrderChanged(classListOld, classListNew), errorMessageClassListOrderUnchanged);
            Assert.IsFalse(hasClassListContentChanged(classListOld, classListNew), errorMessageClassListIsDifferent);


            Utils.TearDown(app);
        }

        [TestMethod, Timeout(3000)]
        public void RandomizeListOfZeroTest()
        {
            // Set up/start the test
            List<string> testListLengthZero = [];
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(testListLengthZero);


            // Randomize the list and check that it works
            Utils.XAMLManager.ClickRandomizeSeatingButton(window, cf);
            List<string> testListLengthZeroShuffled = Utils.XAMLManager.GetClassListFromElement(window, cf);
            Assert.IsNotNull(testListLengthZeroShuffled);


            Utils.TearDown(app);
        }

        [TestMethod, Timeout(3000)]
        public void RandomizeListOfOneTest()
        {
            // Set up/start the test
            List<string> testListLengthOne = ["Name1"];
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(testListLengthOne);


            // Randomize the list and check that it works
            Utils.XAMLManager.ClickRandomizeSeatingButton(window, cf);
            List<string> testListLengthOneShuffled = Utils.XAMLManager.GetClassListFromElement(window, cf);
            Assert.IsNotNull(testListLengthOneShuffled);


            Utils.TearDown(app);
        }

        [TestMethod, Timeout(3000)]
        public void RandomizeListOfTwoTest()
        {
            // Set up/start the test
            List<string> testListLengthTwo = ["Name1", "Name2"];
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(testListLengthTwo);


            // Randomize the list and check that it works
            Utils.XAMLManager.ClickRandomizeSeatingButton(window, cf);
            List<string> testListLengthTwoShuffled = Utils.XAMLManager.GetClassListFromElement(window, cf);
            Assert.IsNotNull(testListLengthTwoShuffled);


            Utils.TearDown(app);
        }
    }
}