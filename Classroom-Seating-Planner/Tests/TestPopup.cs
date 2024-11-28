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
        public void OpenAndCloseHelpWindowTest()
        {
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp();


            // Open the help popup
            FlaUIElement.AutomationElement helpButton = window.FindFirstDescendant(cf.ByText(Utils.PopupHandler.helpButtonText));
            helpButton.Click();

            // Find the popup window
            FlaUIElement.AutomationElement? popupWindow = Utils.PopupHandler.FindPopupWindow(Utils.PopupHandler.helpPopupName, app, automation);
            Assert.IsNotNull(popupWindow);

            // Close the popup immediately
            FlaUIElement.AutomationElement closeButton = popupWindow.FindFirstDescendant(cf.ByText(Utils.PopupHandler.okayButtonText));
            closeButton.Click();

            // Check that the window has closed
            popupWindow = Utils.PopupHandler.FindPopupWindow(Utils.PopupHandler.helpPopupName, app, automation);
            Assert.IsNull(popupWindow);

            // Open the popup again and check if it opened the second time
            helpButton.Click();
            popupWindow = Utils.PopupHandler.FindPopupWindow(Utils.PopupHandler.helpPopupName, app, automation);
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
            FlaUIElement.AutomationElement helpButton = window.FindFirstDescendant(cf.ByText(Utils.PopupHandler.helpButtonText));
            helpButton.Click();

            // Find the popup window
            FlaUIElement.AutomationElement? popupWindow = Utils.PopupHandler.FindPopupWindow(Utils.PopupHandler.helpPopupName, app, automation);
            Assert.IsNotNull(popupWindow);

            // Close the main window
            window.Close();

            // The popup should be closed as well
            popupWindow = Utils.PopupHandler.FindPopupWindow(Utils.PopupHandler.helpPopupName, app, automation);
            Assert.IsNull(popupWindow);


            Utils.TearDown(app);
        }

    }
}