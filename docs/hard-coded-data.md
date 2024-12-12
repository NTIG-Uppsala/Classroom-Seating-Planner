# Hard Coded Data

## File paths
File paths are constructed as such:
```cs
%dataFolderPath%\%dataFolderName%\%fileName%
```
Where the `dataFolderPath` is the user's documents folder, `dataFolderName` is the folder that is created in the documents folder, and `fileName` is the name of the file stored within the `dataFolderName` folder.

#### Changing the file paths
- For production:
  - In [FileHandler.cs](../Classroom-Seating-Planner/Classroom-Seating-Planner/src/FileHandler.cs) change the following properties:
    - `dataFolderName`, `classListFileName`, `classroomLayoutFileName`.
- For testing:
  - In [Utils.cs](../Classroom-Seating-Planner/Tests/Utils.cs) change the following properties:
    - `dataFolderName`, `classListFileName`, `classroomLayoutFileName`.
- Remember to change the file paths in the [file-system-interactions.md](./file-system-interactions.md) and in this file.

## List of names
- For production:
  - Default list of names if that file is missing, change:
    - `defaultClassList` in [FileHandler.cs](../Classroom-Seating-Planner/Classroom-Seating-Planner/src/FileHandler.cs).
- For testing:
  - Default list of names if that file is missing, change:
    - `defaultClassList` in [Utils.cs](../Classroom-Seating-Planner/Tests/Utils.cs).
  - List of names used in the tests, change:
    - `testingClassList` in [Utils.cs](../Classroom-Seating-Planner/Tests/Utils.cs).

## Classroom layout
- For production:
  - Default classroom layout if that file is missing, change:
    - `defaultClassroomLayout` in [FileHandler.cs](../Classroom-Seating-Planner/Classroom-Seating-Planner/src/FileHandler.cs).
- For testing:
  - Default classroom layout if that file is missing, change: 
    - `defaultClassroomLayout` in [Utils.cs](../Classroom-Seating-Planner/Tests/Utils.cs).
  - Classroom layout used in the tests, change: 
    - `testingClassroomLayout` in [Utils.cs](../Classroom-Seating-Planner/Tests/Utils.cs).


---

[Back to README.](../README.md)