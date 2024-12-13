# File System Interactions

This project requires external files and folders to use. These files are generated automatically but can be manually changed.

The files are:

-   `%USERPROFILE%\Documents\Bordsplaceringsgeneratorn\klasslista.txt`
-   `%USERPROFILE%\Documents\Bordsplaceringsgeneratorn\bordskarta.txt`

## File paths
File paths are constructed as such:
`<dataFolderPath>\<fileName>`

Where the `dataFolderPath` is the path to the programs data folder and `fileName` is the name of the file stored within the data folder.

### Changing the file paths and file names
- For production:
  - In [FileHandler.cs](../Classroom-Seating-Planner/Classroom-Seating-Planner/src/FileHandler.cs) change the following properties:
    - `dataFolderName`, `classListFileName`, `classroomLayoutFileName`.
- For testing:
  - In [Utils.cs](../Classroom-Seating-Planner/Tests/Utils.cs) change the following properties:
    - `dataFolderName`, `classListFileName`, `classroomLayoutFileName`.
- Remember to change the file paths in the [file-system-interactions.md](./file-system-interactions.md) and in this file.

---

[Back to README.](../README.md)