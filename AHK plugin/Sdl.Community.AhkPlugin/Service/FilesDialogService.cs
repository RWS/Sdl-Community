using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Sdl.Community.AhkPlugin.Interface;

namespace Sdl.Community.AhkPlugin.Service
{
	public class FilesDialogService : IDialogService
	{
		private readonly OpenFileDialog _fileDialog;

		public FilesDialogService()
		{
			_fileDialog = new OpenFileDialog {Multiselect = true};
		}
		public List<string> FileNames { get; set; }

		/// <summary>
		/// File Dialog window
		/// </summary>
		/// <param name="filter">Files extensions</param>
		public List<string> ShowDialog(string filter)
		{
			_fileDialog.Filter = filter;

			if (_fileDialog.ShowDialog() != true)
			{
				FileNames = new List<string>();
			}
			return _fileDialog.FileNames.ToList();
		}
	}
}

