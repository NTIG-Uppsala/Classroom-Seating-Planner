using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using System.Diagnostics;
using FlaUIElement = FlaUI.Core.AutomationElements;

namespace Tests
{
    [TestClass]
    public class TestPopup
    {

        [TestMethod]
        public void HelpWindowTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp();


            // Open the help popup
            FlaUIElement.AutomationElement helpButton = window.FindFirstDescendant(cf.ByText(Utils.PopupHandler.helpButtonText));
            helpButton.Click();

            // Find the popup window
            List<FlaUIElement.Window> popupWindows = Utils.PopupHandler.FindPopupWindows(app, automation, Utils.PopupHandler.helpPopupName);
            Utils.PopupHandler.PopupWindowContainsText(app, automation, cf, Utils.PopupHandler.helpPopupName, "Klasslistan och bordskartan ligger i");

            // Close the popup immediately
            FlaUIElement.AutomationElement closeButton = popupWindows[0].FindFirstDescendant(cf.ByText(Utils.PopupHandler.okayButtonText));
            closeButton.Click();

            // Check that the window has closed
            popupWindows = Utils.PopupHandler.FindPopupWindows(app, automation, Utils.PopupHandler.helpPopupName);
            Assert.IsTrue(popupWindows.Count.Equals(0), "The popup window did not close successfully");

            // Open the popup again and check if it opened the second time
            helpButton.Click();
            popupWindows = Utils.PopupHandler.FindPopupWindows(app, automation, Utils.PopupHandler.helpPopupName);
            Assert.IsFalse(popupWindows.Count.Equals(0), "No popup window was found");
            Assert.IsTrue(popupWindows.Count.Equals(1), "There is more than one popup window");


            Utils.TearDown(app);
        }

        [TestMethod]
        public void CloseMainAndChildWindowsTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp();


            // Open the help popup
            FlaUIElement.AutomationElement helpButton = window.FindFirstDescendant(cf.ByText(Utils.PopupHandler.helpButtonText));
            helpButton.Click();

            // Find the popup window
            List<FlaUIElement.Window> popupWindows = Utils.PopupHandler.FindPopupWindows(app, automation, Utils.PopupHandler.helpPopupName);
            Assert.IsFalse(popupWindows.Count.Equals(0), "No popup window was found");
            Assert.IsTrue(popupWindows.Count.Equals(1), "There is more than one popup window");

            // Close the main window
            window.Close();

            // The popup should be closed as well
            popupWindows = Utils.PopupHandler.FindPopupWindows(app, automation, Utils.PopupHandler.helpPopupName);
            Assert.IsTrue(popupWindows.Count.Equals(0), "A popup window was found");


            Utils.TearDown(app);
        }

    }
}