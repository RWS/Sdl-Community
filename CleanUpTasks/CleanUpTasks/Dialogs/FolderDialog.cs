using System;
using System.Windows.Forms;

namespace Sdl.Community.CleanUpTasks.Dialogs
{
	public class FolderDialog : IFolderDialog
    {
        public string GetFolder(string saveDirectory, string description)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = description;
            fbd.SelectedPath = saveDirectory;
            fbd.RootFolder = Environment.SpecialFolder.Desktop;

            DialogResult dr = fbd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                return fbd.SelectedPath;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}