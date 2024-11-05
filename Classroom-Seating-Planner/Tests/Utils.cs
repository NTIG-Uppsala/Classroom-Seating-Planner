using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private static List<string> fileBackupList = new List<string>();
        public static (FlaUI.Core.Application, Window, ConditionFactory) SetUpTest(List<string>? testNamesList = null)
        {
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

            string namesListFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\Bordsplaceringsgeneratorn\\klasslista.txt";

            List<string> namesListFileBackupList;
            using (StreamReader reader = new(namesListFile))
            {
                namesListFileBackupList = reader
                    .ReadToEnd()
                    .Split("\n")
                    .Select(name => name.Trim())
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList();
            }

            fileBackupList = namesListFileBackupList;

            using (StreamWriter writer = new(namesListFile, false))
            {
                foreach (string testName in testNamesList)
                {
                    writer.WriteLine(testName);
                }
            }

            // DEBUG
            Trace.WriteLine("\n------------------\nBackup list content\n------------------\n");
            int i = 0;
            foreach (string name in namesListFileBackupList)
            {
                i++;
                Trace.WriteLine($"{i}: {name}");
            }

            Trace.WriteLine("\n------------------\nTest list content\n------------------\n");
            i = 0;
            foreach (string testName in testNamesList)
            {
                i++;
                Trace.WriteLine($"{i}: {testName}");
            }

            return InitializeApplication();
        }

        public static void TearDownTest(FlaUI.Core.Application app)
        {
            // DEBUG
            Trace.WriteLine("\n------------------\nBackup list before restoration\n------------------\n");
            int i = 0;
            // TODO - Restore the file to match the content of the backup
            foreach (string @string in fileBackupList)
            {
                i++;
                Trace.WriteLine($"{i}: {@string}");
            }

            string namesListFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\Bordsplaceringsgeneratorn\\klasslista.txt";

            List<string> namesFileNamesList;
            using (StreamReader reader = new(namesListFile))
            {
                namesFileNamesList = reader
                    .ReadToEnd()
                    .Split("\n")
                    .Select(name => name.Trim())
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList();
            }

            List<string> namesFile

            // DEBUG
            Trace.WriteLine("\n------------------\nFile content before restoration\n------------------\n");
            i = 0;
            foreach (string name in namesFileNamesList)
            {
                i++;
                Trace.WriteLine($"{i}: {name}");
            }

            using (StreamWriter writer = new(namesListFile, false))
            {
                foreach (string @string in fileBackupList)
                {
                    writer.WriteLine(@string);
                }
            }

            // DEBUG
            using (StreamReader reader = new(namesListFile))
            {
                namesFileNamesList = reader
                    .ReadToEnd()
                    .Split("\n")
                    .Select(name => name.Trim())
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList();
            }

            // DEBUG
            Trace.WriteLine("\n------------------\nRestored file content\n------------------\n");
            i = 0;
            foreach (string name in namesFileNamesList)
            {
                i++;
                Trace.WriteLine($"{i}: {name}");
            }

            app.Close();
        }
    }
}
