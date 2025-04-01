using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Windows.Forms.DialogResult;
using Button = System.Windows.Controls.Button;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Controls
{
    /// <summary>
    /// Interaction logic for ReportFolderExplorer.xaml
    /// </summary>
    public partial class ReportFolderExplorer
    {
        public static readonly DependencyProperty ReportFoldersProperty =
            DependencyProperty.Register(nameof(ReportFolders), typeof(ObservableCollection<string>), typeof(ReportFolderExplorer),
                new PropertyMetadata(default(ObservableCollection<string>)));

        public ReportFolderExplorer() => InitializeComponent();

        public ObservableCollection<string> ReportFolders
        {
            get => (ObservableCollection<string>)GetValue(ReportFoldersProperty);
            set => SetValue(ReportFoldersProperty, value);
        }

        public new bool ShowDialog() => base.ShowDialog() ?? false;

        private void AddNewReportFolder()
        {
            using var dialog = new OpenFileDialog();

            dialog.CheckFileExists = false;
            dialog.CheckPathExists = true;
            dialog.ValidateNames = false; // Allows selecting folders
            dialog.FileName = "Select this folder"; // Dummy filename to allow folder selection

            if (dialog.ShowDialog() != OK) return;

            var reportFolder = Path.GetDirectoryName(dialog.FileName);
            if (ReportFolders.Contains(reportFolder)) return;

            ReportFolders.Add(reportFolder);
        }

        private void AddNewReportFolderButton_Clicked(object sender, RoutedEventArgs e) => AddNewReportFolder();

        private void CloseWindow_Click(object sender, RoutedEventArgs e) => Close();

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void RemoveFolderButton_Click(object sender, RoutedEventArgs e)
        {
                //(FolderListGrid.ItemsSource as ObservableCollection<string>)?.Remove(filePath);
            if (sender is Button { DataContext: string filePath }) ReportFolders.Remove(filePath);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}