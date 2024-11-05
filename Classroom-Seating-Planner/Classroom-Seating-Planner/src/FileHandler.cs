using System.IO;

namespace Classroom_Seating_Planner.src
{
    public class FileHandler
    {
        // TODO - This needs to be in the docs
        private static readonly string dataFolderName = "Bordsplaceringsgeneratorn";
        private static readonly string studentNamesListFileName = "klasslista.txt";

        public static readonly string dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), dataFolderName);
        public static readonly string studentNamesListFilePath = Path.Combine(dataFolder, studentNamesListFileName);

        // Public method for fetching data from an external file and returning it as a list
        public static List<string> GetDataFromFile(string filePath)
        {
            // Read the data from the file and return it as a list
            using StreamReader reader = new(filePath);
            List<string> data = reader
                .ReadToEnd()
                .Split("\n")
                .Select(item => item.Trim())
                .Where(item => !string.IsNullOrEmpty(item))
                .ToList();
            return data;
        }

        // Public method for fetching student names from an external file and returning them as a list
        public static List<string> GetStudentNamesFromFile()
        {
            string filePath = studentNamesListFilePath;

            // Read the names from the file and return them as a list
            List<string> studentNamesList = GetDataFromFile(filePath);
            return studentNamesList;
        }
    }
}
