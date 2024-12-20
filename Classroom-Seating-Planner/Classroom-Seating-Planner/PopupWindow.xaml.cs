using System.Diagnostics;
using System.Windows;

namespace Classroom_Seating_Planner
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : System.Windows.Window
    {
        // This is the instructional text that will be displayed in the popup windows
        public static readonly string classListFileTutorialMessage = $"Klasslistan ligger i " + 
            $"{System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Split("\\").Last(), Src.FileHandler.dataFolderName)}.\n" +
            $"Varje rad i listan är ett namn. När du trycker på 'Slumpa'-knappen så läser programmet in det du har ändrat.";
        public static readonly string classroomLayoutFileTutorialMessage = $"Bordskartan ligger i " + 
            $"{System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Split("\\").Last(), Src.FileHandler.dataFolderName)}.\n" +
            $"Varje tecken i filen är en del av klassrummet. B representerar bord/platser och T representerar en del av tavlan. " +
            $"När du trycker på 'Slumpa'-knappen så läser programmet in det du har ändrat.";

        public static readonly string helpWindowMessage = $"Klasslistan och bordskartan ligger i " + 
            $"{System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Split("\\").Last(), Src.FileHandler.dataFolderName)}.\n" +
            $"I klasslistan är varje rad ett namn.\n" +
            $"I bordskartan är varje tecken en del av klassrummet. B representerar bord/platser och T representerar en del av tavlan.\n" +
            $"När du trycker på 'Slumpa'-knappen så läser programmet in det du har ändrat.";

        public PopupWindow(string popupText, string windowTitle, System.Windows.Window parent)
        {
            InitializeComponent();

            popupWindowName.Title = windowTitle;

            TextBody.Text = popupText;

            this.Show();

            // Close if main window is closed
            parent.Closed += (sender, e) => this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", Src.FileHandler.dataFolderPath);
        }

        public static void FileIssuePopup(string dataFileIssue, Window parent)
        {
            if (dataFileIssue.Equals("notAllFilesWereFound"))
            {
                new PopupWindow($"Alla filer hittades inte. {PopupWindow.helpWindowMessage}", "Information", parent);
                return;
            }

            if (dataFileIssue.Equals("emptyClassList"))
            {
                new PopupWindow($"Klasslistan är tom. En standardklasslista har skapats. {PopupWindow.classListFileTutorialMessage}", "Varning", parent);
                return;
            }

            if (dataFileIssue.Equals("defaultClassList"))
            {
                new PopupWindow($"Det verkar som att klasslistan inte har uppdaterats. {PopupWindow.classListFileTutorialMessage}", "Varning", parent);
                return;
            }

            if (dataFileIssue.Equals("noTablesInLayout"))
            {
                new PopupWindow($"Det finns inga bord i bordskartan. {PopupWindow.classroomLayoutFileTutorialMessage}", "Varning", parent);
                return;
            }

            if (dataFileIssue.Equals("noWhiteboardsInLayout"))
            {
                new PopupWindow($"Det finns ingen tavla i bordskartan. {PopupWindow.classroomLayoutFileTutorialMessage}", "Varning", parent);
                return;
            }

            if (dataFileIssue.Equals("moreStudentsThanTables"))
            {
                new PopupWindow($"Det finns fler elever än bord. {PopupWindow.classroomLayoutFileTutorialMessage}", "Varning", parent);
                return;
            }

            if (dataFileIssue.Equals("emptyClassroomLayout"))
            {
                new PopupWindow($"Bordskartan är tom. En standardbordskarta har skapats. {PopupWindow.classroomLayoutFileTutorialMessage}", "Varning", parent);
                return;
            }
        }
    }
}
