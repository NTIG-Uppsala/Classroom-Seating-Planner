using System.IO;

namespace Classroom_Seating_Planner.src
{
    public class FileHandler
    {
        // Public method for fetching student names from an external file
        public static List<string> GetStudentNamesFromFile()
        {
            // Get the paths to the app's directory and the names file
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsFolder, "Bordsplaceringsgeneratorn", "klasslista.txt");

            // Read the names from the file and return them as a list
            using StreamReader reader = new(filePath);
            List<string> names = reader
                .ReadToEnd()
                .Split("\n")
                .Select(name => name.Trim())
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList();
            return names;
        }
    }
}
