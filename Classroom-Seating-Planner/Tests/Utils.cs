using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using FlaUIElement = FlaUI.Core.AutomationElements;

namespace Tests
{
    internal class Utils
    {
        // Variables for file paths
        public static readonly string dataFolderName = "Bordsplaceringsgeneratorn";
        public static readonly string classListFileName = "klasslista.txt";

        // Names for default file if file does not already exist
        public static readonly List<string> defaultClassList = [
                "Förnamn Efternamn",
                "Förnamn Efternamn",
                "Förnamn Efternamn",
            ];

        // Class list used for tests unless another list is specified
        private static readonly List<string> testingClassList = [
            "Ziggy Stardust",
            "Frodo Baggins",
            "Darth Vader",
            "Galadriel Silverleaf",
            "Sparky McFluff",
            "Waldo B. Lost",
            "กาญจนา McSix",
            "Gandalf the Grey",
            "Ulysses 'Snakehands' McDougall",
            "Venkatanarasimharajuvaripeta Wumpus",
            "Shivankumaraswamy Krishnamurthy Raghunath",
            "الحسيني",
            "Muhammad Abdelrahman ibn Al-Mahmoud al-Farouq",
            "Papadopoulos-Alexandropoulos Firestorm",
            "明张",
            "Pipkin Puddleduck",
            "Aleksandrovich Dimitrov Petrovskaya Ivanov",
            "Per-Göran Karlsson von Heidenstam af Skånesläkten",
            "Wiggles Snickerbottom",
            "Zephyr Nightwind",
            "Doodlebug Sparklestep",
            "Sir Adrian Carton de Wiart",
            "Tinkerbell Twinkletoes",
            "Bo Li",
            "Dinglehopper Wobblesworth",
            "Kǎi McQuirk",
            "Fizzlewhit Wobblebottom",
            "鈴木 健太",
            "Jo Wu",
            "Le To",
            "Örjan Johansson Florist",
            "Främling Skådespelare",
            "Émil Låås",
        ];

        // SetUp method
        public static (FlaUI.Core.Application, FlaUI.UIA3.UIA3Automation, FlaUIElement.Window, FlaUI.Core.Conditions.ConditionFactory)
            SetUp(List<string>? testClassList = null, bool ignoreClassListFileBackup = false, bool ignoreTestingClassList = false, bool createDataBackupFolder = false, bool deleteClassListFile = false, bool deleteDataFolder = false)
        {
            // Use default testing class list unless a list is specified
            testClassList ??= testingClassList;

            // Restore backup folder if it exists
            Utils.FileHandler.RestoreBackupFolder();

            // Restore backup data if backup file already exists
            Utils.FileHandler.RestoreBackupFile();

            // Create the data folder and the default class list file if they don't exist
            Utils.FileHandler.CreateDefaultClassListFile();

            if (!ignoreClassListFileBackup)
            {
                // Backup the class list file so it can be restored after testing
                System.IO.File.Copy(Utils.FileHandler.classListFilePath, $"{Utils.FileHandler.classListFilePath}.bak");
            }

            if (!ignoreTestingClassList)
            {
                // Insert the test class list into the file
                System.IO.File.WriteAllLines(Utils.FileHandler.classListFilePath, testClassList);
            }

            if (createDataBackupFolder)
            {
                Utils.FileHandler.CreateDataBackupFolder();
            }

            if (deleteClassListFile)
            {
                System.IO.File.Delete(Utils.FileHandler.classListFilePath);
            }

            if (deleteDataFolder)
            {
                System.IO.Directory.Delete(Utils.FileHandler.dataFolderPath);
            }

            // Return values necessary for running the test
            return InitializeFlaUIApp();
        }

        // TearDown method
        public static void TearDown(FlaUI.Core.Application app)
        {
            // Restore the class list file by filling it with backed up information from before the test
            Utils.FileHandler.RestoreBackupFile();

            // Terminate the app
            app.Close();
        }

        public static (FlaUI.Core.Application, FlaUI.UIA3.UIA3Automation, FlaUIElement.Window, FlaUI.Core.Conditions.ConditionFactory) InitializeFlaUIApp()
        {
            // Find and run the application
            FlaUI.Core.Application app = FlaUI.Core.Application.Launch("..\\..\\..\\..\\Classroom-Seating-Planner\\bin\\Debug\\net8.0-windows\\win-x64\\Classroom-Seating-Planner.exe");
            FlaUI.UIA3.UIA3Automation automation = new();

            // Find the main window for the purpose of finding elements
            FlaUIElement.Window? window = app.GetMainWindow(automation);
            FlaUI.Core.Conditions.ConditionFactory cf = new(new UIA3PropertyLibrary());

            return (app, automation, window, cf);
        }

