using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Classroom_Seating_Planner.Src
{
    public class FileHandler
    {
        // Global variables for file paths
        public static readonly string dataFolderName = "Bordsplaceringsgeneratorn";
        public static readonly string classListFileName = "klasslista.txt";
        public static readonly string classroomLayoutFileName = "bordskarta.txt";

        public static readonly string dataFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), FileHandler.dataFolderName);
        public static readonly string classListFilePath = System.IO.Path.Combine(FileHandler.dataFolderPath, FileHandler.classListFileName);
        public static readonly string classroomLayoutFilePath = System.IO.Path.Combine(FileHandler.dataFolderPath, FileHandler.classroomLayoutFileName);

        public static readonly List<string> defaultClassList =
        [
            "Förnamn Efternamn",
            "Förnamn Efternamn",
            "Förnamn Efternamn",
        ];

        public static readonly List<string> defaultClassroomLayout =
        [
            "     TTTT",
            "",
            "BBBB BBBB BBBB",
            "",
            "BBBB BBBB BBBB",
            "",
            "BBBB BBBB BBBB",
        ];

        public static List<string> ReadClassListFile()
        {
            return System.IO.File.ReadAllLines(FileHandler.classListFilePath)
                .Select(row => row.Trim())
                .Where(row => !row.StartsWith('#')) // Comment rows
                .Where(row => !string.IsNullOrEmpty(row))
                .ToList();
        }

        public static List<string> ReadClassroomLayoutFile()
        {
            return System.IO.File.ReadAllLines(FileHandler.classroomLayoutFilePath)
                .Where(row => !row.StartsWith('#')) // Comment rows
                .ToList();
        }

        // Returns the list of student names read from an external file as a list
        public static List<ConstraintsHandler.Student> GetClassListFromFile()
        {
            List<ConstraintsHandler.Student> students = [];

            // Get the list of student names from the class list file and return as a list
            FileHandler.ReadClassListFile()
                .ToList() // ForEach only exists for lists
                .ForEach(row =>
                {
                    ConstraintsHandler.Student student = new ConstraintsHandler.Student();

                    // Get the student name
                    string name = row.Split(':')[0].Trim();
                    student.name = name;

                    // Get the student's constraints

                    string? constraints = row.Contains(':') ? row.Split(':')[1] : null;

                    if (constraints != null && !constraints.Trim().Equals(string.Empty))
                    {
                        student.constraints = ConstraintsHandler.InterpretStudentConstraints(student.name, constraints.Trim());
                    }

                    if (!string.IsNullOrEmpty(student.name))
                    {
                        students.Add(student);
                    }
                });

            return students;
        }

        public static Cells.Cell GetWhiteboardCover(List<Cells.Cell> classroomElements)
        {
            List<Cells.Cell> whiteboardCells = classroomElements.Where(cell => cell.cellType.Equals("whiteboard")).ToList();

            List<int> gridXCoordinatesList = [];
            List<int> gridYCoordinatesList = [];

            // Collect all the x and y coordinates for summing and finding the min and max values
            whiteboardCells.ForEach((Cells.Cell whiteboardCell) =>
            {
                gridXCoordinatesList.Add(whiteboardCell.gridX);
                gridYCoordinatesList.Add(whiteboardCell.gridY);
            });

            int largestX = gridXCoordinatesList.Max();
            int smallestX = gridXCoordinatesList.Min();

            int largestY = gridYCoordinatesList.Max();
            int smallestY = gridYCoordinatesList.Min();

            // Span of the cover
            int width = largestX - smallestX + 1;
            int height = largestY - smallestY + 1;

            // Whiteboard cover shares gridX and gridY with the top left most whiteboard cell and spans across the rest
            Cells.WhiteboardCoverCell whiteboardCoverCell = new(
                gridX: smallestX,
                gridY: smallestY,
                width: width,
                height: height
            );

            return whiteboardCoverCell;
        }

        public static List<Cells.Cell> GetClassroomElementsFromLayout()
        {
            List<Cells.Cell> classroomElements = [];

            Dictionary<char, Func<int, int, Cells.Cell>> cellTypes = new()
            {
                { 'B', (gridX, gridY) => new Cells.TableCell(gridX, gridY) },
                { 'T', (gridX, gridY) => new Cells.WhiteboardCell(gridX, gridY) }
            };

            // Parse layout character by character and add them to the classroomElements list
            int y = 0;
            ReadClassroomLayoutFile().ForEach((row) =>
            {
                int x = 0;
                row.ToList().ForEach((character) =>
                {
                    if (cellTypes.ContainsKey(character))
                    {
                        classroomElements.Add(cellTypes[character](x, y));
                    }
                    x++;
                });
                y++;
            });

            // Whiteboard cover gets added here since it depends on whiteboard cells existing in the classroom elements list already
            if (classroomElements.Any(element => element.cellType.Equals("whiteboard")))
            {
                classroomElements.Add(GetWhiteboardCover(classroomElements));
            }

            return classroomElements;
        }



        // Used by InterpretClassroomLayoutString 
        public struct ClassroomLayoutData()
        {
            public int columnCount = 0;
            public int rowCount = 0;
            public List<Cells.TableCell> tableCells = [];
            public List<Cells.WhiteboardCell> whiteboardCells = [];
        }

        public static ClassroomLayoutData GetClassroomLayoutDataFromFile()
        {
            List<string> classroomLayout = System.IO.File.ReadAllLines(classroomLayoutFilePath).ToList();
            ClassroomLayoutData returnObject = new();

            // We later find the biggest column width to set the column count
            List<int> xCoordinates = [];

            int rowIndex = 0;
            classroomLayout.ForEach((string row) =>
            {
                // Get every character in the row as a seperate char to iterate over
                int columnIndex = 0;
                row.ToList().ForEach((char letter) =>
                {
                    if (letter.Equals('T'))
                    {
                        returnObject.whiteboardCells.Add(new Cells.WhiteboardCell(columnIndex, rowIndex));
                    }
                    else if (letter.Equals('B'))
                    {
                        returnObject.tableCells.Add(new Cells.TableCell(columnIndex, rowIndex));
                    }

                    columnIndex++;
                });
                xCoordinates.Add(columnIndex);

                rowIndex++;
            });

            int layoutWidth = xCoordinates.Max();
            int layoutHeight = rowIndex;

            returnObject.columnCount = layoutWidth;
            returnObject.rowCount = layoutHeight;

            return returnObject;
        }

        public static void HandleAllDataFileIssues(System.Windows.Window parent)
        {
            // Create booleans regarding the existance of the files at the start of the program
            bool classListFileExists = System.IO.File.Exists(FileHandler.classListFilePath);
            bool classroomLayoutFileExists = System.IO.File.Exists(FileHandler.classroomLayoutFilePath);

            // Give popup warning if either file does not exist
            if (!classListFileExists || !classroomLayoutFileExists)
            {
                PopupWindow.FileIssuePopup("notAllFilesWereFound", parent);

                if (!classListFileExists)
                {
                    CreateDefaultClassListFile();
                }

                if (!classroomLayoutFileExists)
                {
                    CreateDefaultClassroomLayoutFile();
                }
            }

            if (classListFileExists)
            {
                HandleClassListFileIssues(parent);
            }

            if (classroomLayoutFileExists)
            {
                HandleClassroomLayoutFileIssues(parent);
            }

            // Check if there are more students than there are available seats/tables
            int numberOfStudents = FileHandler.ReadClassListFile().Count;
            int numberOfTables = System.IO.File.ReadAllText(FileHandler.classroomLayoutFilePath).Count(letter => letter.Equals('B'));
            if (numberOfStudents > numberOfTables)
            {
                PopupWindow.FileIssuePopup("moreStudentsThanTables", parent);
            }
        }

        public static void CreateDefaultClassListFile()
        {
            // Make sure the data folder exists
            if (!System.IO.Directory.Exists(FileHandler.dataFolderPath))
            {
                System.IO.Directory.CreateDirectory(FileHandler.dataFolderPath);
            }

            // Write the default list to the class list file
            System.IO.File.WriteAllLines(FileHandler.classListFilePath, FileHandler.defaultClassList);
        }

        public static void HandleClassListFileIssues(System.Windows.Window parent)
        {
            // If the file exists, get its content as a list
            List<string> classListFileContent = System.IO.File.ReadAllLines(FileHandler.classListFilePath).ToList();

            // If the file is empty, write the default list to it and return the "empty" message code
            if (classListFileContent.SequenceEqual([]))
            {
                CreateDefaultClassListFile();
                PopupWindow.FileIssuePopup("emptyClassList", parent);
                return;
            }

            // If the file content is the same as the default list, return the "default" message code
            if (classListFileContent.SequenceEqual(FileHandler.defaultClassList))
            {
                PopupWindow.FileIssuePopup("defaultClassList", parent);
                return;
            }
        }

        public static void CreateDefaultClassroomLayoutFile()
        {
            // Make sure the data folder exists
            if (!System.IO.Directory.Exists(FileHandler.dataFolderPath))
            {
                System.IO.Directory.CreateDirectory(FileHandler.dataFolderPath);
            }

            // Write the default layout to the classroom layout file
            System.IO.File.WriteAllLines(FileHandler.classroomLayoutFilePath, FileHandler.defaultClassroomLayout);
        }

        public static void HandleClassroomLayoutFileIssues(System.Windows.Window parent)
        {
            // If the file exists, get its content as a list
            List<string> classroomLayoutFileContent = System.IO.File.ReadAllLines(FileHandler.classroomLayoutFilePath).ToList();

            // If the file is empty or only contains whitespace, write the default list to it and return the "empty" message code
            if (classroomLayoutFileContent.SequenceEqual([]) || classroomLayoutFileContent.All((string row) => row.Trim().Length.Equals(0)))
            {
                CreateDefaultClassroomLayoutFile();
                PopupWindow.FileIssuePopup("emptyClassroomLayout", parent);
                return;
            }

            // If the file contains no tables, display a popup
            if (!classroomLayoutFileContent.Any((classroomLayoutFileLine) => classroomLayoutFileLine.Contains('B')))
            {
                PopupWindow.FileIssuePopup("noTablesInLayout", parent);
            };

            // If the file contains no whiteboards, display a popup
            if (!classroomLayoutFileContent.Any((classroomLayoutFileLine) => classroomLayoutFileLine.Contains('T')))
            {
                PopupWindow.FileIssuePopup("noWhiteboardsInLayout", parent);
            };
        }
    }
}
