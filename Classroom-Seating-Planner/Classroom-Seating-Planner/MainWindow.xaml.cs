using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Classroom_Seating_Planner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Define the global list of names here
        private List<string> listOfNames;
        
        public MainWindow()
        {
            InitializeComponent();

            // Initialize the list with the placeholder names
            List<string> list =
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
            listOfNames = list;

            foreach (string name in listOfNames)
            {
                ListBoxStudentList.Items.Add(name);
            }
        }

        private void ButtonRandomizeSeating_Click(object sender, RoutedEventArgs e)
        {
            static List<string> shuffle(List<string> list, Random rng)
            {
                List<string> newList = [.. list.OrderBy(item => rng.Next())];
                return newList;
            }
            Random rng = new();

            // THIS IS A DEBUG CODE BLOCK THAT SHOULD BE REMOVED
            Trace.Write("\nList before shuffle: ");
            foreach (string name in listOfNames)
            {
                Trace.Write($"{name}, ");
            }

            listOfNames = shuffle(listOfNames, rng);

            ListBoxStudentList.Items.Clear();
            // THIS IS PARTIALLY A DEBUG CODE BLOCK THAT SHOULD BE REMOVED
            Trace.Write("\nList after shuffle: ");
            foreach (string name in listOfNames)
            {
                Trace.Write($"{name}, ");
                ListBoxStudentList.Items.Add(name);
            }
            Trace.Write("\n");
        }
    }
}