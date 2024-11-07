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
        private Window GetHelpWindow(FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation)
        {
            FlaUIElement.Window[] windows = app.GetAllTopLevelWindows(automation);
            return windows.Where(window => window.Name == "Hjälp").FirstOrDefault();
        }

        [TestMethod]
        public void TestOpeningPopup()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, Window window, ConditionFactory cf) = Utils.SetUpTest();

            // Open the help popup
            FlaUIElement.AutomationElement helpButton = window.FindFirstDescendant(cf.ByAutomationId("FileHelpButton"));
            helpButton.Click();

            // Find the popup window
            FlaUIElement.AutomationElement popupWindow = GetHelpWindow(app, automation);
            Trace.Assert(popupWindow != null);
            Assert.IsNotNull(popupWindow);

            // Close it immediately
            FlaUIElement.AutomationElement closeButton = popupWindow.FindFirstDescendant(cf.ByAutomationId("CloseButton"));
            closeButton.Click();

            // Find the window again
            popupWindow = GetHelpWindow(app, automation);
            Assert.IsNull(popupWindow);

            // Open the popup again
            helpButton.Click();
            popupWindow = GetHelpWindow(app, automation);
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
            FlaUIElement.AutomationElement popupWindow = GetHelpWindow(app, automation);
            Assert.IsNotNull(popupWindow);

            // Close the main window
            window.Close();

            // The popup should be closed as well
            popupWindow = GetHelpWindow(app, automation);
            Assert.IsNull(popupWindow);


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
            FlaUIElement.AutomationElement popupWindow = GetHelpWindow(app, automation);
            Assert.IsNotNull(popupWindow);

            // Click the open button
            FlaUIElement.AutomationElement openButton = popupWindow.FindFirstDescendant(cf.ByAutomationId("OpenButton"));
            openButton.Click();

            // Find the file explorer window and make sure it's open
            FlaUIElement.AutomationElement? explorer = null;
            Stopwatch wait = new();
            wait.Start();
            while (explorer == null && wait.ElapsedMilliseconds < 2000)
            {
                explorer = automation.GetDesktop().FindFirstDescendant(cf.ByClassName("CabinetWClass"));
            }
            wait.Stop();

            Assert.IsNotNull(explorer);
            Assert.IsTrue(explorer.Name.Contains(UtilsHelpers.dataFolderName));

            // Close the file explorer window
            FlaUIElement.AutomationElement closeButton = explorer.FindFirstDescendant(cf.ByAutomationId("Close"));
            closeButton.Click();
            app.Close();
        }

        private Window GetNoFileWindow(FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation)
        {
            FlaUIElement.Window[] windows = app.GetAllTopLevelWindows(automation);
            return windows.Where(window => window.Name == "Information").FirstOrDefault();
        }

        [TestMethod]
        public void TestNoNameListFile()
        {
            // Save the file content to restore it after the test
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string applicationFolder = UtilsHelpers.dataFolderPath;
            string backupFolder = Path.Combine(documentsFolder, "BordsplaceringsgeneratornBackup");
            string filePath = UtilsHelpers.studentNamesListFilePath;
            string backupFilePath = Path.Combine(backupFolder, "klasslista.txt");

            if (!Directory.Exists(applicationFolder) || !File.Exists(filePath))
            {
                Assert.Fail("Test failed because the file with the student names does not exist.");
            }

            string fileContent = File.ReadAllText(filePath);
            Directory.CreateDirectory(backupFolder);
            File.WriteAllText(backupFilePath, fileContent);
            // Remove the file
            Directory.Delete(applicationFolder, true);

            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, Window window, ConditionFactory cf) = UtilsHelpers.InitializeApplication();

            // Test that the application can handle the absence of the file
            Window popup = GetNoFileWindow(app, automation);
            Assert.IsNotNull(popup);
            Assert.IsTrue(popup.FindFirstDescendant(cf.ByAutomationId("TextBody")).Name.Contains("Klasslista hittades inte"));

            // Clean up the test environment and restore the file
            Directory.CreateDirectory(applicationFolder);
            File.WriteAllText(filePath, fileContent);
            Directory.Delete(backupFolder, true);
            app.Close();
        }
    }
}