using System.Collections.Generic;
using System.Windows.Forms;

namespace Sdl.Community.CleanUpTasks.Dialogs
{
	public class FileDialog : IFileDialog
    {
        public IEnumerable<string> GetFile(string lastUsedDirectory)
        {
            IEnumerable<string> files = new List<string>();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "xml files (*.xml)|*.xml";
            ofd.InitialDirectory = lastUsedDirectory;
            ofd.Multiselect = true;

            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                if (ofd.FileNames != null && ofd.FileNames.Length > 0)
                {
                    files = ofd.FileNames;
                }
            }

            return files;
        }

        public string SaveFile(string lastUsedDirectory)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "xml files (*.xml)|*.xml";
            sfd.InitialDirectory = lastUsedDirectory;
            sfd.DefaultExt = "xml";

            DialogResult dr = sfd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                return sfd.FileName;
            }
            else
            {
                return null;
            }
        }
    }
}