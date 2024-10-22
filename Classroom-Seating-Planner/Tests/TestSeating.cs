﻿using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using System.Diagnostics;
using FlaUIElement = FlaUI.Core.AutomationElements;
using ListActions = Classroom_Seating_Planner.ListActions;

namespace Tests
{
    [TestClass]
    public class TestSeating
    {
        [TestMethod]
        public void TestSeatingIsCorrect()
        {
            // Find and run the application
            FlaUI.Core.Application app = FlaUI.Core.Application.Launch("..\\..\\..\\..\\Classroom-Seating-Planner\\bin\\Debug\\net8.0-windows\\Classroom-Seating-Planner.exe");
            using FlaUI.UIA3.UIA3Automation automation = new();

            // Find the main window for the purpose of finding elements
            FlaUIElement.Window window = app.GetMainWindow(automation);
            ConditionFactory cf = new(new UIA3PropertyLibrary());

            // Write your test after this comment!

            // Write your test before this comment!
            app.Close();
        }
    }
}