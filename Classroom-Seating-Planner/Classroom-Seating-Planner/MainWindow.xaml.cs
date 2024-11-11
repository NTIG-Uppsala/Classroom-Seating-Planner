using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Classroom_Seating_Planner.src;
using ExtensionMethods;

namespace Classroom_Seating_Planner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        // Define the global list of names here
        private List<string> listOfNames;
        private List<TextBlock> listOfSeats;

        public MainWindow()
        {
            InitializeComponent();

            // Adds event listeners to window
            SizeChanged += Window_SizeChanged;
            Loaded += MainWindow_Loaded;

            // Make items in the ClassListElement unselectable
            ClassListElement.PreviewMouseDown += (sender, e) => { e.Handled = true; };
            ClassListElement.SelectionChanged += (sender, e) => { e.Handled = true; };

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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Check if there are any issues with the class list file, if so, display a popup
            // Pass the current window as the parent window so the popups know if it needs to close
            FileHandler.CheckClassListFileForIssues(this);

            // Get the list of student names from the class list file
            listOfNames = FileHandler.GetClassListFromFile();

            // Populate the ListBox with the contents of listOfNames
            foreach (string name in listOfNames)
            {
                ListBoxItem student = new()
                {
                    Content = name,
                    // These properties prevent the ListBoxItem from being selected
                    IsHitTestVisible = false,
                    Focusable = false,
                    IsSelected = false,
                    IsTabStop = false
                };
                ClassListElement.Items.Add(student);
            }
        }

        private void RandomizeSeatingButton_Click(object sender, RoutedEventArgs e)
        {
            // Shuffle the list of student names using a custom class method
            listOfNames.Shuffle();

            // Clear the ListBox before populating
            ClassListElement.Items.Clear();

            // Populate the ListBox with the new order
            foreach (string name in listOfNames)
            {
                ListBoxItem student = new()
                {
                    Content = name,
                    // These properties prevent the ListBoxItem from being selected
                    IsHitTestVisible = false,
                    Focusable = false,
                    IsSelected = false,
                    IsTabStop = false
                };
                ClassListElement.Items.Add(student);
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

        protected void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Use the value of the smallest dimension (width or height) of the
            // window to account for vertical and horizontal aspect ratios
            double smallestDimension = Math.Min(e.NewSize.Height, e.NewSize.Width);

            // Using the y=mx+b function to find a font size
            double m = 0.0135699;
            double x = smallestDimension;
            double b = 8.89353;
            double fontSize = Math.Round(m * x + b);

            RandomizeSeatingButton.FontSize = fontSize;
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the popup window
            _ = new PopupWindow(PopupWindow.fileTutorialMessage, "Hjälp", this);
        }
    }
}