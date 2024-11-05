using System.IO;

namespace Classroom_Seating_Planner.src
{
    public class FileHandler
    {
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
            // Get the paths to the app's directory and the names file
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsFolder, "Bordsplaceringsgeneratorn", "klasslista.txt");

            // Read the names from the file and return them as a list
            return GetDataFromFile(filePath);
        }
    }
}
