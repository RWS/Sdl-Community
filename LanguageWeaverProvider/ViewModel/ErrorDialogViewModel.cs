using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Input;
using LanguageWeaverProvider.Command;

namespace LanguageWeaverProvider.ViewModel
{
	public class ErrorDialogViewModel : BaseViewModel
	{
		int _windowHeight;
		string _errorTitle;
		string _errorMessage;
		string _detailedReport;
		bool _displayDetailedReport;
		bool _canDisplayDetailedReport;

		public ErrorDialogViewModel(string title, string message, Exception exception = null)
		{
			WindowHeight = 200;
			ErrorTitle = title;
			ErrorMessage = message;
			DetailedReport = exception?.StackTrace;
			CanDisplayDetailedReport = exception is not null;
			InitializeCommands();
		}

		public int WindowHeight
		{
			get => _windowHeight;
			set
			{
				_windowHeight = value;
				OnPropertyChanged();
			}
		}

		public string ErrorTitle
		{
			get => _errorTitle;
			set
			{
				_errorTitle = value;
				OnPropertyChanged();
			}
		}

		public string ErrorMessage
		{
			get => _errorMessage;
			set
			{
				_errorMessage = value;
				OnPropertyChanged();
			}
		}

		public string DetailedReport
		{
			get => _detailedReport;
			set
			{
				_detailedReport = value;
				OnPropertyChanged();
			}
		}

		public bool DisplayDetailedReport
		{
			get => _displayDetailedReport;
			set
			{
				_displayDetailedReport = value;
				OnPropertyChanged();
			}
		}

		public bool CanDisplayDetailedReport
		{
			get => _canDisplayDetailedReport;
			set
			{
				_canDisplayDetailedReport = value;
				OnPropertyChanged();
			}
		}

		public ICommand SaveReportCommand { get; private set; }

		public ICommand CloseDialogCommand { get; private set; }

		public ICommand DisplayReportCommand { get; private set; }

		public delegate void CloseWindowEventRaiser();

		public event CloseWindowEventRaiser CloseEventRaised;

		private void InitializeCommands()
		{
			SaveReportCommand = new RelayCommand(SaveReport);
			DisplayReportCommand = new RelayCommand(DisplayReport);

			CloseDialogCommand = new RelayCommand((action) => CloseEventRaised?.Invoke());
		}

		private void DisplayReport(object parameter)
		{
			if (DisplayDetailedReport)
			{
				DisplayDetailedReport = false;
				WindowHeight = 200;
				return;
			}

			WindowHeight = 500;
			DisplayDetailedReport = true;
		}

		private void SaveReport(object parameter)
		{
			const string TradosAppStoreFolder = "Trados AppStore";

			var directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), TradosAppStoreFolder, Constants.PluginName);
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}

			var fileName = $"{DateTime.Now:ddMMyyHHmmss}_{ErrorTitle}.txt";
			var filePath = Path.Combine(directoryPath, fileName);
			var content = $"{ErrorMessage}\n\n\n{DetailedReport}";
			File.WriteAllText(filePath, content);
			OpenFolderAndSelectFile(filePath);
		}

		private void OpenFolderAndSelectFile(string filePath)
		{

			var pidl = ILCreateFromPathW(filePath);
			SHOpenFolderAndSelectItems(pidl, 0, IntPtr.Zero, 0);
			ILFree(pidl);
		}

		[DllImport("shell32.dll", CharSet = CharSet.Unicode)]
		private static extern IntPtr ILCreateFromPathW(string pszPath);

		[DllImport("shell32.dll")]
		private static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, int cild, IntPtr apidl, int dwFlags);

		[DllImport("shell32.dll")]
		private static extern void ILFree(IntPtr pidl);
	}
}