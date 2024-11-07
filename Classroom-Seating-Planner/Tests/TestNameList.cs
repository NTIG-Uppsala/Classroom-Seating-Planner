using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using System.Diagnostics;
using FlaUIElement = FlaUI.Core.AutomationElements;
using Classroom_Seating_Planner.src;

namespace Tests
{
    [TestClass]
    public class TestNameList
    {
        [TestMethod]
        public void TestRandomizer()
        {
            bool hasClassListOrderChanged(string[] classListOld, string[] classListNew)
            {
                return !classListOld.SequenceEqual(classListNew);
            }

            bool hasClassListContentChanged(string[] classListOld, string[] classListNew)
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

            string[] getClassListFromClassListElementBeforeShuffle(FlaUIElement.Window window, ConditionFactory cf, string listBoxAutomationId)
            {
                // Extract an array of students from the ListBox element in the UI
                return Utils.GetListBoxItemsAsArray(window, cf, listBoxAutomationId);
            }

            string[] clickRandomizeButtonAndGetNewArray(FlaUIElement.Window window, ConditionFactory cf, string listBoxAutomationId)
            {
                Utils.ClickRandomizeSeatingButton(window, cf);

                // Extract an array of students from the ListBox element in the UI
                string[] classListNew = Utils.GetListBoxItemsAsArray(window, cf, listBoxAutomationId);

                return classListNew;
            }

            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, Window window, ConditionFactory cf) = Utils.SetUpTest();

            string classListElementAutomationId = "ClassListElement";

            string[] classListOld = getClassListFromClassListElementBeforeShuffle(window, cf, classListElementAutomationId);
            string[] classListNew = clickRandomizeButtonAndGetNewArray(window, cf, classListElementAutomationId);

            // Custom error messages for asserts
            string errorMessageClassListOrderUnchanged = "Test failed because the order of the class list has not changed.";
            string errorMessageClassListIsDifferent = "Test failed because the content of the generated class list is not the same as the list before generating";

            // Trigger the randomizing function and assert that a new, randomized, class list is generated and make sure all the names are the same
            Assert.IsTrue(hasClassListOrderChanged(classListOld, classListNew), errorMessageClassListOrderUnchanged);
            Assert.IsFalse(hasClassListContentChanged(classListOld, classListNew), errorMessageClassListIsDifferent);

            classListOld = getClassListFromClassListElementBeforeShuffle(window, cf, classListElementAutomationId);
            classListNew = clickRandomizeButtonAndGetNewArray(window, cf, classListElementAutomationId);

            // Test one more time to make sure that the list can be scrambled again
            Assert.IsTrue(hasClassListOrderChanged(classListOld, classListNew), errorMessageClassListOrderUnchanged);
            Assert.IsFalse(hasClassListContentChanged(classListOld, classListNew), errorMessageClassListIsDifferent);

            Utils.TearDownTest(app);
        }

        //[TestMethod, Timeout(3000)]
        //public void TestShuffleShortList() // This is not a test of the name list, rather it's a test of the shuffle function
        //{
        //    // Test that Utils.ShuffleListcan handle a list with 0 items
        //    List<string> testListLengthZero = [];
        //    List<string> testListLengthZeroShuffled = Utils.ShuffleList(testListLengthZero);
        //    Assert.IsNotNull(testListLengthZeroShuffled);

        //    // Test that Utils.ShuffleListcan handle a list with 1 item
        //    List<string> testListLengthOne = ["Name1"];
        //    List<string> testListLengthOneShuffled = Utils.ShuffleList(testListLengthOne);
        //    Assert.IsNotNull(testListLengthOneShuffled);

        //    // Test that Utils.ShuffleListcan handle a list with 2 items and make sure the order of the lists are different
        //    List<string> testListLengthTwo = ["Name1", "Name2"];
        //    List<string> testListLengthTwoShuffled = Utils.ShuffleList(testListLengthTwo);
        //    Assert.IsFalse(testListLengthTwo.SequenceEqual(testListLengthTwoShuffled));
        //}
    }
}