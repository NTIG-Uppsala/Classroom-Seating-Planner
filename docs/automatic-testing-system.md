# Automatic Testing System

_**Do not move your cursor or touch your keyboard while the tests are running!**_

## Running the Tests

-   Before running any tests, run the program once to create the necessary files. 
-   In Visual Studio, navigate to `Build`>`Configuration Manager...`.
-   Make sure `Active solution configuration:` is set to `Debug`.
-   Close the `Configuration Manager`.
-   Navigate to `Test`>`Test Explorer`.
-   Click `Run All Tests` or press `Ctrl+R, A` to build and run all the tests.

If you recieve errors when trying to run the tests, try the following:

-   Make sure no windows from previous tests are still running.

## Editing the Tests

#### Editing the Default Student Names List

-   Navigate to [Utils.cs](../Classroom-Seating-Planner/Tests/Utils.cs).
-   Navigate to the `SetUpTest`-method and change the List `testNamesList`.

#### Creating a New Test

-   Duplicate [TestTemplate.cs](../Classroom-Seating-Planner/Tests/TestTemplate.cs).
-   Change the file name to something that is appropriate for the test you want to write.
-   Change the method name to something that is appropriate for what you want to test.
-   Follow the instructions in the comments of the `TestMethod`.
-   If you are creating a new method within a test file, and want it to be run as a test, make sure it is preceded by `[TestMethod]`.

---

[Back to README.](../README.md)
