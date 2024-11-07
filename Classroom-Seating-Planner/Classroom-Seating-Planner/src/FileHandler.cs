using System.Diagnostics;
using System.IO;

namespace Classroom_Seating_Planner.src
{
    public class FileHandler
    {
        // Global variables for file paths
        private static readonly string dataFolderName = "Bordsplaceringsgeneratorn";
        private static readonly string classListFileName = "klasslista.txt";
        
        public static readonly string dataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), dataFolderName);
        public static readonly string classListFilePath = Path.Combine(dataFolderPath, classListFileName);

        public static readonly List<string> defaultClassList =
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

        public static void WriteDefaultClassListFile()
        {
            // Make sure the data folder exists
            if (!Directory.Exists(dataFolderPath))
            {
                Directory.CreateDirectory(dataFolderPath);
            }

            // Write the default list to the class list file
            File.WriteAllText(classListFilePath, string.Join("\n", defaultClassList));
        }

        // Handles the data in the class list file
        public static string? CheckClassListFileForIssues()
        {
            // Create the class list file if it does not exist, write the default list to it, and return the "not found" message code
            if (!File.Exists(classListFilePath))
            {
                WriteDefaultClassListFile();
                return "not found";
            }

            // If the file exists, get its content as a list
            List<string> classListFileContent = GetDataFromFileAsList(classListFilePath);

            // If the file is empty, write the default list to it and return the "empty" message code
            if (classListFileContent.SequenceEqual([]))
            {
                WriteDefaultClassListFile();
                return "empty";
            }

            // If the file content is the same as the default list, return the "default" message code
            if (classListFileContent.SequenceEqual(defaultClassList))
            {
                return "default";
            }

            // Returns that there are no issues with the class list file
            return null;
        }

        // Returns the list of student names read from an external file as a list
        public static List<string> GetClassListFromFile()
        {
            // Get the list of student names from the class list file
            List<string> classListFileContent = GetDataFromFileAsList(classListFilePath);

            // Return the list of student names
            return classListFileContent;
        }
    }
}
