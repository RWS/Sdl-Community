using Microsoft.Win32;
using Sdl.Community.DeepLMTProvider.Interface;
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Sdl.Community.DeepLMTProvider.Helpers
{
    public class DialogWrapper : IBrowseDialog
    {
        public string[] FileNames { get; set; }

        private string LastPath { get; set; } = @"C:\";

        public bool ShowDialog()
        {
            var importDialog = new OpenFileDialog
            {
                Filter = "All Supported Formats (*.tsv;*.csv;*.xlsx)|*.tsv;*.csv;*.xlsx|Tab-Separated Values (*.tsv)|*.tsv|Comma-Separated Values (*.csv)|*.csv",
                Title = "Select files",
                Multiselect = true
            };

            if (importDialog.ShowDialog() != true) return false;

            FileNames = importDialog.FileNames;
            return true;
        }

        public string ShowSaveDialog()
        {
            using var folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = LastPath;

            var result = folderDialog.ShowDialog();
            LastPath = folderDialog.SelectedPath;

            return result == DialogResult.OK ? folderDialog.SelectedPath : null;
        }
    }
}