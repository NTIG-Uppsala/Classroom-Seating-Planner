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

        // TODO - move to somewhere appropriate? - idea: "Src/Structs.cs"
        public struct Constraint
        {
            public string type;
            public List<string?> arguments;
            public int priority;
        }

        // TODO - move to somewhere more appropriate?
        public struct Student
        {
            public string name;
            public List<Constraint>? constraints;
        }

        public static List<Constraint>? InterpretStudentConstraints(string studentName, string rawConstraints)
        {
            Dictionary<string, Constraint> functionLookupTable = new() {
                { "nära",         new Constraint { type = "distance", arguments = [studentName, "near", null], priority = 1 }},
                { "intenära",     new Constraint { type = "distance", arguments = [studentName, "far",  null], priority = 1 }},
                { "inte nära",    new Constraint { type = "distance", arguments = [studentName, "far",  null], priority = 1 }},
                { "långtfrån",    new Constraint { type = "distance", arguments = [studentName, "far",  null], priority = 1 }},
                { "långt från",   new Constraint { type = "distance", arguments = [studentName, "far",  null], priority = 1 }},
                { "bredvid",      new Constraint { type = "adjacent", arguments = [studentName, "yes",  null], priority = 1 }},
                { "intebredvid",  new Constraint { type = "adjacent", arguments = [studentName, "no",   null], priority = 1 }},
                { "inte bredvid", new Constraint { type = "adjacent", arguments = [studentName, "no",   null], priority = 1 }},
            };

            Dictionary<string, string> recipientLookupTable = new() {
                {"tavlan",       "whiteboardCover"},
                {"tavla",        "whiteboardCover"},
                {"whiteboard",   "whiteboardCover"},
                {"whiteboards",  "whiteboardCover"},
                {"svartatavlan", "whiteboardCover"},
                {"klösbrädan",   "whiteboardCover"},
                {"dörren",       "door"},
                {"dörr",         "door"},
                {"fönstret",     "window"},
                {"fönster",      "window"},
                {"vindöga",      "window"},
            };

            List<Constraint> interpretedConstraints = [];

            List<string> rawConstraintsList = rawConstraints.Split('/').Select(rawConstraint => rawConstraint.Trim()).ToList();

            rawConstraintsList.ForEach(rawConstraint =>
            { // TODO - Whitespace remover Regex.Replace(input, @"\s", "")
                string trimmedConstraint = rawConstraint.ToLower();
                string? functionName = functionLookupTable.Keys
                    .ToList()
                    .Where(functionName => trimmedConstraint.StartsWith(functionName))
                    .FirstOrDefault();

                if (functionName == null) return;

                Constraint interpretedConstraint = functionLookupTable[functionName];

                // Isolate the recipient string
                string recipient = Regex.Replace(rawConstraint, @"\(.*\) ", "") // Remove priority (N)
                    .Replace(functionName, "") // TODO - whitespace remover, this is linked to the whitespace remover mentioned above
                    .Trim();

                // Look if the recipent is in the recipientLookupTable
                if (recipientLookupTable.TryGetValue(Regex.Replace(recipient, @"\s", "").ToLower(), out string? value))
                {
                    recipient = value;
                }
                interpretedConstraint.arguments[2] = recipient;

                // Find the priority of the constraint
                // Priority is the number insite the parenthesis (N)
                System.Text.RegularExpressions.Match match = Regex.Match(rawConstraint, @"\(([^)]+)\)");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int intValue))
                {
                    interpretedConstraint.priority = intValue; // Override default priority value
                }

                interpretedConstraints.Add(interpretedConstraint);
            });

            return interpretedConstraints;
        }

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
        public static List<Student> GetClassListFromFile()
        {
            List<Student> students = [];

            // TODO - ignore lines that start with "#" - CHECK THAT THIS WORKS
            // Get the list of student names from the class list file and return as a list
            FileHandler.ReadClassListFile()
                .ToList() // ForEach only exists for lists
                .ForEach(row =>
                {
                    Student student = new Student();

                    // Get the student name
                    string name = row.Split(':')[0].Trim();
                    student.name = name;

                    // Get the student's constraints
                    string? constraints = row.Split(':').LastOrDefault();
                    if (constraints != null && !constraints.Trim().Equals(string.Empty))
                    {
                        student.constraints = InterpretStudentConstraints(student.name, constraints.Trim());
                    }

                    if (!string.IsNullOrEmpty(student.name))
                    {
                        students.Add(student);
                    }
                });

            return students;
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
