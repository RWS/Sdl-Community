using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sdl.Community.NumberVerifier.Services
{
    public class FilePathDialogService
    {
        public void GetSaveLocationInputFromUser(out string saveLocation, string title = "", string filter = "", string fileName = "")
        {
            var dlg = new SaveFileDialog
            {
                Title = title,
                Filter = filter,
                FileName = fileName,
                AddExtension = false,
                RestoreDirectory = true
            };

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                saveLocation = null;
                return;
            }

            saveLocation = dlg.FileName;
            if (File.Exists(saveLocation))
            {
                try
                {
                    File.Delete(saveLocation);
                }
                catch
                {
                    saveLocation = CreateNewFile(saveLocation);
                    throw;
                }
            }
        }

        public string[] GetFilePathInputFromUser(string title = "", string initialDirectory = "", string filter = "", bool multiselect = false)
        {
            var dlg = new OpenFileDialog
            {
                Title = title,
                Filter = filter,
                InitialDirectory = GetInitialPath(initialDirectory),
                Multiselect = multiselect,
                RestoreDirectory = true,
            };


            return dlg.ShowDialog() == DialogResult.OK ? dlg.FileNames : null;
        }

        private string CreateNewFile(string filePath)
        {
            var index = 0;
            while (true)
            {
                if (index == 0)
                {
                    filePath = filePath.Insert(filePath.IndexOf(".xlsx", StringComparison.OrdinalIgnoreCase),
                        $"_{index}");
                }
                else
                {
                    filePath = filePath.Replace((index - 1).ToString(), index.ToString());
                }

                if (File.Exists(filePath))
                {
                    index++;
                    continue;
                }

                break;
            }

            return filePath;
        }

        private string GetInitialPath(string initialDirectory)
        {
            return string.IsNullOrWhiteSpace(initialDirectory)
                ? ""
                : initialDirectory.Contains(".")
                    ? initialDirectory.Substring(0, initialDirectory.LastIndexOf('\\') + 1)
                    : initialDirectory;
        }
    }
}
