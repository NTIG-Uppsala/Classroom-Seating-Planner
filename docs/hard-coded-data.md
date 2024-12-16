# Hard Coded Data

_When changing the hard coded data, remember to change it in production <u>and</u> testing. Lest you want problems... *smirk*_

## List of names
- For production:
  - Default list of names if that file is missing:
    - `defaultClassList` in [FileHandler.cs](../Classroom-Seating-Planner/Classroom-Seating-Planner/src/FileHandler.cs).
- For testing:
  - Default list of names if that file is missing:
    - `defaultClassList` in [Utils.cs](../Classroom-Seating-Planner/Tests/Utils.cs).
  - List of names used in the tests:
    - `testingClassList` in [Utils.cs](../Classroom-Seating-Planner/Tests/Utils.cs).
  - In some tests there are lists of names that have specific properties. E.g. shorter lists, lists with certain constraints, etc. These lists are defined in the test methods themselves. Change these with great caution.

## Classroom layout
- For production:
  - Default classroom layout if that file is missing:
    - `defaultClassroomLayout` in [FileHandler.cs](../Classroom-Seating-Planner/Classroom-Seating-Planner/src/FileHandler.cs).
- For testing:
  - Default classroom layout if that file is missing: 
    - `defaultClassroomLayout` in [Utils.cs](../Classroom-Seating-Planner/Tests/Utils.cs).
  - Classroom layout used in the tests: 
    - `testingClassroomLayout` in [Utils.cs](../Classroom-Seating-Planner/Tests/Utils.cs).
  - In some tests there are classroom layouts that have specific properties. E.g. smaller classrooms, classrooms with tables at unusual places, etc. These layouts are defined in the test methods themselves. Change these with great caution.


---

[Back to README.](../README.md)