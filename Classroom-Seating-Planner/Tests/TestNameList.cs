using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using System.Diagnostics;
using FlaUIElement = FlaUI.Core.AutomationElements;
using ListActions = Classroom_Seating_Planner.ListActions;

namespace Tests
{
    [TestClass]
    public class TestNameList
    {
        [TestMethod]
        public void TestRandomizer()
        {
            bool hasStudentListOrderChanged(string[] namesOld, string[] namesNew)
            {
                return !namesOld.SequenceEqual(namesNew);
            }

            bool hasListContentChanged(string[] namesOld, string[] namesNew)
            {
                // Check that every name in the old list exists in the new list
                foreach (string name in namesOld)
                {
                    if (!namesNew.Contains(name))
                    {
                        return true;
                    }
                }

                return false;
            }

            string[] getArrayOfStudentListBeforeShuffle(FlaUIElement.Window window, ConditionFactory cf, string listBoxAutomationId)
            {
                // Extract an array of student names from the ListBox element in the UI
                return ListActions.GetListBoxItemsAsArray(window, cf, listBoxAutomationId);
            }

            string[] clickRandomizeButtonAndGetNewArray(FlaUIElement.Window window, ConditionFactory cf, string listBoxAutomationId)
            {
                // Find and press the randomizer button
                FlaUIElement.AutomationElement randomizeButton = window.FindFirstDescendant(cf.ByAutomationId("RandomizeSeatingButton")).AsButton();
                randomizeButton.Click();

                // Extract an array of student names from the ListBox element in the UI
                string[] namesNew = ListActions.GetListBoxItemsAsArray(window, cf, listBoxAutomationId);

                return namesNew;
            }

            // Find and run the application
            FlaUI.Core.Application app = FlaUI.Core.Application.Launch("..\\..\\..\\..\\Classroom-Seating-Planner\\bin\\Debug\\net8.0-windows\\win-x64\\Classroom-Seating-Planner.exe");
            using FlaUI.UIA3.UIA3Automation automation = new();

            // Find the main window for the purpose of finding elements
            Window window = app.GetMainWindow(automation);
            ConditionFactory cf = new(new UIA3PropertyLibrary());

            string studentListAutomationId = "StudentList";

            string[] namesOld = getArrayOfStudentListBeforeShuffle(window, cf, studentListAutomationId);
            string[] namesNew = clickRandomizeButtonAndGetNewArray(window, cf, studentListAutomationId);

            // Custom error messages for asserts
            string errorMessageStudentListOrderUnchanged = "Test failed because the order of the student list has not changed.";
            string errorMessageNamesAreDifferent = "Test failed because the content of the generated student list is not the same as the list before generating";

            // Trigger the randomizing function and assert that a new, randomized, class list is generated and make sure all the names are the same
            Assert.IsTrue(hasStudentListOrderChanged(namesOld, namesNew), errorMessageStudentListOrderUnchanged);
            Assert.IsFalse(hasListContentChanged(namesOld, namesNew), errorMessageNamesAreDifferent);

            namesOld = getArrayOfStudentListBeforeShuffle(window, cf, studentListAutomationId);
            namesNew = clickRandomizeButtonAndGetNewArray(window, cf, studentListAutomationId);

            // Test one more time to make sure that the list can be scrambled again
            Assert.IsTrue(hasStudentListOrderChanged(namesOld, namesNew), errorMessageStudentListOrderUnchanged);
            Assert.IsFalse(hasListContentChanged(namesOld, namesNew), errorMessageNamesAreDifferent);

            app.Close();
        }

        [TestMethod, Timeout(3000)]
        public void TestShuffleShortList()
        {
            // Test that ListActions.Shuffle can handle a list with 0 items
            List<string> testListLengthZero = [];
            List<string> testListLengthZeroShuffled = ListActions.Shuffle(testListLengthZero);
            Assert.IsNotNull(testListLengthZeroShuffled);

            // Test that ListActions.Shuffle can handle a list with 1 item
            List<string> testListLengthOne = ["Name1"];
            List<string> testListLengthOneShuffled = ListActions.Shuffle(testListLengthOne);
            Assert.IsNotNull(testListLengthOneShuffled);

            // Test that ListActions.Shuffle can handle a list with 2 items and make sure the order of the lists are different
            List<string> testListLengthTwo = ["Name1", "Name2"];
            List<string> testListLengthTwoShuffled = ListActions.Shuffle(testListLengthTwo);
            Assert.IsFalse(testListLengthTwo.SequenceEqual(testListLengthTwoShuffled));
        }
    }
}