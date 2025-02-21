using System.Collections.Generic;
using SDLTM.Import.Interface;

namespace SDLTM.Import.Service
{
	public class FilesDialogService : IDialogService
	{
		private readonly IOpenFileDialogService _openFileDialogService;
		private readonly ISelectFolderDialogService _folderDialogService;
		private readonly IMessageBoxService _messageBoxService;

		public FilesDialogService(IOpenFileDialogService openFileDialogService,ISelectFolderDialogService folderDialogService,IMessageBoxService messageBoxService)
		{
			_openFileDialogService = openFileDialogService;
			_folderDialogService = folderDialogService;
			_messageBoxService = messageBoxService;
		}

		/// <summary>
		/// Display File dialog window
		/// </summary>
		/// <param name="filter">file extension</param>
		/// <returns>Selecteed files path based on extension</returns>
		public List<string> ShowFileDialog(string filter)
		{
			_openFileDialogService.ShowDialog(filter);
			return _openFileDialogService.FileNames;
		}

		/// <summary>
		/// Display folder dialog window
		/// </summary>
		/// <returns>Selected folder path</returns>
		public string ShowFolderDialog(string dialogTitle)
		{
			_folderDialogService.ShowDialog(dialogTitle);
			return _folderDialogService.FolderPath;
		}

		/// <summary>
		/// Show message box with Yes and No buttons
		/// </summary>
		/// <param name="title">Dialog Title</param>
		/// <param name="message">Dialog message</param>
		/// <returns></returns>
		public MessageDialogResult ShowYesNoDialogResult(string title, string message)
		{
			return _messageBoxService.ShowYesNoMessageBox(title, message);
		}
	}
}
