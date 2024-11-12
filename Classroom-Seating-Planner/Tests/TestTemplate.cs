using FlaUIElement = FlaUI.Core.AutomationElements;

namespace Tests
{
    [TestClass]
    public class TestTemplate
    {
        [TestMethod]
        public void TemplateMethod()
        {
            return; // Remove this line in the real tests
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp();

            // Write your test after this comment!

            // Write your test before this comment!
            Utils.TearDown(app);
        }
    }
}