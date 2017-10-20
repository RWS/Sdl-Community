using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			OpenFileDialog fromDialog = new OpenFileDialog();

			fromDialog.InitialDirectory = "c:\\";
			fromDialog.Filter = "TM files (*.sdltm)|*.sdltm| TB files (*sdltb.*)|*sdltb.*";
			fromDialog.FilterIndex = 2;
			fromDialog.Multiselect = true;
			fromDialog.RestoreDirectory = true;

			if (fromDialog.ShowDialog() == DialogResult.OK && fromDialog.FileNames.Any())
			{
				foreach(var fileName in fromDialog.FileNames)
				{
					txt_BackupFrom.Text = txt_BackupFrom.Text + fileName + ";";
				}
				txt_BackupFrom.Text.Remove(txt_BackupFrom.Text.Length - 1);
			}
		}

		private void btn_BackupTo_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog folderDialog = new FolderBrowserDialog();
			if (folderDialog.ShowDialog() == DialogResult.OK)
			{
				txt_BackupTo.Text = folderDialog.SelectedPath;
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
