using Sdl.Community.TMBackup.Helpers;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Sdl.Community.TMBackup
{
	public partial class TMBackupForm : Form
	{
		public TMBackupForm()
		{
			InitializeComponent();
		}

		private void btn_BackupFrom_Click(object sender, EventArgs e)
		{
			var fromFolderDialog = new FolderSelectDialog();

			if (fromFolderDialog.ShowDialog())
			{
				if (fromFolderDialog.Files.Any())
				{
					foreach (var folderName in fromFolderDialog.Files)
					{
						txt_BackupFrom.Text = txt_BackupFrom.Text + folderName + ";";
					}
					txt_BackupFrom.Text.Remove(txt_BackupFrom.Text.Length - 1);
				}
			}
		}

		private void btn_BackupTo_Click(object sender, EventArgs e)
		{
			var toFolderDialog = new FolderSelectDialog();

			if (toFolderDialog.ShowDialog())
			{
				txt_BackupTo.Text = toFolderDialog.FileName;
			}					
		}

		private void btn_Details_Click(object sender, EventArgs e)
		{

		}

		private void btn_Change_Click(object sender, EventArgs e)
		{

		}
	}
}