using FlaUIElement = FlaUI.Core.AutomationElements;
using Tests;

namespace A02_Automatic_Tests
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
                = Utils.SetUp(); // SetUp has optional arguments that may be useful for certain tests

            // Write your test after this comment!

            // Write your test before this comment!
            Utils.TearDown(app);
        }
    }
}