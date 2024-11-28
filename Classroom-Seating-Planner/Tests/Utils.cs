using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FlaUIElement = FlaUI.Core.AutomationElements;

namespace Tests
{
    internal class Utils
    {
        // Variables for file paths
        public static readonly string dataFolderName = "Bordsplaceringsgeneratorn";
        public static readonly string classListFileName = "klasslista.txt";
        public static readonly string classroomLayoutFileName = "bordskarta.txt";

        // Names for default file if file does not already exist
        public static readonly List<string> defaultClassList =
        [
            "Förnamn Efternamn",
            "Förnamn Efternamn",
            "Förnamn Efternamn",
        ];

        // Class list used for tests unless another list is specified
        private static readonly List<string> testingClassList =
        [
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

        // Classroom layout for default file if file does not already exist
        public static readonly List<string> defaultClassroomLayout =
        [
            "     TTTT",
            "",
            "BBBB BBBB BBBB",
            "",
            "BBBB BBBB BBBB",
            "",
            "BBBB BBBB BBBB",
        ];

        // Classroom layout used for tests unless another layout is specified
        public static readonly List<string> testingClassroomLayout =
        [
            "   TTTT",
            "",
            "BB BB BB BB BB",
            "",
            "BBBB       BBB",
            "      BBBB",
            "",
            " BB BB  BB BB",
            "",
            "B BB BB  BB",
        ];

        // SetUp method
        public static (FlaUI.Core.Application, FlaUI.UIA3.UIA3Automation, FlaUIElement.Window, FlaUI.Core.Conditions.ConditionFactory)
            SetUp(List<string>? testClassList = null, List<string>? testClassroomLayout = null, bool ignoreClassListFileBackup = false, bool ignoreClassroomLayoutFileBackup = false, bool ignoreTestingClassList = false, bool ignoreTestingClassroomLayout = false, bool createDataBackupFolder = false, bool deleteClassListFile = false, bool deleteClassroomLayoutFile = false, bool deleteDataFolder = false)
        {
            // Use default testing class list and classroom layout unless a list is specified
            testClassList ??= Utils.testingClassList;
            testClassroomLayout ??= Utils.testingClassroomLayout;

            // Restore backup folder if it exists
            Utils.FileHandler.RestoreDataFolder();

            // Restore backup data if backup files already exist
            Utils.FileHandler.RestoreAllDataFiles();

            // Create the data folder and the default class list file if they don't exist
            Utils.FileHandler.CreateDefaultDataFiles();

            if (!ignoreClassListFileBackup)
            {
                // Backup the class list file so it can be restored after testing
                System.IO.File.Copy(Utils.FileHandler.classListFilePath, Utils.FileHandler.classListBackupFilePath);
            }

            if (!ignoreClassroomLayoutFileBackup)
            {
                // Backup the classroom layout file so it can be restored after testing
                System.IO.File.Copy(Utils.FileHandler.classroomLayoutFilePath, Utils.FileHandler.classroomLayoutBackupFilePath);
            }

            if (!ignoreTestingClassList)
            {
                // Insert the test class list into the file
                System.IO.File.WriteAllLines(Utils.FileHandler.classListFilePath, testClassList);
            }

            if (!ignoreTestingClassroomLayout)
            {
                // Insert the test classroom layout into the file
                System.IO.File.WriteAllLines(Utils.FileHandler.classroomLayoutFilePath, testClassroomLayout);
            }

            // Condition used by specific tests to create a backup folder
            if (createDataBackupFolder)
            {
                Utils.FileHandler.CreateDataBackupFolderOfData();
            }

            // Condition used by specific tests to delete the class list file
            if (deleteClassListFile)
            {
                System.IO.File.Delete(Utils.FileHandler.classListFilePath);
            }

            if (deleteClassroomLayoutFile)
            {
                System.IO.File.Delete(Utils.FileHandler.classroomLayoutFilePath);
            }

            // Condition used by specifi tests to delete the data folder
            if (deleteDataFolder)
            {
                System.IO.Directory.Delete(Utils.FileHandler.dataFolderPath);
            }

            // Return values necessary for running the test
            return Utils.Helpers.InitializeFlaUIApp();
        }

        // TearDown method
        public static void TearDown(FlaUI.Core.Application app)
        {
            // Restore backup folder if it exists
            Utils.FileHandler.RestoreDataFolder();

            // Restore the data files by filling them with backed up information from before the test
            Utils.FileHandler.RestoreAllDataFiles();

            // Terminate the app
            app.Close();
        }


        private class Helpers
        {
            public static (FlaUI.Core.Application, FlaUI.UIA3.UIA3Automation, FlaUIElement.Window, FlaUI.Core.Conditions.ConditionFactory) InitializeFlaUIApp()
            {
                // Find and run the application
                FlaUI.Core.Application app = FlaUI.Core.Application.Launch("..\\..\\..\\..\\Classroom-Seating-Planner\\bin\\Debug\\net8.0-windows\\win-x64\\Classroom-Seating-Planner.exe");
                FlaUI.UIA3.UIA3Automation automation = new();

                // Find the main window for the purpose of finding elements
                FlaUIElement.Window window = app.GetMainWindow(automation);
                FlaUI.Core.Conditions.ConditionFactory cf = new(new UIA3PropertyLibrary());

                return (app, automation, window, cf);
            }
        }

        public class XAMLHandler
        {
            public record Options(bool matchWholeString = false)
            {
                public bool matchWholeString { get; set; } = matchWholeString;
            }

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

            public static List<FlaUIElement.AutomationElement>?
                GetAllElementsByAutomationId(
                    FlaUIElement.Window window,
                    FlaUI.Core.Conditions.ConditionFactory cf,
                    string? matchString = null,
                    FlaUI.Core.Definitions.ControlType? controlType = null,
                    Options? options = null
                )
            {
                if (matchString == null)
                {
                    return null;
                }

                // Use default options for non-defined values
                options ??= new Options();

                // Find all elements
                List<FlaUIElement.AutomationElement> allElementsInWindow = window.FindAllDescendants(cf.ByFrameworkId("WPF")).ToList();

                // Return all elements where AutomationId contains value or is equal to value
                List<FlaUIElement.AutomationElement> allElementsByAutomationId = allElementsInWindow.Where(element =>
                {

                    // Match the string differently depending on the given option
                    bool isStringMatched = false;
                    if (options.matchWholeString.Equals(true))
                    {
                        isStringMatched = element.AutomationId.Equals((string)matchString);
                    }
                    else if (options.matchWholeString.Equals(false))
                    {
                        isStringMatched = element.AutomationId.Contains((string)matchString);
                    }

                    return
                    !string.IsNullOrEmpty(element.AutomationId)
                    &&
                    element.AutomationId.Contains(matchString)
                    &&
                    (controlType == null || element.ControlType.Equals(controlType));
                }).ToList();

                return allElementsByAutomationId;
            }

            public static List<FlaUIElement.AutomationElement>?
                GetAllElementsByHelpText(
                    FlaUIElement.Window window,
                    FlaUI.Core.Conditions.ConditionFactory cf,
                    string? key = null,
                    object? value = null,
                    FlaUI.Core.Definitions.ControlType? controlType = null,
                    Options? options = null
                )
            {
                // If neither a value nor a key is passed, return null
                if (key == null && value == null)
                {
                    return null;
                }

                // Use default options for non-defined values
                options ??= new Options();

                // Find all elements
                List<FlaUIElement.AutomationElement> allElementsInWindow = window.FindAllDescendants(cf.ByFrameworkId("WPF")).ToList();

                // If only a key is passed, return all elements where HelpText contains key
                if (key != null && value == null)
                {
                    List<FlaUIElement.AutomationElement> allElementsByHelpText = allElementsInWindow.Where(element =>
                        !string.IsNullOrEmpty(element.HelpText)
                        &&
                        (
                            element.HelpText.Contains(key)
                            &&
                            !Utils.XAMLHandler.ParseStringToObject(element.HelpText)[key].Equals(null)
                        )
                        &&
                        (controlType == null || element.ControlType.Equals(controlType))
                    ).ToList();

                    return allElementsByHelpText;
                }
                // If only a value is passed, return all elements where HelpText is equal to value or contains the value
                if (key == null && value != null)
                {
                    List<FlaUIElement.AutomationElement> allElementsByHelpText = allElementsInWindow.Where(element =>
                    {

                        // Match the string differently depending on the given option
                        bool isStringMatched = false;
                        if (options.matchWholeString.Equals(true))
                        {
                            isStringMatched = element.HelpText.Equals((string)value);
                        }
                        else if (options.matchWholeString.Equals(false))
                        {
                            isStringMatched = element.HelpText.Contains((string)value);
                        }

                        return
                        !string.IsNullOrEmpty(element.HelpText)
                        &&
                        isStringMatched
                        &&
                        (controlType == null || element.ControlType.Equals(controlType));
                    }).ToList();

                    return allElementsByHelpText;
                }
                // If both a key and a value is passed, parse the HelpText and return all elements where the value of key is equal to value
                if (key != null && value != null)
                {
                    List<FlaUIElement.AutomationElement> allElementsByHelpText = allElementsInWindow.Where(element =>
                        !string.IsNullOrEmpty(element.HelpText)
                        &&
                        (
                            element.HelpText.Contains(key)
                            &&
                            Utils.XAMLHandler.ParseStringToObject(element.HelpText)[key].Equals(value)
                        )
                        &&
                        (controlType == null || element.ControlType.Equals(controlType))
                    ).ToList();

                    return allElementsByHelpText;
                }

                // Make sure all code paths return a value
                return null;
            }

            // Parses a object formatted string and converts it to an object e.g. "x:1, y:5" -> { x: 1, y: 5 }
            // Currenlty supports bool and int data types (falls back to string)
            public static IDictionary<string, object>? ParseStringToObject(string inputString)
            {
                if (string.IsNullOrWhiteSpace(inputString))
                {
                    return null; // Input string cannot be null or empty
                }

                IDictionary<string, object> returnObject = new ExpandoObject() as IDictionary<string, object>;

                List<string> properties = inputString.Split("|")
                    .Select((property) => property.Trim())
                    .ToList();

                foreach (string property in properties)
                {
                    List<string> keyValuePair = property.Split(":")
                        .Select((keyOrValue) => keyOrValue.Trim())
                        .ToList();

                    // If the property is not in the format "key: value" or if there are any empty strings, throw an exception
                    if (keyValuePair.Count != 2 || keyValuePair.Any((element) => string.IsNullOrEmpty(element)))
                    {
                        return null; // String is not formatted as an object
                    }

                    string key = keyValuePair[0];
                    string value = keyValuePair[1];

                    // Determine the type of the value and convert it dynamically
                    if (bool.TryParse(value, out bool boolValue))
                    {
                        returnObject[key] = (bool)boolValue;
                    }
                    // Float works better than int in this case.
                    else if (float.TryParse(value, out float floatValue))
                    {
                        returnObject[key] = (float)floatValue;
                    }
                    else
                    {
                        // Fallback, just store it as a string
                        returnObject[key] = value;
                    }
                }

                return returnObject.ToDictionary();
            }
        }

        public class FileHandler
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
}
