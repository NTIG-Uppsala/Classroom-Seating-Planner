using System.Diagnostics;
using System.IO;

namespace Classroom_Seating_Planner.src
{
    public class FileHandler
    {
        // Global variables for file paths
        private static readonly string dataFolderName = "Bordsplaceringsgeneratorn";
        private static readonly string studentNamesListFileName = "klasslista.txt";
        
        public static readonly string dataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), dataFolderName);
        public static readonly string studentNamesListFilePath = Path.Combine(dataFolderPath, studentNamesListFileName);

        public static readonly List<string> defaultStudentNamesList =
        [
            "Förnamn Efternamn",
            "Förnamn Efternamn",
            "Förnamn Efternamn",
        ];

        // Returns a list of data from an external file
        public static List<string> GetDataFromFileAsList(string filePath)
        {
            // Read the data from the file and return it as a list
            using StreamReader reader = new(filePath);
            List<string> dataList = reader
                .ReadToEnd()
                .Split("\n")
                .Select(item => item.Trim())
                .Where(item => !string.IsNullOrEmpty(item))
                .ToList();
            return dataList;
        }

        public static void WriteDefaultStudentNamesListFile()
        {
            // Make sure the data folder exists
            if (!Directory.Exists(dataFolderPath))
            {
                Directory.CreateDirectory(dataFolderPath);
            }

            // Write the default list to the student names list file
            File.WriteAllText(studentNamesListFilePath, string.Join("\n", defaultStudentNamesList));
        }

        // Handles the data in the student names list file
        public static string? CheckStudentNamesListFileForIssues()
        {
            // Create the student names list file if it does not exist, write the default list to it, and return the "not found" message code
            if (!File.Exists(studentNamesListFilePath))
            {
                WriteDefaultStudentNamesListFile();
                return "not found";
            }

            // If the file exists, get its content as a list
            List<string> studentListFileContent = GetDataFromFileAsList(studentNamesListFilePath);

            // If the file is empty, write the default list to it and return the "empty" message code
            if (studentListFileContent.SequenceEqual([]))
            {
                WriteDefaultStudentNamesListFile();
                return "empty";
            }

            // If the file content is the same as the default list, return the "default" message code
            if (studentListFileContent.SequenceEqual(defaultStudentNamesList))
            {
                return "default";
            }

            // Returns that there are no issues with the student names list file
            return null;
        }

        // Returns the list of student names read from an external file as a list
        public static List<string> GetStudentNamesFromFile()
        {
            // Get the list of student names from the student names list file
            List<string> studentListFileContent = GetDataFromFileAsList(studentNamesListFilePath);

            // Return the list of student names
            return studentListFileContent;
        }
    }
}
