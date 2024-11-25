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
        public static readonly string fileTutorialMessage = $"Klasslistan ligger i\n" +
            $"{System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Split("\\").Last(), Src.FileHandler.dataFolderName)}.\n" +
            $"Varje rad i listan är ett namn. Efter du har fyllt i den måste du starta om programmet för att se dina ändringar.";

        public static readonly string noFileFoundMessage = "Klasslista hittades inte. En textfil har skapats. ";
        public static readonly string emptyFileMessage = "Klasslistan är tom. En standardklasslista har skapats. ";
        public static readonly string defaultFileMessage = "Det verkar som att klasslistan inte har uppdaterats. ";

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
            Process.Start("explorer.exe", System.IO.Path.Combine(Src.FileHandler.dataFolderPath));
        }

        public static void FileIssuePopup(string classListFileIssue, Window parent)
        {
            if (classListFileIssue == "not found")
            {
                _ = new PopupWindow(PopupWindow.noFileFoundMessage + PopupWindow.fileTutorialMessage, "Information", parent);
                return;
            }
            if (classListFileIssue == "empty")
            {
                _ = new PopupWindow(PopupWindow.emptyFileMessage + PopupWindow.fileTutorialMessage, "Varning", parent);
                return;
            }
            if (classListFileIssue == "default")
            {
                _ = new PopupWindow(PopupWindow.defaultFileMessage + PopupWindow.fileTutorialMessage, "Varning", parent);
                return;
            }
        }
    }
}
