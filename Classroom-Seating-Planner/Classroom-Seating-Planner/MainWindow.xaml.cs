using Classroom_Seating_Planner.Src;
using Extension_Methods;
using System.Diagnostics;
using System.Windows;

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
            this.SizeChanged += Window_SizeChanged;
            this.Loaded += MainWindow_Loaded;

            // Make items in the ClassListElement unselectable
            ClassListElement.PreviewMouseDown += (sender, e) => { e.Handled = true; };
            ClassListElement.SelectionChanged += (sender, e) => { e.Handled = true; };

            // Give a reference to the grid where the tables and whiteboard will be placed to its handler
            this.classroomLayoutManager = new(ClassroomLayoutGridElement);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Handle any possible file issues
            // Pass the current window as the parent window so the popups know if it needs to close
            Src.FileHandler.HandleAllDataFileIssues(this);


            // Read the data files
            List<Cells.Cell> classroomElements = Src.FileHandler.GetClassroomElementsFromLayout();
            List<ConstraintsHandler.Student> students = Src.FileHandler.GetClassListFromFile();


            // Populate the list of names to the left with the names of the students
            Src.ClassListElementHandler.Populate(ClassListElement, students);
            // Draw all the tables and whiteboards
            this.classroomLayoutManager.Render(classroomElements);

            // Dynamic sizing
            Src.DynamicClassroomSizeHandler.UpdateClassroomLayoutSize(new(this.ActualWidth, this.ActualHeight), this);
        }

        private void RandomizeSeatingButton_Click(object sender, RoutedEventArgs _)
        {
            // Read the data files
            List<Cells.Cell> classroomElements = Src.FileHandler.GetClassroomElementsFromLayout();
            List<ConstraintsHandler.Student> students = Src.FileHandler.GetClassListFromFile();

            // This is the main algoritm that decides where students are seated
            Src.SeatingHandler.GenerateSeatingArrangement(students, classroomElements);

            this.classroomLayoutManager.Render(classroomElements);

            // Dynamic sizing
            Src.DynamicClassroomSizeHandler.UpdateClassroomLayoutSize(new(this.ActualWidth, this.ActualHeight), this);
        }

        private void HelpButton_Click(object sender, RoutedEventArgs _)
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

            // Dynamic sizing of the classroom elements
            Src.DynamicClassroomSizeHandler.UpdateClassroomLayoutSize(new(this.ActualWidth, this.ActualHeight), this);
        }
    }
}