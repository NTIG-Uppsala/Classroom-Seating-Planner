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
            bool hasStudentListOrderChanged(FlaUIElement.Window window, ConditionFactory cf, string[] namesOld, string[] namesNew)
            {
                return namesOld != namesNew;
            }

            bool hasListContentChanged(FlaUIElement.Window window, ConditionFactory cf, string[] namesOld, string[] namesNew)
            {
                // Check that every name in the old list exists in the new list
                bool hasContentChanged = false;
                foreach (string name in namesOld)
                {
                    if (!namesNew.Contains(name))
                    {
                        hasContentChanged = true;
                    }
                }

                return hasContentChanged;
            }

            // Find and run the application
            FlaUI.Core.Application app = FlaUI.Core.Application.Launch("..\\..\\..\\..\\Classroom-Seating-Planner\\bin\\Debug\\net8.0-windows\\Classroom-Seating-Planner.exe");
            using FlaUI.UIA3.UIA3Automation automation = new();

            // Find the main window for the purpose of finding elements
            Window window = app.GetMainWindow(automation);
            ConditionFactory cf = new(new UIA3PropertyLibrary());

            // Find the ListBox where the class list is displayed and place the class list into a string-array
            FlaUIElement.ListBox listBoxStudentList = window.FindFirstDescendant(cf.ByAutomationId("ListBoxStudentList")).AsListBox();
            ListBoxItem[] studentListOld = listBoxStudentList.Items;
            string[] namesOld = studentListOld.Select(listItem => listItem.Text).ToArray();

            // Find and press the randomizer button
            FlaUIElement.Button randomizeButton = window.FindFirstDescendant(cf.ByAutomationId("ButtonRandomizeSeating")).AsButton();
            randomizeButton.Click();

            // Update the variable where the ListBox is stored and store the randomized class list in a new variable
            listBoxStudentList = window.FindFirstDescendant(cf.ByAutomationId("ListBoxStudentList")).AsListBox();
            ListBoxItem[] studentListNew = listBoxStudentList.Items;
            string[] namesNew = studentListNew.Select(listItem => listItem.Text).ToArray();

            // Custom error messages for asserts
            string errorMessageStudentListOrderUnchanged = "Test failed because the order of the student list has not changed.";
            string errorMessageNamesAreDifferent = "Test failed because the content of the generated student list is not the same as the list before generating";

            // Trigger the randomizing function and assert that a new, randomized, class list is generated and make sure all the names are the same
            Assert.IsTrue(hasStudentListOrderChanged(window, cf, namesOld, namesNew), errorMessageStudentListOrderUnchanged);
            Assert.IsTrue(hasListContentChanged(window, cf, namesOld, namesNew), errorMessageNamesAreDifferent);

            // Test one more time to make sure that the list can be scrambled again
            Assert.IsTrue(hasStudentListOrderChanged(window, cf, namesOld, namesNew), errorMessageStudentListOrderUnchanged);
            Assert.IsTrue(hasListContentChanged(window, cf, namesOld, namesNew), errorMessageNamesAreDifferent);

            app.Close();
        }
    }
}