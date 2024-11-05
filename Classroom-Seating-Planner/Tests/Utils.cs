﻿using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using FlaUIElement = FlaUI.Core.AutomationElements;

namespace Tests
{
    internal class Utils
    {
        public static void ClickRandomizeSeatingButton(FlaUIElement.Window window, ConditionFactory cf)
        {
            // Find and press the randomizer button
            FlaUIElement.AutomationElement randomizeButton = window.FindFirstDescendant(cf.ByAutomationId("RandomizeSeatingButton")).AsButton();
            randomizeButton.Click();
        }

        public static List<FlaUIElement.AutomationElement> GetAllByAutomationId(Window window, ConditionFactory cf, string identifier, FlaUI.Core.Definitions.ControlType? controlType = null)
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

        private static List<string> fileBackupList = [];
        public static (FlaUI.Core.Application, Window, ConditionFactory) SetUpTest(List<string>? testNamesList = null)
        {
            // Backup the data from the names list file so it can be restored after testing
            System.IO.File.Copy(UtilsHelpers.studentNamesListFilePath, $"{UtilsHelpers.studentNamesListFilePath}.bak");

            // Default list of names used for tests
            testNamesList ??=
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

            // Insert the test data into the file
            string namesListFile = UtilsHelpers.studentNamesListFilePath;
            using (StreamWriter writer = new(namesListFile, false))
            {
                foreach (string testName in testNamesList)
                {
                    writer.WriteLine(testName);
                }
            }

            // Return values necessary for running the test
            return UtilsHelpers.InitializeApplication();
        }

        public static void TearDownTest(FlaUI.Core.Application app)
        {
            // Restore the names list file by filling it with backed up information from before the test
            System.IO.File.Delete(UtilsHelpers.studentNamesListFilePath);
            System.IO.File.Move($"{UtilsHelpers.studentNamesListFilePath}.bak", UtilsHelpers.studentNamesListFilePath);

            // Terminate the app
            app.Close();
        }

        // Returns a shuffled list from the list that is passed
        public static List<string> ShuffleList(List<string> list)
        {
            // Can't shuffle 0 or 1 elements
            if (list.Count < 2) return list;

            Random rng = new();
            List<string> newList = list;

            // Shuffle until the list has a new order
            while (newList.SequenceEqual(list))
            {
                newList = [.. list.OrderBy(item => rng.Next())];
            }
            return newList;
        }

        // Returns array of ListBox Items
        public static string[] GetListBoxItemsAsArray(FlaUIElement.Window window, ConditionFactory cf, string listBoxAutomaitonId)
        {
            FlaUIElement.ListBox listBox = window.FindFirstDescendant(cf.ByAutomationId(listBoxAutomaitonId)).AsListBox();
            ListBoxItem[] listBoxItemsList = listBox.Items;
            string[] listItemsArray = listBoxItemsList.Select(listItem => listItem.Text).ToArray();

            return listItemsArray;
        }
    }

    internal class UtilsHelpers
    {
        // Global variables for file paths
        public static readonly string dataFolderName = "Bordsplaceringsgeneratorn";
        public static readonly string studentNamesListFileName = "klasslista.txt";

        public static readonly string dataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), dataFolderName);
        public static readonly string studentNamesListFilePath = Path.Combine(dataFolderPath, studentNamesListFileName);

        // Returns a list of data from an external file
        public static List<string> GetDataFromFile(string filePath)
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
        public static List<string> GetStudentNamesFromFile()
        {
            string filePath = studentNamesListFilePath;

            // Read the names from the file and return them as a list
            List<string> studentNamesList = GetDataFromFile(filePath);
            return studentNamesList;
        }

        public static (FlaUI.Core.Application, Window, ConditionFactory) InitializeApplication()
        {
            // Find and run the application
            FlaUI.Core.Application app = FlaUI.Core.Application.Launch("..\\..\\..\\..\\Classroom-Seating-Planner\\bin\\Debug\\net8.0-windows\\win-x64\\Classroom-Seating-Planner.exe");
            using FlaUI.UIA3.UIA3Automation automation = new();

            // Find the main window for the purpose of finding elements
            Window window = app.GetMainWindow(automation);
            ConditionFactory cf = new(new UIA3PropertyLibrary());

            return (app, window, cf);
        }
    }
}
