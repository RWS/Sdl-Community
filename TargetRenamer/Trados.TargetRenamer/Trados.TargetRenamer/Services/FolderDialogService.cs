using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trados.TargetRenamer.Interfaces;
using DialogResult = System.Windows.Forms.DialogResult;

namespace Trados.TargetRenamer.Services
{
	
	public class FolderDialogService : IFolderDialogService
	{
		private readonly FolderBrowserDialog _folderDialog;

		public FolderDialogService()
		{
			_folderDialog = new FolderBrowserDialog();
		}

		public string ShowDialog()
		{
			if (_folderDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(_folderDialog.SelectedPath))
			{
				return _folderDialog.SelectedPath;
			}
			else
			{
				return null;
			}
		}
	}
}
