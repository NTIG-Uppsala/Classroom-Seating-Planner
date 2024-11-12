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
        private static readonly string helpPopupName = "Hjälp";
        private static readonly string missingFilePopupName = "Information";
        private static readonly string badFilePopupName = "Varning";

        private static Window? FindPopupWindow(string windowTitle, FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation)
        {
            FlaUIElement.Window[] windows = app.GetAllTopLevelWindows(automation);
            return windows.Where(window => window.Name == windowTitle).FirstOrDefault();
        }

        [TestMethod]
        public void OpenAndCloseHelpWindowTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp();


            // Open the help popup
            FlaUIElement.AutomationElement helpButton = window.FindFirstDescendant(cf.ByText("Hjälp"));
            helpButton.Click();

            // Find the popup window
            FlaUIElement.AutomationElement? popupWindow = FindPopupWindow(helpPopupName, app, automation);
            Assert.IsNotNull(popupWindow);

            // Close the popup immediately
            FlaUIElement.AutomationElement closeButton = popupWindow.FindFirstDescendant(cf.ByText("Okej"));
            closeButton.Click();

            // Check that the window has closed
            popupWindow = FindPopupWindow(helpPopupName, app, automation);
            Assert.IsNull(popupWindow);

            // Open the popup again and check if it opened the second time
            helpButton.Click();
            popupWindow = FindPopupWindow(helpPopupName, app, automation);
            Assert.IsNotNull(popupWindow);


            Utils.TearDown(app);
        }

        [TestMethod]
        public void CloseMainAndChildWindowsTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp();


            // Open the help popup
            FlaUIElement.AutomationElement helpButton = window.FindFirstDescendant(cf.ByText("Hjälp"));
            helpButton.Click();

            // Find the popup window
            FlaUIElement.AutomationElement? popupWindow = FindPopupWindow(helpPopupName, app, automation);
            Assert.IsNotNull(popupWindow);

            // Close the main window
            window.Close();

            // The popup should be closed as well
            popupWindow = FindPopupWindow(helpPopupName, app, automation);
            Assert.IsNull(popupWindow);


            Utils.TearDown(app);
        }

        private static List<FlaUIElement.AutomationElement> GetAllExplorerInstances(FlaUI.UIA3.UIA3Automation automation, ConditionFactory cf)
        {
            return automation.GetDesktop().FindAllChildren(cf.ByClassName("CabinetWClass")).ToList();
        }

        [TestMethod]
        public void OpenClassListDirectoryTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp();


            // Open the help popup
            FlaUIElement.AutomationElement helpButton = window.FindFirstDescendant(cf.ByText("Hjälp"));
            helpButton.Click();

            // Find the popup window
            FlaUIElement.AutomationElement? popupWindow = FindPopupWindow(helpPopupName, app, automation);
            Assert.IsNotNull(popupWindow);

            // Save the currently open file explorer windows
            List<FlaUIElement.AutomationElement> explorerInstances = GetAllExplorerInstances(automation, cf);

            // Click the open button
            FlaUIElement.AutomationElement openButton = popupWindow.FindFirstDescendant(cf.ByText("Öppna mapp"));
            openButton.Click();

            // Wait until a new explorer window is opened
            Stopwatch timeout = new();
            timeout.Start();
            while (explorerInstances.SequenceEqual(GetAllExplorerInstances(automation, cf)) && timeout.ElapsedMilliseconds < 10000)
            {
                // Wait...
                // Wait...
                // It's gonna be legen -
                // Wait for it...
                // - DARY!
            }

            // Get the new explorer window
            FlaUIElement.AutomationElement? explorer = GetAllExplorerInstances(automation, cf).Except(explorerInstances).FirstOrDefault();

            if (explorer == null)
            {
                Assert.Fail("The file explorer window did not open within the timeout.");
            }

            // Check that the file explorer is opened in the correct directory
            Assert.IsTrue(explorer.Name.Contains(Utils.dataFolderName));

            // Close the file explorer window
            FlaUIElement.AutomationElement closeButton = explorer.FindFirstDescendant(cf.ByAutomationId("Close"));
            closeButton.Click();


            Utils.TearDown(app);
        }

        [TestMethod]
        public void MissingDirectoryTest()
        {
            // Create a backup folder
            string backupFolderName = $"{Utils.dataFolderName}.bak";
            string documentsFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string backupFolderPath = System.IO.Path.Combine(documentsFolderPath, backupFolderName);

            // Ensure that there is no preexisting backup folder and create a new one
            if (System.IO.Directory.Exists(backupFolderPath))
            {
                System.IO.Directory.Delete(backupFolderPath, true);
            }
            System.IO.Directory.CreateDirectory(backupFolderPath);

            // Move all files from the data folder to the backup folder
            foreach (string filePath in System.IO.Directory.GetFiles(Utils.FileManager.dataFolderPath))
            {
                System.IO.File.Move(filePath, System.IO.Path.Combine(backupFolderPath, System.IO.Path.GetFileName(filePath)));
            }

            // Delete the data folder
            System.IO.Directory.Delete(Utils.FileManager.dataFolderPath, true);

            // Get FLaUI boilerplate
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.InitializeFlaUIApp();

            // Check that the correct popup is shown when the directory is missing
            FlaUIElement.Window? popupWindow = FindPopupWindow(missingFilePopupName, app, automation);
            Assert.IsNotNull(popupWindow);
            Assert.IsTrue(popupWindow.FindFirstDescendant(cf.ByAutomationId("TextBody")).Name.Contains("Klasslista hittades inte"));

            // Delete the data folder after the tests have created them
            System.IO.Directory.Delete(Utils.FileManager.dataFolderPath, true);

            // Ensure that the data folder exists
            System.IO.Directory.CreateDirectory(Utils.FileManager.dataFolderPath);

            // Move all files back from the backup folder to the data folder
            foreach (string filePath in System.IO.Directory.GetFiles(backupFolderPath))
            {
                System.IO.File.Move(filePath, System.IO.Path.Combine(Utils.FileManager.dataFolderPath, System.IO.Path.GetFileName(filePath)));
            }

            // Delete the backup folder
            System.IO.Directory.Delete(backupFolderPath, true);


            Utils.TearDown(app);
        }

        [TestMethod]
        public void MissingClassListFileTest()
        {
            // Restore backup data if backup file already exists
            if (System.IO.File.Exists($"{Utils.FileManager.classListFilePath}.bak"))
            {
                Utils.FileManager.RestoreBackupData(Utils.FileManager.classListFilePath);
            }

            // Backup data and delete original file
            System.IO.File.Copy(Utils.FileManager.classListFilePath, Utils.FileManager.classListBackupFilePath);
            System.IO.File.Delete(Utils.FileManager.classListFilePath);

            // Get FLaUI boilerplate
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.InitializeFlaUIApp();

            // Assert that the correct popup is shown when the file is missing
            FlaUIElement.Window? popupWindow = FindPopupWindow(missingFilePopupName, app, automation);
            Assert.IsNotNull(popupWindow);
            Assert.IsTrue(popupWindow.FindFirstDescendant(cf.ByAutomationId("TextBody")).Name.Contains("Klasslista hittades inte"));


            Utils.TearDown(app);
        }

        // Test that the application gives a popup warning when loading an empty list
        [TestMethod]
        public void EmptyClassListFileTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp([]);


            // Assert that the correct popup is shown when the empty list is loaded
            FlaUIElement.Window? popupWindow = FindPopupWindow(badFilePopupName, app, automation);
            Assert.IsNotNull(popupWindow);
            Assert.IsTrue(popupWindow.FindFirstDescendant(cf.ByAutomationId("TextBody")).Name.Contains("Klasslistan är tom"));


            Utils.TearDown(app);
        }

        // Test that the application gives a popup warning when loading a default list
        [TestMethod]
        public void DefaultClassListFileTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(Utils.defaultClassList);

            // Assert that the correct popup is shown when the default list is loaded
            FlaUIElement.Window? popupWindow = FindPopupWindow(badFilePopupName, app, automation);
            Assert.IsNotNull(popupWindow);
            Assert.IsTrue(popupWindow.FindFirstDescendant(cf.ByAutomationId("TextBody")).Name.Contains("klasslistan inte har uppdaterats"));


            Utils.TearDown(app);
        }
    }
}