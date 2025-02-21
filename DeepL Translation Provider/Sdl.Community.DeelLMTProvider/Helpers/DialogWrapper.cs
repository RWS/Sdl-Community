using Sdl.Community.DeepLMTProvider.Interface;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Sdl.Community.DeepLMTProvider.Helpers
{
    public class DialogWrapper : IBrowseDialog
    {
        public string[] FileNames { get; set; }

        private string LastPath { get; } = @"C:\";

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
            var saveFileDialog = new FolderSelectDialog { InitialDirectory = LastPath };
            return saveFileDialog.ShowDialog()
                ? saveFileDialog.FileName
                : null;
        }
    }
}