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
        public PopupWindow(string popupText)
        {
            InitializeComponent();

            TextBody.Text = popupText;
            this.Show();

            // Close if main window is closed
            Closed += (sender, e) => this.Close();
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
