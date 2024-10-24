﻿using System.Diagnostics;
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

            // Populate the ListBox with the contents of listOfNames
            foreach (string name in listOfNames)
            {
                StudentList.Items.Add(name);
            }
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

            // List of available seat labels, starting from Seat1, Seat2, etc.
            var seats = new List<TextBlock> { Seat1, Seat2 /* Add more seats like Seat3, Seat4, etc. based on your grid */ };

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
                //var seat = FindName($"Seat{index}");
                seats[index].Text = string.Empty;  // Clear the seat if it's not occupied
            }
        }
    }
}