        public class XAMLHandler
        {
            // Returns list of ListBox items
            public static List<string> GetClassListFromElement(FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
            {
                string automationId = "ClassListElement";
                FlaUIElement.ListBox classListElement = window.FindFirstDescendant(cf.ByAutomationId(automationId)).AsListBox();
                List<FlaUIElement.ListBoxItem> classListAsItems = classListElement.Items.ToList();
                List<string> classList = classListAsItems.Select(item => item.Text).ToList();

                return classList;
            }

            public static void ClickRandomizeSeatingButton(FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
            {
                // Find and press the randomizer button
                FlaUIElement.AutomationElement randomizeButton = window.FindFirstDescendant(cf.ByAutomationId("RandomizeSeatingButton")).AsButton();
                randomizeButton.Click();
            }

            public static List<FlaUIElement.AutomationElement> GetAllByAutomationId(FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf, string identifier, FlaUI.Core.Definitions.ControlType? controlType = null)
            {
                // Find all element
                List<AutomationElement> allElements = window.FindAllDescendants(cf.ByFrameworkId("WPF")).ToList();

                // Find all seats
                List<FlaUIElement.AutomationElement> allSeats = allElements.Where(element =>
                    !string.IsNullOrEmpty(element.AutomationId)
                    &&
                    element.AutomationId.Contains(identifier)
                    &&
                    (controlType == null || element.ControlType.Equals(controlType))
                ).ToList();

                return allSeats;
            }
        }

        public class FileHandler
        {
            // Path variables constructed with folder name and file name defined at the top of Utils
            public static readonly string dataFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), dataFolderName);
            public static readonly string dataBackupFolderPath = $"{dataFolderPath}.bak";
            public static readonly string classListFilePath = System.IO.Path.Combine(dataFolderPath, classListFileName);
            public static readonly string classListBackupFilePath = $"{classListFilePath}.bak";

            // Returns the list of students read from an external file as a list
            public static List<string> GetClassListFromFile()
            {
                // Read the names from the file and return them as a list
                // TODO - File.ReadAllLines?
                //using System.IO.StreamReader reader = new(Utils.FileHandler.classListFilePath);
                //List<string> classList = reader
                //    .ReadToEnd()
                //    .Split("\n")
                //    .Select(name => name.Trim())
                //    .Where(name => !string.IsNullOrEmpty(name)) // Remove empty lines
                //    .ToList();
                //return classList;
                return System.IO.File.ReadAllLines(Utils.FileHandler.classListFilePath)
                    .Select(name => name.Trim())
                    .Where(name => !string.IsNullOrEmpty(name)) // Remove empty lines)
                    .ToList();
            }

            public static void RestoreBackupFolder()
            {
                if (System.IO.Directory.Exists(Utils.FileHandler.dataBackupFolderPath))
                {
                    // Make sure there is an empty data folder
                    if (System.IO.Directory.Exists(Utils.FileHandler.dataFolderPath)) { System.IO.Directory.Delete(Utils.FileHandler.dataFolderPath, true); }
                    System.IO.Directory.CreateDirectory(Utils.FileHandler.dataFolderPath);

                    // Move all files back from the backup folder to the data folder
                    foreach (string filePath in System.IO.Directory.GetFiles(Utils.FileHandler.dataBackupFolderPath))
                    {
                        System.IO.File.Move(filePath, System.IO.Path.Combine(Utils.FileHandler.dataFolderPath, System.IO.Path.GetFileName(filePath)));
                    }
                }
            }

            public static void RestoreBackupFile()
            {
                if (System.IO.File.Exists(Utils.FileHandler.classListBackupFilePath))
                {
                    // Deletes the class list file and renames the backup file to take its place
                    System.IO.File.Delete(Utils.FileHandler.classListFilePath);
                    System.IO.File.Move(Utils.FileHandler.classListBackupFilePath, Utils.FileHandler.classListFilePath);
                }
            }

            public static void CreateDefaultClassListFile()
            {
                // Make sure that the data folder exists
                if (!System.IO.Directory.Exists(Utils.FileHandler.dataFolderPath))
                {
                    System.IO.Directory.CreateDirectory(Utils.FileHandler.dataFolderPath);
                }

                // Create the default class list if no class list exists
                if (!System.IO.File.Exists(Utils.FileHandler.classListFilePath))
                {
                    System.IO.File.WriteAllLines(Utils.FileHandler.classListFilePath, Utils.defaultClassList);
                }
            }

            public static void CreateDataBackupFolder()
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
}
