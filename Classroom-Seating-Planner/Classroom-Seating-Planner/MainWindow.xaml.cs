using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Classroom_Seating_Planner.src;

namespace Classroom_Seating_Planner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Define the global list of names here
        private List<string> listOfNames;
        private List<TextBlock> listOfSeats;

        public MainWindow()
        {
            InitializeComponent();

            this.SizeChanged += WindowSize_Changed;

            // Initialize the list with the placeholder names
            List<string> namesList =
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
            listOfNames = namesList;

            // Populate the ListBox with the contents of listOfNames
            foreach (string name in listOfNames)
            {
                StudentList.Items.Add(name);
            }

            List<TextBlock> seatsList = [
                Seat1,
                Seat2,
                Seat3,
                Seat4,
                Seat5,
                Seat6,
                Seat7,
                Seat8,
                Seat9,
                Seat10,
                Seat11,
                Seat12,
                Seat13,
                Seat14,
                Seat15,
                Seat16,
                Seat17,
                Seat18,
                Seat19,
                Seat20,
                Seat21,
                Seat22,
                Seat23,
                Seat24,
                Seat25,
                Seat26,
                Seat27,
                Seat28,
                Seat29,
                Seat30,
                Seat31,
                Seat32,
                Seat33,
                Seat34,
                Seat35,
                Seat36
            ];
            listOfSeats = seatsList;
        }

        private void RandomizeSeatingButton_Click(object sender, RoutedEventArgs e)
        {
            // Shuffle the list of student names using a custom class method
            listOfNames = ListActions.Shuffle(listOfNames);

            // Clear the ListBox before populating
            StudentList.Items.Clear();

            // Populate the ListBox with the new order
            foreach (string name in listOfNames)
            {
                StudentList.Items.Add(name);
            }

            List<TextBlock> seats = listOfSeats;

            // Ensure we don't exceed the number of available seats
            int seatCount = Math.Min(listOfNames.Count, seats.Count);

            // Update the tables with the new order
            for (int index = 0; index < seatCount; index++)
            {
                // Assign the shuffled student name to the corresponding seat
                seats[index].Text = listOfNames[index];
            }

            // If there are more seats than students, clear the remaining seats
            for (int index = seatCount; index < seats.Count; index++)
            {
                // Clear the seat if it's not occupied
                seats[index].Text = string.Empty;
            }
        }

        protected void WindowSize_Changed(object sender, SizeChangedEventArgs e)
        {
            double newWindowHeight = e.NewSize.Height;
            //double newWindowWidth = e.NewSize.Width;
            //double prevWindowHeight = e.PreviousSize.Height;
            //double prevWindowWidth = e.PreviousSize.Width;

            // Use smallest between window height/width
            double directionalCoefficient = 0.0135699;
            double yIntercept = 8.89353;
            double newFontSize = Math.Round(newWindowHeight * directionalCoefficient + yIntercept);
            RandomizeSeatingButton.FontSize = newFontSize;
            StudentListTitle.FontSize = newFontSize;
        }
    }
}