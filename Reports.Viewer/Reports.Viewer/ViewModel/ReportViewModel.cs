using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sdl.Community.Reports.Viewer.Model;
using Sdl.Community.Reports.Viewer.View;

namespace Sdl.Community.Reports.Viewer.ViewModel
{
	public class ReportViewModel : INotifyPropertyChanged, IDisposable
	{
		private string _windowTitle;
		private readonly BrowserView _browserView;
		private readonly DataView _dataView;
		private readonly BrowserViewModel _browserViewModel;
		private readonly DataViewModel _dataViewModel;
		private object _currentView;

		//public ReportViewModel(BrowserViewModel browserViewModel, object browserView,
		//	DataViewModel dataViewModel, object dataView)
		//{
		//	_browserViewModel = browserViewModel;
		//	_browserView = browserView;

		//	_dataViewModel = dataViewModel;
		//	_dataView = dataView;

		//	CurrentView = _dataView;
		//}

		public ReportViewModel()
		{
			_browserViewModel = new BrowserViewModel();
			_browserView = new BrowserView();
			_browserView.DataContext = _browserViewModel;

			_dataViewModel = new DataViewModel();
			_dataView = new DataView();
			_dataView.DataContext = _dataViewModel;

			//CurrentView = _dataView;
		}

		public object CurrentView
		{
			get => _currentView;
			set
			{
				_currentView = value;
				OnPropertyChanged(nameof(CurrentView));
			}
		}
		
		public void UpdateReport(string filePath)
		{
			CurrentView = _browserView;
			_browserViewModel.HtmlUri = filePath;
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
