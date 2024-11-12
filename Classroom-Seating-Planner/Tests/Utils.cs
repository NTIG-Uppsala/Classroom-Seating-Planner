﻿using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using System.Diagnostics;
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
        public static (FlaUI.Core.Application, FlaUI.UIA3.UIA3Automation, FlaUIElement.Window, FlaUI.Core.Conditions.ConditionFactory) SetUp(List<string>? testClassList = null)
        {
            // Use default testing class list unless a list is specified
            testClassList ??= testingClassList;

            // TODO - restore backup folder
            // Restore backup data if backup file already exists
            if (System.IO.File.Exists($"{Utils.FileManager.classListFilePath}.bak"))
            {
                Utils.FileManager.RestoreBackupData(Utils.FileManager.classListFilePath);
            }

            // Create the data folder and an empty class list file if they don't exist
            if (!System.IO.File.Exists(Utils.FileManager.classListFilePath))
            {
                System.IO.Directory.CreateDirectory(Utils.FileManager.dataFolderPath);
                System.IO.File.Create(Utils.FileManager.classListFilePath).Close();
            }

            // Backup the data from the class list file so it can be restored after testing
            System.IO.File.Copy(Utils.FileManager.classListFilePath, $"{Utils.FileManager.classListFilePath}.bak");

            // Insert the test data into the file
            using (System.IO.StreamWriter writer = new(Utils.FileManager.classListFilePath, false))
            {
                foreach (string testStudent in testClassList)
                {
                    writer.WriteLine(testStudent);
                }
            }

            // Return values necessary for running the test
            return InitializeFlaUIApp();
        }

        // TearDown method
        public static void TearDown(FlaUI.Core.Application app)
        {
            // Restore the class list file by filling it with backed up information from before the test
            Utils.FileManager.RestoreBackupData(Utils.FileManager.classListFilePath);

            // Terminate the app
            app.Close();
        }

        public static (FlaUI.Core.Application, FlaUI.UIA3.UIA3Automation, FlaUIElement.Window, FlaUI.Core.Conditions.ConditionFactory) InitializeFlaUIApp()
        {
            // Find and run the application
            //FlaUI.Core.Application app = FlaUI.Core.Application.Launch("C:\\Users\\viggo.strom\\Documents\\GitHub\\Classroom-Seating-Planner\\Classroom-Seating-Planner\\Classroom-Seating-Planner\\bin\\Debug\\net8.0-windows\\win-x64\\Classroom-Seating-Planner.exe");
            //Console.WriteLine(app.ProcessId);
            FlaUI.Core.Application app = FlaUI.Core.Application.Launch("..\\..\\..\\..\\Classroom-Seating-Planner\\bin\\Debug\\net8.0-windows\\win-x64\\Classroom-Seating-Planner.exe");
            FlaUI.UIA3.UIA3Automation automation = new();

            // Find the main window for the purpose of finding elements
            FlaUIElement.Window? window = app.GetMainWindow(automation);
            FlaUI.Core.Conditions.ConditionFactory cf = new(new UIA3PropertyLibrary());

            return (app, automation, window, cf);
        }

        public class XAMLManager
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

        public class FileManager
        {
            // Path variables constructed with folder name and file name defined at the top of Utils
            public static readonly string dataFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), dataFolderName);
            public static readonly string classListFilePath = System.IO.Path.Combine(dataFolderPath, classListFileName);
            public static readonly string classListBackupFilePath = $"{System.IO.Path.Combine(dataFolderPath, classListFileName)}.bak";

            // Returns a list of data from an external file
            public static List<string> GetDataFromFile(string filePath)
            {
                // Read the data from the file and return it as a list
                using System.IO.StreamReader reader = new(filePath);
                List<string> dataList = reader
                    .ReadToEnd()
                    .Split("\n")
                    .Select(item => item.Trim())
                    .Where(item => !string.IsNullOrEmpty(item))
                    .ToList();
                return dataList;
            }

            public static void RestoreBackupData(string originalFilePath)
            {
                if (System.IO.File.Exists($"{classListFilePath}.bak"))
                {
                    System.IO.File.Delete(originalFilePath);
                    System.IO.File.Move($"{originalFilePath}.bak", originalFilePath);
                }
            }

            // Returns the list of students read from an external file as a list
            public static List<string> GetClassListFromFile()
            {
                string filePath = classListFilePath;

                // Read the names from the file and return them as a list
                List<string> classList = GetDataFromFile(filePath);
                return classList;
            }
        }
    }
}
