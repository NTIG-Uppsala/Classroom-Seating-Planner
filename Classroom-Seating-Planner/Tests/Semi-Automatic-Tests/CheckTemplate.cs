using FlaUIElement = FlaUI.Core.AutomationElements;
using Tests;
using System.Diagnostics;

namespace A01_Semi_Automatic_Tests
{
    [TestClass]
    public class CheckTemplate
    {
        [TestMethod]
        public void TemplateMethod()
        {
            return; // Remove this line in the real tests
            // Set up/start the test
            (FlaUI.Core.Application app, FlaUI.UIA3.UIA3Automation automation, FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
                = Utils.SetUp();


            // Write your code after this comment!

            // Write your code before this comment!

            Stopwatch safetyTimeout = new();
            safetyTimeout.Start();

            try
            {
                // Wait until the user has either minimized the window or closed it
                while (true)
                {
                    // Fail the test if the user fails to verify the validity of the test within the time limit
                    if (safetyTimeout.ElapsedMilliseconds > 2 * 60000)
                    {
                        throw new Exception("Test timed out");
                    }

                    // If the window is minimized it is moved to (-32000, -32000), triggering the if statement
                    // If the window is closed, the conditional can not be evaluated and throws an exception (which is different from the exception thrown inside the if statement)
                    if (window.BoundingRectangle.X < -30000 || window.BoundingRectangle.Y < -30000)
                    {
                        // Throw an exception and send a specific message if window is minimized
                        throw new Exception("Window was minimized");
                    }

                    Thread.Sleep(250);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Equals("Test timed out"))
                {
                    Assert.Fail("The user failed to verify the validity of the test within the time limit");
                }

                // If the window was minimzed, pass the test
                else if (e.Message.Equals("Window was minimized"))
                {
                    Trace.WriteLine("The window was minimized by user, signaling that the test passed");
                }

                // If the window was closed, fail the test
                else
                {
                    Assert.Fail("Window was closed by user, signaling that the test failed");
                }
            }


            Utils.TearDown(app);
        }
    }
}