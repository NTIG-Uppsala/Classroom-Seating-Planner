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
            while (explorer == null && wait.ElapsedMilliseconds < 5000)
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
        public void TestNoDirectory()
        {
            // Save the file content to restore it after the test
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string dataFolder = UtilsHelpers.dataFolderPath;
            string backupFolder = Path.Combine(documentsFolder, "BordsplaceringsgeneratornBackup");
            string filePath = UtilsHelpers.studentNamesListFilePath;
            string backupFilePath = Path.Combine(backupFolder, "klasslista.no-directory.txt.bak");

            if (!Directory.Exists(dataFolder) || !File.Exists(filePath))
            {
                Assert.Fail("Test failed because the file with the student names does not exist.");
            }

            string fileContent = File.ReadAllText(filePath);
            Directory.CreateDirectory(backupFolder);
            File.WriteAllText(backupFilePath, fileContent);
            // Remove the file
            Directory.Delete(dataFolder, true);

            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, Window window, ConditionFactory cf) = UtilsHelpers.InitializeApplication();

            // Assert that the correct popup is shown when the directory is missing
            Window popup = GetNoFileWindow(app, automation);
            Assert.IsNotNull(popup);
            Assert.IsTrue(popup.FindFirstDescendant(cf.ByAutomationId("TextBody")).Name.Contains("Klasslista hittades inte"));

            // Clean up the test environment and restore the file
            Directory.CreateDirectory(dataFolder);
            File.WriteAllText(filePath, fileContent);
            Directory.Delete(backupFolder, true);
            app.Close();
        }

        [TestMethod]
        public void TestNoNameFile()
        {
            string dataFolder = UtilsHelpers.dataFolderPath;
            string filePath = UtilsHelpers.studentNamesListFilePath;
            string backupFilePath = Path.Combine(UtilsHelpers.dataFolderPath, "klasslista.no-file.txt.bak");

            if (!Directory.Exists(dataFolder) || !File.Exists(filePath))
            {
                Assert.Fail("Test failed because the file with the student names does not exist.");
            }

            string fileContent = File.ReadAllText(filePath);
            File.WriteAllText(backupFilePath, fileContent);
            File.Delete(filePath);

            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, Window window, ConditionFactory cf) = UtilsHelpers.InitializeApplication();

            // Assert that the correct popup is shown when the file is missing
            Window popup = GetNoFileWindow(app, automation);
            Assert.IsNotNull(popup);
            Assert.IsTrue(popup.FindFirstDescendant(cf.ByAutomationId("TextBody")).Name.Contains("Klasslista hittades inte"));

            // Clean up the test environment and restore the file
            File.WriteAllText(filePath, fileContent);
            File.Delete(backupFilePath);
            app.Close();

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