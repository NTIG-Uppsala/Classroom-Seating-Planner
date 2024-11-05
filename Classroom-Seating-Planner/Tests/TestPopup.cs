using Classroom_Seating_Planner;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using System.Diagnostics;
using System.Windows.Automation;
using FlaUIElement = FlaUI.Core.AutomationElements;

namespace Tests
{
    [TestClass]
    public class TestPopup
    {
        private Window GetPopupWindow(FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation)
        {
            FlaUIElement.Window[] windows = app.GetAllTopLevelWindows(automation);
            return windows.Where(window => window.Name == "Hj�lp").FirstOrDefault();
        }

        [TestMethod]
        public void TestOpeningPopup()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, Window window, ConditionFactory cf) = Utils.SetUpTest();

            FlaUIElement.Window[] windows = app.GetAllTopLevelWindows(automation);

            // Open the help popup
            FlaUIElement.AutomationElement helpButton = window.FindFirstDescendant(cf.ByAutomationId("FileHelpButton"));
            helpButton.Click();

            // Find the popup window
            FlaUIElement.AutomationElement popupWindow = GetPopupWindow(app, automation);
            Trace.Assert(popupWindow != null);
            Assert.IsNotNull(popupWindow);

            // Close it immediately
            FlaUIElement.AutomationElement closeButton = popupWindow.FindFirstDescendant(cf.ByAutomationId("CloseButton"));
            closeButton.Click();

            // Find the window again
            popupWindow = GetPopupWindow(app, automation);
            Assert.IsNull(popupWindow);

            // Open the popup again
            helpButton.Click();
            popupWindow = GetPopupWindow(app, automation);
            Assert.IsNotNull(popupWindow);


            app.Close();
        }

        [TestMethod]
        public void TestClosingMainWindow()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, Window window, ConditionFactory cf) = Utils.SetUpTest();

            // Open the help popup
            FlaUIElement.AutomationElement helpButton = window.FindFirstDescendant(cf.ByAutomationId("FileHelpButton"));
            helpButton.Click();

            // Find the popup window
            FlaUIElement.AutomationElement popupWindow = GetPopupWindow(app, automation);
            Assert.IsNotNull(popupWindow);

            // Close the main window
            window.Close();

            // The popup should be closed as well
            Assert.IsFalse(window.IsAvailable);


            app.Close();
        }

        [TestMethod]
        public void TestOpenFileExplorer()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, Window window, ConditionFactory cf) = Utils.SetUpTest();


            // Open the help popup
            FlaUIElement.AutomationElement helpButton = window.FindFirstDescendant(cf.ByAutomationId("FileHelpButton"));
            helpButton.Click();

            // Find the popup window
            FlaUIElement.AutomationElement popupWindow = GetPopupWindow(app, automation);
            Assert.IsNotNull(popupWindow);

            // Click the open button
            FlaUIElement.AutomationElement openButton = popupWindow.FindFirstDescendant(cf.ByAutomationId("OpenButton"));
            openButton.Click();

            // Find the file explorer window and make sure it's open
            FlaUIElement.AutomationElement explorer = automation.GetDesktop().FindFirstDescendant(cf.ByClassName("CabinetWClass"));
            Assert.IsNotNull(explorer);
            Assert.IsTrue(explorer.Name.Contains(UtilsHelpers.dataFolderName));


            app.Close();
        }

        [TestMethod]
        public void TestNoNameListFile()
        {

            // Save the file content to restore it after the test
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string applicationFolder = Path.Combine(documentsFolder, "Bordsplaceringsgeneratorn");
            string filePath = Path.Combine(applicationFolder, "klasslista.txt");

            if (!Directory.Exists(applicationFolder) || !File.Exists(filePath))
            {
                Assert.Fail("Test failed because the file with the student names does not exist.");
            }

            string fileContent = File.ReadAllText(filePath);

            // Remove the file
            Directory.Delete(applicationFolder, true);

            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, Window window, ConditionFactory cf) = UtilsHelpers.InitializeApplication();
            using FlaUI.UIA3.UIA3Automation automation = new();

            // Find the popup window
            Window GetPopup()
            {
                var windows = app.GetAllTopLevelWindows(automation);
                return windows.Where(window => window.Name == "Information").First()!;
            }

            // Check if the popup exists
            bool PopupExists()
            {
                return GetPopup() != null;
            }

            // Test that the application can handle the absence of the file
            Assert.IsTrue(PopupExists());
            Assert.Equals(GetPopup().FindFirstDescendant(cf.ByAutomationId("InformationText")).Name, "Klasslista hittades inte. En textfil har skapats i Documents/Bordsplaceringsgeneratorn/. Fyll i namnen i den p� seperata rader och starta sedan om programmet.");

            // Clean up the test environment and restore the file
            Directory.CreateDirectory(applicationFolder);
            File.WriteAllText(filePath, fileContent);
            Utils.TearDownTest(app);
        }
    }
}