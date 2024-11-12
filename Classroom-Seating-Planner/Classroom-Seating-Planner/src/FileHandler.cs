using System.Windows;

namespace Classroom_Seating_Planner.src
{
    public class FileHandler
    {
        // Global variables for file paths
        private static readonly string dataFolderName = "Bordsplaceringsgeneratorn";
        private static readonly string classListFileName = "klasslista.txt";

        public static readonly string dataFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), FileHandler.dataFolderName);
        public static readonly string classListFilePath = System.IO.Path.Combine(FileHandler.dataFolderPath, FileHandler.classListFileName);

        public static readonly List<string> defaultClassList =
        [
            "Förnamn Efternamn",
            "Förnamn Efternamn",
            "Förnamn Efternamn",
        ];

        // Returns the list of student names read from an external file as a list
        public static List<string> GetClassListFromFile()
        {
            // Get the list of student names from the class list file and return as a list
            using System.IO.StreamReader reader = new(FileHandler.classListFilePath);
            List<string> dataList = reader
                .ReadToEnd()
                .Split("\n")
                .Select(item => item.Trim())
                .Where(item => !string.IsNullOrEmpty(item))
                .ToList();
            return dataList;
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

        // Handles the data in the class list file
        public static void CheckClassListFileForIssues(Window parent)
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
