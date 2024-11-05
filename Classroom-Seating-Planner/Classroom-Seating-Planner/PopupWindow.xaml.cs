using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Classroom_Seating_Planner
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : Window
    {
        public PopupWindow(string popupText, string windowTitle, Window parent)
        {
            InitializeComponent();

            popupWindowName.Title = windowTitle;

            TextBody.Text = popupText;

            Owner = parent;

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
            string documentsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            Process.Start("explorer.exe", Path.Combine(documentsFolder, "Bordsplaceringsgeneratorn"));
        }
    }
}
