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

        // Class list used for tests unless another list is specified
        public static readonly List<string> testingClassList =
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
            SetUp(List<string>? testingClassList = null, List<string>? testingClassroomLayout = null, bool ignoreClassListFileBackup = false, bool ignoreClassroomLayoutFileBackup = false, bool ignoreTestingClassList = false, bool ignoreTestingClassroomLayout = false, bool createDataBackupFolder = false, bool deleteClassListFile = false, bool deleteClassroomLayoutFile = false, bool deleteDataFolder = false)
        {
            // Use default testing class list and classroom layout unless a list is specified
            testingClassList ??= Utils.testingClassList;
            testingClassroomLayout ??= Utils.testingClassroomLayout;

            // Restore backup folder if it exists
            Utils.FileHandler.RestoreDataFolder();

            // Restore backup data if backup files already exist
            Utils.FileHandler.RestoreAllDataFiles();

            // Create the data folder and the default files if they don't exist
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
                System.IO.File.WriteAllLines(Utils.FileHandler.classListFilePath, testingClassList);
            }

            if (!ignoreTestingClassroomLayout)
            {
                // Insert the test classroom layout into the file
                System.IO.File.WriteAllLines(Utils.FileHandler.classroomLayoutFilePath, testingClassroomLayout);
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

            // Create the data folder and the default files if tests have gone wrong and they don't exist
            Utils.FileHandler.CreateDefaultDataFiles();

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
                FlaUI.Core.Conditions.ConditionFactory cf = new(new FlaUI.UIA3.UIA3PropertyLibrary());

                return (app, automation, window, cf);
            }
        }

        public class XAMLHandler : Tests.XAMLHandler { } // Defined in the Utils folder

        public class PopupHandler : Tests.PopupHandler { } // Defined in the Utils folder

        public class FileHandler : Tests.FileHandler { } // Defined in the Utils folder
    }
}