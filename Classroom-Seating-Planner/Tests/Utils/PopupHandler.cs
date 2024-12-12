using FlaUI.Core.AutomationElements;
using System.Dynamic;
using FlaUIElement = FlaUI.Core.AutomationElements;

namespace Tests
{
    internal class PopupHandler
    {
        // Names of the popup window titles, used to find the popup windows
        public static readonly string helpPopupName = "Hjälp";
        public static readonly string missingFilePopupName = "Information";
        public static readonly string badFilePopupName = "Varning";

        // Names of the help button in the main window and the buttons in the popup windows
        public static readonly string helpButtonText = "Hjälp";
        public static readonly string okayButtonText = "Okej";
        public static readonly string openFolderButtonText = "Öppna mapp";

        public static List<FlaUIElement.Window> FindPopupWindows(FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, string windowTitle)
        {
            FlaUIElement.Window[] windows = app.GetAllTopLevelWindows(automation);
            return windows.Where(window => window.Name.Equals(windowTitle)).ToList();
        }

        public static void AnyPopupWindowContainsText(FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUI.Core.Conditions.ConditionFactory cf, string windowTitle, string expectedText)
        {
            List<FlaUIElement.Window> popupWindows = Utils.PopupHandler.FindPopupWindows(app, automation, windowTitle);

            // Test fails if no popup windows are found
            if (popupWindows == null || popupWindows.Count.Equals(0))
            {
                Assert.Fail("No popup windows were found");
            }

            // Check if any popup window contains the expected text
            bool anyPopupWindowContainsExpectedText = popupWindows.Any((FlaUIElement.Window window) =>
            {
                FlaUIElement.AutomationElement textBody = window.FindFirstDescendant(cf.ByAutomationId("TextBody"));
                return textBody != null && textBody.Name.Contains(expectedText);
            });

            Assert.IsTrue(anyPopupWindowContainsExpectedText, "No popup window contains {0}", expectedText);
        }

        public static void NoPopupWindowContainsText(FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUI.Core.Conditions.ConditionFactory cf, string windowTitle, string expectedText)
        {
            List<FlaUIElement.Window> popupWindows = Utils.PopupHandler.FindPopupWindows(app, automation, windowTitle);

            // Test is passed if no popup windows are found
            if (popupWindows.Count.Equals(0))
            {
                return;
            }

            // Check if any popup window contains the expected text
            bool anyPopupWindowContainsExpectedText = popupWindows.Any((FlaUIElement.Window window) =>
            {
                FlaUIElement.AutomationElement textBody = window.FindFirstDescendant(cf.ByAutomationId("TextBody"));
                return textBody != null && textBody.Name.Contains(expectedText);
            });

            Assert.IsFalse(anyPopupWindowContainsExpectedText, "There is a popup window that contains {0}", expectedText);
        }
    }
}
