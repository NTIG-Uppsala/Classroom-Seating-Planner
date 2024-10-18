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
            static bool hasClassListChanged(FlaUIElement.Window window, ConditionFactory cf)
            {
                // Find the ListBox where the class list is displayed and place the class list into a string-array
                FlaUIElement.ListBox listBoxClassList = window.FindFirstDescendant(cf.ByAutomationId("ListBoxClassList")).AsListBox();
                ListBoxItem[] classListOld = listBoxClassList.Items;
                string[] namesOld = classListOld.Select(item => item.Text).ToArray();

                // Find and press the randomizer button
                FlaUIElement.Button randomizeButton = window.FindFirstDescendant(cf.ByAutomationId("ButtonRandomize")).AsButton();
                randomizeButton.Click();

                // Update the variable where the ListBox is stored and store the randomized class list in a new variable
                listBoxClassList = window.FindFirstDescendant(cf.ByAutomationId("ListBoxClassList")).AsListBox();
                ListBoxItem[] classListNew = listBoxClassList.Items;
                string[] namesNew = classListNew.Select(item => item.Text).ToArray();

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
                // --------------------------------- DEBUG ENDS HERE --------------------------------------

                return (namesOld != namesNew);
            }
            // Find and run the application
            FlaUI.Core.Application app = FlaUI.Core.Application.Launch("..\\..\\..\\..\\Classroom-Seating-Planner\\bin\\Debug\\net8.0-windows\\Classroom-Seating-Planner.exe");
            using FlaUI.UIA3.UIA3Automation automation = new();
            
            // Find the main window for the purpose of finding elements
            Window window = app.GetMainWindow(automation);
            ConditionFactory cf = new(new UIA3PropertyLibrary());

            // Trigger the randomizing function and assert that a new, randomized, class list is generated
            Assert.IsTrue(hasClassListChanged(window, cf));
            Assert.IsTrue(hasClassListChanged(window, cf));

            app.Close();
        }
    }
}