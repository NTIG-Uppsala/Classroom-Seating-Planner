# Testing System

_**Do not move your cursor or touch your keyboard while the automatic tests are running!**_

This project uses a testing framework called FlaUI which attaches itself to a running instance of the program to read and interact with the UI elements. While the tests are running, FlaUI uses the user keyboard and mouse to interact with the program. That is why you should not move your cursor or touch your keyboard while the tests are running. If you do, the tests might fail or show unexpected behavior.

## Running the Tests

1.  In Visual Studio, navigate to `Build`>`Configuration Manager...`.
1.  Make sure `Active solution configuration:` is set to `Debug`.
1.  Close the `Configuration Manager`.
1.  Navigate to `Test`>`Test Explorer`.
1.  Click `Run All Tests` or press `Ctrl+R, A` to build and run all the tests.

### Troubleshooting

-   Make sure no windows from previous tests are still running.
-   Run the failed tests one at a time.
-   Restart Visual Studio.
-   Restart your computer.
-   Make sure the XAML property values match the C# code that needs to reference them. For example, AutomationID and HelpText.

### Editing the Tests

-   To change any hard coded values, please refer to [hard-coded-data.md](./hard-coded-data.md). Otherwise, editing the tests just requires changing the code in the test files located in their respective folders in the `Tests` project.

-   There are tests which have their own hard coded testing class lists and classroom layouts. These have a very specific purpose for each individual test and should be changed with caution.

---

## Semi-Automatic Tests

-   Pass by minimizing the window.
-   Fail by closing the window.

The semi-automatic tests are run before the automatic tests. Like the other tests, these start an instance of the program but instead of testing something automatically, the results are determined by the tester. If the tester minimizes the window, the test will pass. If the tester closes the window, the test will fail.

#### Creating a New Semi-Automatic Test

1.  Duplicate [CheckTemplate.cs](../Classroom-Seating-Planner/Tests/Semi-Automatic-Tests/CheckTemplate.cs)
1.  Change the file name to something that is appropriate for the test you want to write.
1.  Change the method name to something that is appropriate for what you want to test.
1.  Follow the instructions in the comments of the `TemplateMethod`.

-   If you are creating a new method within a test file, and want it to be run as a test, make sure it is preceded by `[TestMethod]` and contains `Utils.SetUp()` and `Utils.TearDown()` like in the template.
    -   `Utils.SetUp()` has a lot of arguments that allows you to customize how you want the test to start and if you want to use any custom testing data.

---

## Automatic Testing System

Automatic tests run instances of the program and interacts with them automatically to test specific features. So do not worry about the windows that will be flashing in your view. 

#### Creating a New Automatic Test

1.  Duplicate [TestTemplate.cs](../Classroom-Seating-Planner/Tests/Automatic-Tests/TestTemplate.cs).
1.  Change the file name to something that is appropriate for the test you want to write.
1.  Change the method name to something that is appropriate for what you want to test.
1.  Follow the instructions in the comments of the `TemplateMethod`.

-   If you are creating a new method within a test file, and want it to be run as a test, make sure it is preceded by `[TestMethod]` and contains `Utils.SetUp()` and `Utils.TearDown()` like in the template.
    -   `Utils.SetUp()` has a lot of arguments that allows you to customize how you want the test to start and if you want to use any custom testing data.

---

[Back to README.](../README.md)
