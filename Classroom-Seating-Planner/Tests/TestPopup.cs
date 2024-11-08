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
        private FlaUIElement.Window GetHelpWindow(FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation)
        {
            FlaUIElement.Window[] windows = app.GetAllTopLevelWindows(automation);
            return windows.Where(window => window.Name == "Hjälp").FirstOrDefault();
        }

        [TestMethod]
        public void TestOpeningPopup()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf) = Utils.SetUpTest();


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
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf) = Utils.SetUpTest();


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

        private static List<FlaUIElement.AutomationElement> GetAllExplorerInstances(FlaUI.UIA3.UIA3Automation automation, ConditionFactory cf)
        {
            return automation.GetDesktop().FindAllChildren(cf.ByClassName("CabinetWClass")).ToList();
        }

        [TestMethod]
        public void TestOpenFileExplorer()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf) = Utils.SetUpTest();


            // Open the help popup
            FlaUIElement.AutomationElement helpButton = window.FindFirstDescendant(cf.ByAutomationId("FileHelpButton"));
            helpButton.Click();

            // Find the popup window
            FlaUIElement.AutomationElement popupWindow = GetHelpWindow(app, automation);
            Assert.IsNotNull(popupWindow);

            // Save the currently open file explorer windows
            List<FlaUIElement.AutomationElement> explorerInstances = GetAllExplorerInstances(automation, cf);

            // Click the open button
            FlaUIElement.AutomationElement openButton = popupWindow.FindFirstDescendant(cf.ByAutomationId("OpenButton"));
            openButton.Click();

            // Wait until a new explorer window is opened
            Stopwatch timeout = new();
            timeout.Start();
            while (explorerInstances.SequenceEqual(GetAllExplorerInstances(automation, cf)) && timeout.ElapsedMilliseconds < 10000)
            {
                // Wait...
                // Wait...
                // It's gonna be...
                // Wait for it...
                // LEGENDARY!
            }

            // Get the new explorer window
            FlaUIElement.AutomationElement? explorer = GetAllExplorerInstances(automation, cf).Except(explorerInstances).FirstOrDefault();

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

        private FlaUIElement.Window GetNoFileWindow(FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation)
        {
            FlaUIElement.Window[] windows = app.GetAllTopLevelWindows(automation);
            return windows.Where(window => window.Name == "Information").FirstOrDefault();
        }

        [TestMethod]
        public void TestMissingDirectory()
        {
            // Create a backup folder
            string backupFolderName = $"{UtilsHelpers.dataFolderName}.bak";
            string documentsFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string backupFolderPath = System.IO.Path.Combine(documentsFolderPath, backupFolderName);

            // Ensure that there is no preexisting backup folder and create a new one
            if (System.IO.Directory.Exists(backupFolderPath))
            {
                System.IO.Directory.Delete(backupFolderPath, true);
            }
            System.IO.Directory.CreateDirectory(backupFolderPath);

            // Move all files from the data folder to the backup folder
            foreach (string filePath in System.IO.Directory.GetFiles(UtilsHelpers.dataFolderPath))
            {
                System.IO.File.Move(filePath, System.IO.Path.Combine(backupFolderPath, System.IO.Path.GetFileName(filePath)));
            }

            // Delete the data folder
            System.IO.Directory.Delete(UtilsHelpers.dataFolderPath, true);

            // Get FLaUI boilerplate
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf) = UtilsHelpers.InitializeApplication();

            // Check that the correct popup is shown when the directory is missing
            FlaUIElement.Window popupWindow = GetNoFileWindow(app, automation);
            Assert.IsNotNull(popupWindow);
            Assert.IsTrue(popupWindow.FindFirstDescendant(cf.ByAutomationId("TextBody")).Name.Contains("Klasslista hittades inte"));

            // Delete the data folder after the tests have created them
            System.IO.Directory.Delete(UtilsHelpers.dataFolderPath, true);

            // Ensure that the data folder exists
            System.IO.Directory.CreateDirectory(UtilsHelpers.dataFolderPath);

            // Move all files back from the backup folder to the data folder
            foreach (string filePath in System.IO.Directory.GetFiles(backupFolderPath))
            {
                System.IO.File.Move(filePath, System.IO.Path.Combine(UtilsHelpers.dataFolderPath, System.IO.Path.GetFileName(filePath)));
            }

            // Delete the backup folder
            System.IO.Directory.Delete(backupFolderPath, true);


            Utils.TearDownTest(app);
        }

        [TestMethod]
        public void TestMissingClassListFile()
        {
            // Restore backup data if backup file already exists
            if (System.IO.File.Exists($"{UtilsHelpers.classListFilePath}.bak"))
            {
                UtilsHelpers.RestoreBackupData(UtilsHelpers.classListFilePath);
            }

            // Backup data and delete original file
            System.IO.File.Copy(UtilsHelpers.classListFilePath, UtilsHelpers.classListBackupFilePath);
            System.IO.File.Delete(UtilsHelpers.classListFilePath);

            // Get FLaUI boilerplate
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf) = UtilsHelpers.InitializeApplication();

            // Assert that the correct popup is shown when the file is missing
            FlaUIElement.Window popup = GetNoFileWindow(app, automation);
            Assert.IsNotNull(popup);
            Assert.IsTrue(popup.FindFirstDescendant(cf.ByAutomationId("TextBody")).Name.Contains("Klasslista hittades inte"));


            Utils.TearDownTest(app);
        }

        private FlaUIElement.Window GetBadFileWindow(FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation)
        {
            FlaUIElement.Window[] windows = app.GetAllTopLevelWindows(automation);
            return windows.Where(window => window.Name == "Varning").FirstOrDefault();
        }


        // Test that the application gives a popup warning when loading an empty list
        [TestMethod]
        public void TestEmptyListFile()
        {
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf) = Utils.SetUpTest([]);

            // Assert that the correct popup is shown when the empty list is loaded
            FlaUIElement.Window popup = GetBadFileWindow(app, automation);
            Assert.IsNotNull(popup);
            Assert.IsTrue(popup.FindFirstDescendant(cf.ByAutomationId("TextBody")).Name.Contains("Klasslistan är tom"));


            Utils.TearDownTest(app);
        }

        // Test that the application gives a popup warning when loading a default list
        [TestMethod]
        public void TestDefaultList()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf) = Utils.SetUpTest(UtilsHelpers.defaultClassList);


            // Assert that the correct popup is shown when the default list is loaded
            FlaUIElement.Window popup = GetBadFileWindow(app, automation);
            Trace.WriteLine(popup);
            Assert.IsNotNull(popup);
            Assert.IsTrue(popup.FindFirstDescendant(cf.ByAutomationId("TextBody")).Name.Contains("klasslistan inte har uppdaterats"));


            Utils.TearDownTest(app);
        }
    }
}