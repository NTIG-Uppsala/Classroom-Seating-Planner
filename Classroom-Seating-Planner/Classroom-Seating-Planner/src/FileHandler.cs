using System.Diagnostics;
using System.IO;

namespace Classroom_Seating_Planner.src
{
    public class FileHandler
    {
        // Global variables for file paths
        public static readonly string dataFolderName = "Bordsplaceringsgeneratorn";
        public static readonly string studentNamesListFileName = "klasslista.txt";

        public static readonly string dataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), dataFolderName);
        public static readonly string studentNamesListFilePath = Path.Combine(dataFolderPath, studentNamesListFileName);

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

        // Returns the list of student names read from an external file as a list
        public static (List<string>, string? error) GetStudentNamesFromFile()
        {
            string filePath = studentNamesListFilePath;

            // Default list of student names
            List<string> studentListDefault =
                [
                    "Förnamn Efternamn",
                    "Förnamn Efternamn",
                    "Förnamn Efternamn",
                ];

            string studentListDefaultString = string.Join("\n", studentListDefault);

            // Create the file if it does not exist
            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory(dataFolderPath);
                File.WriteAllText(filePath, studentListDefaultString);
                return ([], "not found");
            }

            // Read the names from the file and return them as a list
            List<string> studentNamesList = GetDataFromFileAsList(filePath);

            // Check if the list is empty and create a default list if it is
            if (studentNamesList.SequenceEqual([]))
            {
                File.WriteAllText(filePath, studentListDefaultString);
                return ([], "empty");
            }

            // Check if the list is the default list
            if (studentNamesList.SequenceEqual(studentListDefault))
            {
                return ([], "default");
            }

            // Return the list of student names
            return (studentNamesList, null);
        }
    }
}
