using FlaUIElement = FlaUI.Core.AutomationElements;
using Tests;

namespace A02_Automatic_Tests
{
    [TestClass]
    public class TestConstraints
    {
        [TestMethod]
        public void ExtremeConstraintTest()
        {
            List<string> testingClassroomLayout = [
                "   TTTT",
                "",
                "     B",
                "   B",
                "",
                "BBBB BBBB B",
                "",
                "BBB BBB BBB"
                ];

            List<string> testingClassList = [
                "Emma Andersson",
                "William Eriksson",
                "Olivia Karlsson: långtfrån Lucas Johansson (5)",
                "Lucas Johansson",
                "Alice Svensson: långtfrån Ella Larsson (7)",
                "Elias Nilsson",
                "Ella Larsson: nära tavlan (300)",
                "Noah Persson",
                "Alva Olsson: bredvid Noah Persson (9)",
                "Liam Lindberg",
                "Ebba Bergström: nära tavlan (700)",
                "Oscar Holmgren: bredvid Emma Andersson (4)",
                "Nora Lundqvist: intebredvid Liam Lindberg (3) / nära Ebba Bergström (8)",
                "Alexander Håkansson",
                "Wilma Strömberg",
                "Hugo Sandberg"
                ];

            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp(testingClassList: testingClassList, testingClassroomLayout: testingClassroomLayout); // SetUp has optional arguments that may be useful for certain tests

            // Write your test after this comment!

            // Write your test before this comment!
            Utils.TearDown(app);
        }

        [TestMethod]
        public void ALotOfConstraintsStressTest() // AllConstrainedStudentsTest?
        {
            
        }
    }
}