using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Sdl.Reports.Viewer.API.Model;

namespace Sdl.Community.Reports.Viewer.ViewModel
{
	public class ReportViewModel : INotifyPropertyChanged, IDisposable
	{
		private string _windowTitle;
		private readonly ContentControl _browserView;
		private readonly ContentControl _dataView;
		private readonly BrowserViewModel _browserViewModel;
		private readonly DataViewModel _dataViewModel;
		private ContentControl _currentView;

		public ReportViewModel(BrowserViewModel browserViewModel, ContentControl browserView,
			DataViewModel dataViewModel, ContentControl dataView)
		{
			_browserViewModel = browserViewModel;
			_browserView = browserView;

			_dataViewModel = dataViewModel;
			_dataView = dataView;

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

		public string ProjectLocalFolder { get; set; }

		public void UpdateReport(Report report)
		{
			CurrentView = _browserView;

			string file = null;
			if (report != null)
			{
				file = Path.Combine(ProjectLocalFolder, report.Path);
				if (!File.Exists(file))
				{
					file = null;
				}
			}

			_browserViewModel.HtmlUri = file;
		}

		public void UpdateData(List<Report> reports)
		{
			CurrentView = _dataView;
			_dataViewModel.Reports = reports;
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
