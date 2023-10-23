using System;
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

		}
	}
}