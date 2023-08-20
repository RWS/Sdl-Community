using Microsoft.Win32;
using Sdl.Community.DeepLMTProvider.Interface;

namespace Sdl.Community.DeepLMTProvider.Helpers
{
    public class OpenFileDialogWrapper : IOpenFileDialog
    {
        public bool ShowDialog()
        {
            var importDialog = new OpenFileDialog
            {
                Filter = "TSV files (*.tsv)|*.tsv|All files (*.*)|*.*",
                Title = "Select TSV files",
                Multiselect = true
            };

            if (importDialog.ShowDialog() != true) return false;

            FileNames = importDialog.FileNames;
            return true;
        }

        public string[] FileNames { get; set; }
    }
}