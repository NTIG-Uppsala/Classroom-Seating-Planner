# Automatic Testing System

_**Do not move your cursor or touch your keyboard while the tests are running!**_

## Running the Tests

1.  In Visual Studio, navigate to `Build`>`Configuration Manager...`.
1.  Make sure `Active solution configuration:` is set to `Debug`.
1.  Close the `Configuration Manager`.
1.  Navigate to `Test`>`Test Explorer`.
1.  Click `Run All Tests` or press `Ctrl+R, A` to build and run all the tests.

If you receive errors when trying to run the tests, try the following:

-   Make sure no windows from previous tests are still running.
-   Make sure no XAML-references such as automation ID or text content are incorrect.

## Editing the Tests

#### Editing the Default Testing Class List

1.  Navigate to [Utils.cs](../Classroom-Seating-Planner/Tests/Utils.cs).
1.  Navigate to the `SetUpTest`-method and change the List `testClassList`.

#### Creating a New Test

1.  Duplicate [TestTemplate.cs](../Classroom-Seating-Planner/Tests/TestTemplate.cs).
1.  Change the file name to something that is appropriate for the test you want to write.
1.  Change the method name to something that is appropriate for what you want to test.
1.  Follow the instructions in the comments of the `TestMethod`.

-   If you are creating a new method within a test file, and want it to be run as a test, make sure it is preceded by `[TestMethod]` and contains `Utils.SetUpTest` and `Utils.TearDownTest` like in the template.

---

[Back to README.](../README.md)
