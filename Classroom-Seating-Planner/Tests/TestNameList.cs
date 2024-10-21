using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using System.Diagnostics;
using FlaUIElement = FlaUI.Core.AutomationElements;
namespace Tests
{
    [TestClass]
    public class TestNameList
    {
        [TestMethod]
        public void TestRandomizer()
        {
            static List<bool> hasStudentListOrderChangedAndAreNamesIdentical(FlaUIElement.Window window, ConditionFactory cf)
            {
                // Find the ListBox where the class list is displayed and place the class list into a string-array
                FlaUIElement.ListBox listBoxStudentList = window.FindFirstDescendant(cf.ByAutomationId("ListBoxStudentList")).AsListBox();
                ListBoxItem[] studentListOld = listBoxStudentList.Items;
                string[] namesOld = studentListOld.Select(item => item.Text).ToArray();

                // Find and press the randomizer button
                FlaUIElement.Button randomizeButton = window.FindFirstDescendant(cf.ByAutomationId("ButtonRandomizeSeating")).AsButton();
                randomizeButton.Click();

                // Update the variable where the ListBox is stored and store the randomized class list in a new variable
                listBoxStudentList = window.FindFirstDescendant(cf.ByAutomationId("ListBoxStudentList")).AsListBox();
                ListBoxItem[] studentListNew = listBoxStudentList.Items;
                string[] namesNew = studentListNew.Select(item => item.Text).ToArray();

                // Check that every name in the old list exists in the new list
                bool allNamesRemain = true;
                foreach (string name in namesOld)
                {
                    if (!namesNew.Contains(name))
                    {
                        allNamesRemain = false;
                    }
                }

                // --------------------------------- DEBUG STARTS HERE ------------------------------------
                Trace.Write("List before scrambling: ");
                foreach (string name in namesOld)
                {
                    Trace.Write($"{name}, ");
                }
                Trace.Write("\n");

                Trace.Write("List after scrambling: ");
                foreach (string name in namesNew)
                {
                    Trace.Write($"{name}, ");
                }
                Trace.Write("\n");

                Trace.WriteLine($"Generated student list is different than student list before generating and all names exist in generated student list: {namesOld != namesNew && allNamesRemain}\nTest failed for one of the above reasons.");
                // --------------------------------- DEBUG ENDS HERE --------------------------------------
                return [namesOld != namesNew, allNamesRemain];
            }
            // Find and run the application
            FlaUI.Core.Application app = FlaUI.Core.Application.Launch("..\\..\\..\\..\\Classroom-Seating-Planner\\bin\\Debug\\net8.0-windows\\Classroom-Seating-Planner.exe");
            using FlaUI.UIA3.UIA3Automation automation = new();

            // Find the main window for the purpose of finding elements
            Window window = app.GetMainWindow(automation);
            ConditionFactory cf = new(new UIA3PropertyLibrary());

            // Custom error messages for asserts
            string errorMessageStudentListOrderUnchanged = "Test failed because the order of the student list has not changed.";
            string errorMessageNamesAreDifferent = "Test failed because the content of the generated student list is not the same as the list before generating";

            // Trigger the randomizing function and assert that a new, randomized, class list is generated and make sure all the names are the same
            List<bool> hasStudentListChangedBools = hasStudentListOrderChangedAndAreNamesIdentical(window, cf);
            bool hasStudentListChanged = hasStudentListChangedBools[0];
            bool allNamesAreIdentical = hasStudentListChangedBools[1];
            Assert.IsTrue(hasStudentListChanged, errorMessageStudentListOrderUnchanged);
            Assert.IsTrue(allNamesAreIdentical, errorMessageNamesAreDifferent);

            // Test one more time to make sure that the list can be scrambled again
            hasStudentListChangedBools = hasStudentListOrderChangedAndAreNamesIdentical(window, cf);
            hasStudentListChanged = hasStudentListChangedBools[0];
            allNamesAreIdentical = hasStudentListChangedBools[1];
            Assert.IsTrue(hasStudentListChanged, errorMessageStudentListOrderUnchanged);
            Assert.IsTrue(allNamesAreIdentical, errorMessageNamesAreDifferent);

            app.Close();
        }
    }
}