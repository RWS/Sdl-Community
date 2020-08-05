using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Sdl.Community.Reports.Viewer.Commands;
using Sdl.Community.Reports.Viewer.Model;

namespace Sdl.Community.Reports.Viewer.ViewModel
{
	public class DataViewModel : INotifyPropertyChanged, IDisposable
	{
		private string _windowTitle;
		private List<Report> _reports;
		private Report _selectedReport;
		private IList _selectedReports;
		private ICommand _clearSelectionCommand;


		public ICommand ClearSelectionCommand => _clearSelectionCommand ?? (_clearSelectionCommand = new CommandHandler(ClearSelection));


		public string WindowTitle
		{
			get => _windowTitle;
			set
			{
				_windowTitle = value;
				OnPropertyChanged(nameof(WindowTitle));
			}
		}


		public List<Report> Reports
		{
			get => _reports;
			set
			{
				_reports = value;
				OnPropertyChanged(nameof(Reports));
			}
		}

		public Report SelectedReport
		{
			get => _selectedReport;
			set
			{
				_selectedReport = value;
				OnPropertyChanged(nameof(SelectedReport));
			}
		}

		public IList SelectedReports
		{
			get => _selectedReports;
			set
			{
				_selectedReports = value;
				OnPropertyChanged(nameof(SelectedReports));
				OnPropertyChanged(nameof(StatusLabel));
			}
		}

		public string StatusLabel
		{
			get
			{
				var message = string.Format("Reports: {0}, Selected: {1}",
					_reports.Count,
					_selectedReports.Count);
				return message;
			}
		}


		private void ClearSelection(object parameter)
		{
			SelectedReports?.Clear();
			SelectedReport = null;
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
