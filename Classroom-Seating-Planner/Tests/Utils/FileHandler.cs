using FlaUI.Core.AutomationElements;
using System.Dynamic;
using FlaUIElement = FlaUI.Core.AutomationElements;

namespace Tests
{
    internal class FileHandler
    {
        // Path variables constructed with folder name and file name defined at the top of Utils
        public static readonly string dataFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Utils.dataFolderName);
        public static readonly string dataBackupFolderPath = $"{Utils.FileHandler.dataFolderPath}.bak";
        public static readonly string classListFilePath = System.IO.Path.Combine(Utils.FileHandler.dataFolderPath, Utils.classListFileName);
        public static readonly string classListBackupFilePath = $"{Utils.FileHandler.classListFilePath}.bak";
        public static readonly string classroomLayoutFilePath = System.IO.Path.Combine(Utils.FileHandler.dataFolderPath, Utils.classroomLayoutFileName);
        public static readonly string classroomLayoutBackupFilePath = $"{Utils.FileHandler.classroomLayoutFilePath}.bak";

        // Returns the list of students read from an external file as a list
        public static List<string> GetClassListFromFile()
        {
            // Read the names from the file and return them as a list
            return System.IO.File.ReadAllLines(Utils.FileHandler.classListFilePath)
                .Select(name => name.Trim())
                .Where(name => !string.IsNullOrEmpty(name)) // Remove empty lines)
                .ToList();
        }

        public static void RestoreDataFolder()
        {
            // Exit if the backup directory does not exist
            if (!System.IO.Directory.Exists(Utils.FileHandler.dataBackupFolderPath))
            {
                return;
            }

            // Empty the data directory
            if (System.IO.Directory.Exists(Utils.FileHandler.dataFolderPath))
            {
                System.IO.Directory.Delete(Utils.FileHandler.dataFolderPath, true);
            }
            System.IO.Directory.CreateDirectory(Utils.FileHandler.dataFolderPath);

            // Move all files from the backup directory to the data directory
            System.IO.Directory.GetFiles(Utils.FileHandler.dataBackupFolderPath).ToList().ForEach((string filePath) =>
            {
                System.IO.File.Move(filePath, System.IO.Path.Combine(Utils.FileHandler.dataFolderPath, System.IO.Path.GetFileName(filePath)));
            });
        }

        public static void RestoreAllDataFiles()
        {
            // If the data directory does not exist, abort and let the default files be created after this function
            if (!System.IO.Directory.Exists(Utils.FileHandler.dataFolderPath))
            {
                return;
            }

            // Find all backup files and restore them
            System.IO.Directory.GetFiles(Utils.FileHandler.dataFolderPath)
                .Where((dataFile) => dataFile.Contains(".bak"))
                .ToList()
                .ForEach((string dataFilePath) =>
                {
                    string dataBackupFileName = System.IO.Path.GetFileName(dataFilePath);
                    string dataFileName = dataBackupFileName.Remove(dataBackupFileName.Length - 4);
                    Utils.FileHandler.RestoreDataFile(dataFileName);
                });
        }

        public static void RestoreDataFile(string dataFileName)
        {
            string dataFilePath = System.IO.Path.Combine(Utils.FileHandler.dataFolderPath, dataFileName);
            string dataBackupFilePath = $"{dataFilePath}.bak";
            if (System.IO.File.Exists(dataBackupFilePath))
            {
                // Deletes the class list file and renames the backup file to take its place
                System.IO.File.Delete(dataFilePath);
                System.IO.File.Move(dataBackupFilePath, dataFilePath);
            }
        }

        public static void CreateDefaultDataFiles()
        {
            CreateDefaultDataFile(Utils.classListFileName, Utils.defaultClassList);
            CreateDefaultDataFile(Utils.classroomLayoutFileName, Utils.defaultClassroomLayout);
        }

        public static void CreateDefaultDataFile(string dataFileName, List<string> defaultFileData)
        {

            // Make sure that the data folder exists
            if (!System.IO.Directory.Exists(Utils.FileHandler.dataFolderPath))
            {
                System.IO.Directory.CreateDirectory(Utils.FileHandler.dataFolderPath);
            }

            // Create the default class list if no class list exists
            if (!System.IO.File.Exists(System.IO.Path.Join(Utils.FileHandler.dataFolderPath, dataFileName)))
            {
                System.IO.File.WriteAllLines(System.IO.Path.Join(Utils.FileHandler.dataFolderPath, dataFileName), defaultFileData);
            }
        }

        public static void CreateDataBackupFolderOfData()
        {
            // Move all files from the data folder to a backup folder
            System.IO.Directory.CreateDirectory(Utils.FileHandler.dataBackupFolderPath);
            foreach (string filePath in System.IO.Directory.GetFiles(Utils.FileHandler.dataFolderPath))
            {
                System.IO.File.Move(filePath, System.IO.Path.Combine(Utils.FileHandler.dataBackupFolderPath, System.IO.Path.GetFileName(filePath)));
            }
        }
    }
}
