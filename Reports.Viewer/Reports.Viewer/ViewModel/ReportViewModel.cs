using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Sdl.Community.Reports.Viewer.View;
using Sdl.Reports.Viewer.API.Model;

namespace Sdl.Community.Reports.Viewer.ViewModel
{
	public class ReportViewModel : INotifyPropertyChanged, IDisposable
	{
		private string _windowTitle;
		private readonly BrowserView _browserView;
		private readonly DataView _dataView;
		private readonly DataViewModel _dataViewModel;
		private string _projectLocalFolder;
		private ContentControl _currentView;

		public ReportViewModel(BrowserView browserView,
			DataViewModel dataViewModel, DataView dataView)
		{			
			_browserView = browserView;
			_dataViewModel = dataViewModel;
			_dataView = dataView;
			_dataViewModel.ReportSelectionChanged += DataViewModel_ReportSelectionChanged;

			CurrentView = _dataView;
		}

		public ContentControl CurrentView
		{
			get => _currentView;
			set
			{
				_currentView = value;
				OnPropertyChanged(nameof(CurrentView));
			}
		}

		public string ProjectLocalFolder
		{
			get => _projectLocalFolder;
			set
			{
				if (_projectLocalFolder == value)
				{
					return;
				}

				_projectLocalFolder = value;
				OnPropertyChanged(nameof(ProjectLocalFolder));

				if (_dataViewModel != null)
				{
					_dataViewModel.ProjectLocalFolder = _projectLocalFolder;
				}
			}
		}

		public void Print()
		{
			_browserView.WebBrowser.Print();
		}

		public void ShowPageSetupDialog()
		{
			_browserView.WebBrowser.ShowPageSetupDialog();
		}

		public void ShowPrintPreviewDialog()
		{
			_browserView.WebBrowser.ShowPrintPreviewDialog();
		}

		public void SaveReport()
		{
			_browserView.WebBrowser.ShowSaveAsDialog();
		}

		public void UpdateReport(Report report)
		{
			CurrentView = _browserView;

			WebBrowserNavigateToReport(report);
		}

		private void WebBrowserNavigateToReport(Report report)
		{
			string file = null;
			if (report != null)
			{
				file = Path.Combine(ProjectLocalFolder, report.Path);
				if (!File.Exists(file))
				{
					file = null;
				}
			}

			_browserView.WebBrowser.Navigate(file != null ? "file://" + file : null);
		}

		public void UpdateData(List<Report> reports)
		{
			CurrentView = _dataView;
			_dataViewModel.Reports = new ObservableCollection<Report>(reports);			
		}

		public string WindowTitle
		{
			get => _windowTitle;
			set
			{
				_windowTitle = value;
				OnPropertyChanged(nameof(WindowTitle));
			}
		}

		private void DataViewModel_ReportSelectionChanged(object sender, CustomEventArgs.ReportSelectionChangedEventArgs e)
		{
			if (CurrentView is DataView)
			{
				WebBrowserNavigateToReport(e.SelectedReports?.Count > 0 ? e.SelectedReports[0] : null);
			}
		}

		public void Dispose()
		{
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
