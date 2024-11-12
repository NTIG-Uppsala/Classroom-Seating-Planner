using Classroom_Seating_Planner.src;
using ExtensionMethods;
using System.Windows;
using System.Windows.Controls;

namespace Classroom_Seating_Planner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        // Where the student names are stored
        private List<string>? classListFromFile;
        // Seats are the xaml elements where the names will be displayed
        private readonly List<TextBlock> seatElements;

        public MainWindow()
        {
            InitializeComponent();

            // Adds event listeners to window
            SizeChanged += Window_SizeChanged;
            Loaded += MainWindow_Loaded;

            // Make items in the ClassListElement unselectable
            ClassListElement.PreviewMouseDown += (sender, e) => { e.Handled = true; };
            ClassListElement.SelectionChanged += (sender, e) => { e.Handled = true; };

            this.seatElements = [
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
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Check if there are any issues with the class list file, if so, display a popup
            // Pass the current window as the parent window so the popups know if it needs to close
            FileHandler.CheckClassListFileForIssues(this);

            // Get the list of student names from the class list file
            this.classListFromFile = FileHandler.GetClassListFromFile();

            // Populate the ListBox with the contents of listOfNames
            ClassListElementHandler.Populate(ClassListElement, this.classListFromFile);
        }

        private void RandomizeSeatingButton_Click(object sender, RoutedEventArgs e)
        {
            // Shuffle the list of student names using a custom class method
            this.classListFromFile.Shuffle();

            // Populate the class list and the seats with the new order
            ClassListElementHandler.Populate(ClassListElement, this.classListFromFile);
            SeatingHandler.Populate(this.seatElements, this.classListFromFile);
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the popup window
            _ = new PopupWindow(PopupWindow.fileTutorialMessage, "Hjälp", this);
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