using FlaUI.Core.Conditions;
using System.Diagnostics;
using FlaUIElement = FlaUI.Core.AutomationElements;
using Tests;

namespace A02_Automatic_Tests
{
    [TestClass]
    public class TestDataDirectory
    {
        [TestMethod]
        public void OpenDataDirectoryTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp();


            static List<FlaUIElement.AutomationElement> getAllExplorerInstances(FlaUI.UIA3.UIA3Automation automation, ConditionFactory cf)
            {
                // The class name for the file explorers window is "CabinetWClass". It's weird but that just how it is.
                return automation.GetDesktop().FindAllChildren(cf.ByClassName("CabinetWClass")).ToList();
            }

            // Open the help popup
            FlaUIElement.AutomationElement helpButton = window.FindFirstDescendant(cf.ByText(Utils.PopupHandler.helpButtonText));
            helpButton.Click();

            // Get all matching popup windows (Should only be one)
            List<FlaUIElement.Window> popupWindows = Utils.PopupHandler.FindPopupWindows(app, automation, Utils.PopupHandler.helpPopupName);
            Assert.IsFalse(popupWindows.Count.Equals(0), "No popup window was found");
            Assert.IsTrue(popupWindows.Count.Equals(1), "There is more than one popup window");


            // Get the popup window
            FlaUIElement.Window popupWindow = popupWindows[0];
            Assert.IsNotNull(popupWindow);

            // Save the currently open file explorer windows
            List<FlaUIElement.AutomationElement> explorerInstances = getAllExplorerInstances(automation, cf);

            // Click the open button
            FlaUIElement.AutomationElement openButton = popupWindow.FindFirstDescendant(cf.ByText(Utils.PopupHandler.openFolderButtonText));
            openButton.Click();

            // Wait until a new explorer window is opened
            Stopwatch timeout = new();
            timeout.Start();
            while (explorerInstances.SequenceEqual(getAllExplorerInstances(automation, cf)) && timeout.ElapsedMilliseconds < 10000)
            {
                // Wait...
                // Wait...
                // It's gonna be legen -
                // Wait for it...
                // - DARY!
            }

            // Get the new explorer window
            FlaUIElement.AutomationElement? explorer = getAllExplorerInstances(automation, cf).Except(explorerInstances).FirstOrDefault();

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
        public void MissingDataDirectoryTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(ignoreClassListFileBackup: true, ignoreClassroomLayoutFileBackup: true, ignoreTestingClassList: true, ignoreTestingClassroomLayout: true, createDataBackupFolder: true, deleteDataFolder: true);

            // Check that the correct popup is shown when the directory is missing
            Utils.PopupHandler.AnyPopupWindowContainsText(app, automation, cf, Utils.PopupHandler.missingFilePopupName, "Alla filer hittades inte");
            Utils.PopupHandler.NoPopupWindowContainsText(app, automation, cf, Utils.PopupHandler.badFilePopupName, "är tom");

            // Delete the data folder after the tests have created them
            System.IO.Directory.Delete(Utils.FileHandler.dataFolderPath, true);

            // Ensure that the data folder exists
            System.IO.Directory.CreateDirectory(Utils.FileHandler.dataFolderPath);

            // Move all files back from the backup folder to the data folder
            foreach (string filePath in System.IO.Directory.GetFiles(Utils.FileHandler.dataBackupFolderPath))
            {
                System.IO.File.Move(filePath, System.IO.Path.Combine(Utils.FileHandler.dataFolderPath, System.IO.Path.GetFileName(filePath)));
            }

            // Delete the backup folder
            System.IO.Directory.Delete(Utils.FileHandler.dataBackupFolderPath, true);


            Utils.TearDown(app);
        }
    }
}
