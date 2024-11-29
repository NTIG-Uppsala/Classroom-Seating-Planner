using Classroom_Seating_Planner.Src;
using Extension_Methods;
using System.Diagnostics;
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
        private List<string> classListFromFile = [];
        public Src.ClassroomLayoutManager classroomLayoutManager;

        public MainWindow()
        {
            InitializeComponent();
            // Adds event listeners to window
            SizeChanged += Window_SizeChanged;
            Loaded += MainWindow_Loaded;

            // Make items in the ClassListElement unselectable
            ClassListElement.PreviewMouseDown += (sender, e) => { e.Handled = true; };
            ClassListElement.SelectionChanged += (sender, e) => { e.Handled = true; };

            // Give a reference to the grid where the tables and whiteboard will be placed to its handler
            classroomLayoutManager = new(ClassroomElement);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Handle any possible file issues
            // Pass the current window as the parent window so the popups know if it needs to close
            Src.FileHandler.HandleAllDataFileIssues(this);

            // Get the list of student names from the class list file
            this.classListFromFile = Src.FileHandler.GetClassListFromFile();

            // Populate the ListBox with the content of listOfNames
            Src.ClassListElementHandler.Populate(ClassListElement, this.classListFromFile);

            // The argument retrieves data about the classroom layout for the manager to save and use
            classroomLayoutManager.Initialize(Src.FileHandler.GetClassroomLayoutDataFromFile());
            classroomLayoutManager.DrawChildren();

            System.Windows.Size windowSize = new(this.Width, this.Height);
            Src.ClassroomLayoutHandler.UpdateClassroomLayoutSize(windowSize, this);
        }

        private void RandomizeSeatingButton_Click(object sender, RoutedEventArgs e)
        {
            // Shuffle the list of student names using a custom class method
            this.classListFromFile.Shuffle();

            // Populate the class list and the seats with the new order
            Src.ClassListElementHandler.Populate(ClassListElement, this.classListFromFile);

            // Populate the tables with the randomised order of the class list
            Src.SeatingHandler.Populate(classroomLayoutManager.tableElements, this.classListFromFile);
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the popup window
            new PopupWindow(PopupWindow.helpWindowMessage, "Hjälp", this);
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

            // Refresh the size of the classroom layout
            Src.ClassroomLayoutHandler.UpdateClassroomLayoutSize(e.NewSize, this);
        }
    }
}