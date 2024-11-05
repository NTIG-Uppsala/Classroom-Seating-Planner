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
        public static List<string>? GetStudentNamesFromFile()
        {
            string filePath = studentNamesListFilePath;

            if (!File.Exists(filePath))
            {
                // Create the file if it does not exist and fill it with placeholder data to guide the user to the correct structure
                Directory.CreateDirectory(dataFolderPath);
                List<string> studentListPlaceholder =
                [
                    "Förnamn Efternamn 1",
                    "Förnamn Efternamn 2",
                    "Förnamn Efternamn 3",
                ];

                File.WriteAllText(filePath, string.Join("\n", studentListPlaceholder));
                return null;
            }

            // Read the names from the file and return them as a list
            List<string> studentNamesList = GetDataFromFileAsList(filePath);
            return studentNamesList;
        }
    }
}
