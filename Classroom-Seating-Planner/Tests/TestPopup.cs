using Classroom_Seating_Planner;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using System.Collections;
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

            // Close the popup immediately
            FlaUIElement.AutomationElement closeButton = popupWindow.FindFirstDescendant(cf.ByAutomationId("CloseButton"));
            closeButton.Click();

            // Check that the window has closed
            popupWindow = GetHelpWindow(app, automation);
            Assert.IsNull(popupWindow);

            // Open the popup again and check if it opened the second time
            helpButton.Click();
            popupWindow = GetHelpWindow(app, automation);
            Assert.IsNotNull(popupWindow);


            Utils.TearDownTest(app);
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


            Utils.TearDownTest(app);
        }

        private static List<Window> GetAllExplorerInstances() // Add app and automation
        {
              // TODO: implement
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

            // Save the currently open file explorer windows
            List<string> explorerInstances = GetAllExplorerInstances();

            // Click the open button
            FlaUIElement.AutomationElement openButton = popupWindow.FindFirstDescendant(cf.ByAutomationId("OpenButton"));
            openButton.Click();

            // Until the file explorer window opens or the timeout is reached, check for the new explorer window
            Window? explorer = null;
            Stopwatch timeout = new();
            timeout.Start();
            while (explorer == null && timeout.ElapsedMilliseconds < 10000)
            {
                explorer = GetAllExplorerInstances().Except(explorerInstances).FirstOrDefault();
            }
            if (explorer == null)
            {
                Assert.Fail("The file explorer window did not open within the timeout.");
            }

            // Check that the file explorer window has the expected name
            Assert.IsTrue(explorer.Name.Contains(UtilsHelpers.dataFolderName));

            // Close the file explorer window
            FlaUIElement.AutomationElement closeButton = explorer.FindFirstDescendant(cf.ByAutomationId("Close"));
            closeButton.Click();


            Utils.TearDownTest(app);
        }

        private Window GetNoFileWindow(FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation)
        {
            FlaUIElement.Window[] windows = app.GetAllTopLevelWindows(automation);
            return windows.Where(window => window.Name == "Information").FirstOrDefault();
        }

        [TestMethod]
        public void TestNoDirectory()
        {
            // Save the file content to restore it after the test
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string backupFolder = Path.Combine(documentsFolder, "BordsplaceringsgeneratornBackup");
            string backupFilePath = Path.Combine(backupFolder, "klasslista.no-directory.txt.bak");

            if (!Directory.Exists(UtilsHelpers.dataFolderPath) || !File.Exists(UtilsHelpers.classListFilePath))
            {
                Assert.Fail("Test failed because the file with the class list does not exist.");
            }

            string fileContent = File.ReadAllText(UtilsHelpers.classListFilePath); // TODO : Avoid string for file content
            Directory.CreateDirectory(backupFolder);
            File.WriteAllText(backupFilePath, fileContent);
            // Remove the file
            Directory.Delete(UtilsHelpers.dataFolderPath, true);

            // Get FLaUI boilerplate
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, Window window, ConditionFactory cf) = UtilsHelpers.InitializeApplication();

            // Check that the correct popup is shown when the directory is missing
            Window popup = GetNoFileWindow(app, automation);
            Assert.IsNotNull(popup);
            Assert.IsTrue(popup.FindFirstDescendant(cf.ByAutomationId("TextBody")).Name.Contains("Klasslista hittades inte"));

            // Clean up the test environment and restore the file
            Directory.CreateDirectory(UtilsHelpers.dataFolderPath);
            File.WriteAllText(UtilsHelpers.classListFilePath, fileContent);
            Directory.Delete(backupFolder, true);


            Utils.TearDownTest(app);
        }

        [TestMethod]
        public void TestNoClassListFile()
        {
            // Restore backup data if backup file already exists
            if (System.IO.File.Exists($"{UtilsHelpers.classListFilePath}.bak"))
            {
                UtilsHelpers.RestoreBackupData(UtilsHelpers.classListFilePath);
            }

            // Backup data and delete original file
            File.Copy(UtilsHelpers.classListFilePath, UtilsHelpers.classListBackupFilePath);
            File.Delete(UtilsHelpers.classListFilePath);

            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, Window window, ConditionFactory cf) = UtilsHelpers.InitializeApplication();

            // Assert that the correct popup is shown when the file is missing
            Window popup = GetNoFileWindow(app, automation);
            Assert.IsNotNull(popup);
            Assert.IsTrue(popup.FindFirstDescendant(cf.ByAutomationId("TextBody")).Name.Contains("Klasslista hittades inte"));


            Utils.TearDownTest(app);
        }

        private Window GetBadFileWindow(FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation)
        {
            FlaUIElement.Window[] windows = app.GetAllTopLevelWindows(automation);
            return windows.Where(window => window.Name == "Varning").FirstOrDefault();
        }


        // Test that the application gives a popup warning when loading an empty list
        [TestMethod]
        public void TestEmptyListFile()
        {
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, Window window, ConditionFactory cf) = Utils.SetUpTest([]);


            Window popup = GetBadFileWindow(app, automation);
            Assert.IsNotNull(popup);
            Assert.IsTrue(popup.FindFirstDescendant(cf.ByAutomationId("TextBody")).Name.Contains("Klasslistan är tom"));


            Utils.TearDownTest(app);
        }

        // Test that the application gives a popup warning when loading a default list
        [TestMethod]
        public void TestDefaultList()
        {
            List<string> defaultNameList =
            [
                "Förnamn Efternamn",
                "Förnamn Efternamn",
                "Förnamn Efternamn",
            ];

            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, Window window, ConditionFactory cf) = Utils.SetUpTest(defaultNameList);

            Window popup = GetBadFileWindow(app, automation);
            Trace.WriteLine(popup);
            Assert.IsNotNull(popup);
            Assert.IsTrue(popup.FindFirstDescendant(cf.ByAutomationId("TextBody")).Name.Contains("klasslistan inte har uppdaterats"));


            Utils.TearDownTest(app);
        }
    }
}