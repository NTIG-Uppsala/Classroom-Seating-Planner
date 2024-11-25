using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Classroom_Seating_Planner.src
{
    public class FileHandler
    {
        // Global variables for file paths
        public static readonly string dataFolderName = "Bordsplaceringsgeneratorn";
        public static readonly string classListFileName = "klasslista.txt";

        public static readonly string dataFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), FileHandler.dataFolderName);
        public static readonly string classListFilePath = System.IO.Path.Combine(FileHandler.dataFolderPath, FileHandler.classListFileName);

        public static readonly List<string> defaultClassList =
        [
            "Förnamn Efternamn",
            "Förnamn Efternamn",
            "Förnamn Efternamn",
        ];

        // TODO - move this to a file and read that file instead
        public static string classroomLayoutString =
            "   TTTT\n" +
            "\n" +
            "BB BB BB BB BB\n" +
            "\n" +
            "BBBB       BBB\n" +
            "      BBBB\n" +
            "\n" +
            " BB BB  BB BB\n" +
            "\n" +
            "B BB BB  BB";

        // Used by InterpretClassroomLayoutString 
        public struct ClassroomLayoutData()
        {
            public int columnCount = 0;
            public int rowCount = 0;
            public List<cells.TableCell> tableCells = [];
            public List<cells.WhiteboardCell> whiteboardCells = [];
        }

        public static ClassroomLayoutData InterpretClassroomLayoutString(string classroomLayoutString)
        {
            ClassroomLayoutData returnObject = new();
            
            // We later find the biggest column width to set the column count
            List<int> xCoordinates = [];

            int rowIndex = 0;
            classroomLayoutString.Split("\n").ToList().ForEach((string row) =>
            {
                // Get every character in the row as a seperate char to iterate over
                int columnIndex = 0;
                row.ToList().ForEach((char letter) =>
                {
                    if (letter.Equals('T'))
                    {
                        returnObject.whiteboardCells.Add(new cells.WhiteboardCell(columnIndex, rowIndex));
                    }
                    else if (letter.Equals('B'))
                    {
                        returnObject.tableCells.Add(new cells.TableCell(columnIndex, rowIndex));
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

        public static void WriteDefaultClassListFile()
        {
            // Make sure the data folder exists
            if (!System.IO.Directory.Exists(FileHandler.dataFolderPath))
            {
                System.IO.Directory.CreateDirectory(FileHandler.dataFolderPath);
            }

            // Write the default list to the class list file
            System.IO.File.WriteAllText(FileHandler.classListFilePath, string.Join("\n", FileHandler.defaultClassList));
        }

        public static void HandleClassListFileIssues(Window parent)
        {
            // Create the class list file if it does not exist, write the default list to it, and return the "not found" message code
            if (!System.IO.File.Exists(FileHandler.classListFilePath))
            {
                WriteDefaultClassListFile();
                PopupWindow.FileIssuePopup("not found", parent);
            }

            // If the file exists, get its content as a list
            List<string> classListFileContent = GetClassListFromFile();

            // If the file is empty, write the default list to it and return the "empty" message code
            if (classListFileContent.SequenceEqual([]))
            {
                WriteDefaultClassListFile();
                PopupWindow.FileIssuePopup("empty", parent);
            }

            // If the file content is the same as the default list, return the "default" message code
            if (classListFileContent.SequenceEqual(FileHandler.defaultClassList))
            {
                PopupWindow.FileIssuePopup("default", parent);
            }
        }
    }
}
