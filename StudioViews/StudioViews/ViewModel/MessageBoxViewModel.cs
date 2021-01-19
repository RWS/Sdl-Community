using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.StudioViews.Commands;
using Sdl.Community.StudioViews.Model;

namespace Sdl.Community.StudioViews.ViewModel
{
	public class MessageBoxViewModel : BaseModel
	{
		private readonly MessageInfo _messageInfo;
		private readonly Window _window;
		private ICommand _okClickCommand;
		private ICommand _viewReportCommand;
		private ICommand _openFolderInExplorerCommand;

		public MessageBoxViewModel(Window window, MessageInfo messageInfo)
		{
			_window = window;
			_messageInfo = messageInfo;

			WindowResult = DialogResult.None;
		}

		public ICommand OpenFolderInExplorerCommand => _openFolderInExplorerCommand ?? (_openFolderInExplorerCommand = new CommandHandler(OpenFolderInExplorer));

		public ICommand OkClickCommand => _okClickCommand ?? (_okClickCommand = new CommandHandler(OKClick));

		public ICommand ViewReportCommand => _viewReportCommand ?? (_viewReportCommand = new CommandHandler(ViewReport));

		public DialogResult WindowResult { get; set; }

		public bool ShowImage => _messageInfo.ShowImage;

		public string ImageUrl => _messageInfo.ImageUrl;

		public string Title => _messageInfo.Title;

		public string Message => _messageInfo.Message;

		public bool ShowViewReport => !string.IsNullOrEmpty(_messageInfo.LogFilePath) && File.Exists(_messageInfo.LogFilePath);

		private void ViewReport(object parameter)
		{
			if (File.Exists(_messageInfo.LogFilePath))
			{
				Process.Start(_messageInfo.LogFilePath);
			}
		}

		private void OpenFolderInExplorer(object parameter)
		{
			if (Directory.Exists(_messageInfo.Folder))
			{
				Process.Start(_messageInfo.Folder);
			}
		}

		private void OKClick(object parameter)
		{
			WindowResult = DialogResult.OK;
			_window.Close();
		}
	}
}
