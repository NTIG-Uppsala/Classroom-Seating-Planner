using System.Diagnostics;
using System.IO;
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

            // Adds event listener to window
            SizeChanged += Window_SizeChanged;

            // Initialize the list of names from the file
            listOfNames = GetStudentNamesFromFile();

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

        private static List<string> GetStudentNamesFromFile()
        {
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string directoryPath = Path.Combine(documentsFolder, "Bordsplaceringsgeneratorn");
            string filePath = Path.Combine(directoryPath, "klasslista.txt");

            using (StreamReader reader = new(filePath))
            {
                List<string> names = reader.ReadToEnd()
                                           .Split('\n')
                                           .Select(name => name.Trim())
                                           .ToList();
                return names;
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
    }
}