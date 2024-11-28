using System.Diagnostics;
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

        // Returns the list of student names read from an external file as a list
        public static List<string> GetClassListFromFile()
        {
            // Get the list of student names from the class list file and return as a list
            return System.IO.File.ReadAllLines(FileHandler.classListFilePath)
                .Select(name => name.Trim())
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList();
        }

        public static void HandleAllDataFileIssues(System.Windows.Window parent)
        {
            HandleClassroomLayoutFileIssues(parent);
            HandleClassListFileIssues(parent);

            // Check if there are more students than there are available seats/tables
            int numberOfStudents = GetClassListFromFile().Count;
            int numberOfTables = System.IO.File.ReadAllText(FileHandler.classroomLayoutFilePath).Count(letter => letter == 'B');
            if (numberOfStudents > numberOfTables)
            {
                PopupWindow.FileIssuePopup("moreStudentsThanTables", parent);
            }
        }

        public static void WriteDefaultClassListFile()
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
            // Create the class list file if it does not exist, write the default list to it, and return the "not found" message code
            if (!System.IO.File.Exists(FileHandler.classListFilePath))
            {
                WriteDefaultClassListFile();
                PopupWindow.FileIssuePopup("noClassList", parent);
                return;
            }

            // If the file exists, get its content as a list
            List<string> classListFileContent = GetClassListFromFile();

            // If the file is empty, write the default list to it and return the "empty" message code
            if (classListFileContent.SequenceEqual([]))
            {
                WriteDefaultClassListFile();
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
    
        public static void WriteDefaultClassroomLayoutFile()
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
            // Create the classroom layout file if it does not exist, write the default layout to it, and return the "not found" message code
            if (!System.IO.File.Exists(FileHandler.classroomLayoutFilePath))
            {
                WriteDefaultClassroomLayoutFile();
                return;
            }

            // If the file exists, get its content as a list
            List<string> classroomLayoutFileContent = System.IO.File.ReadAllLines(FileHandler.classroomLayoutFilePath).ToList();

            // If the file is empty or only contains whitespace, write the default list to it and return the "empty" message code
            if (classroomLayoutFileContent.SequenceEqual([]) || classroomLayoutFileContent.All((string row) => row.Trim().Length.Equals(0)))
            {
                WriteDefaultClassroomLayoutFile();
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
