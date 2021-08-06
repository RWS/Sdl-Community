using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Sdl.Community.Reports.Viewer.API.Example.Actions;
using Sdl.Community.Reports.Viewer.API.Example.Commands;
using Sdl.Community.Reports.Viewer.API.Example.Controls;
using Sdl.ProjectAutomation.FileBased.Reports.Models;
using Sdl.ProjectAutomation.FileBased.Reports.Operations;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Reports.Viewer.API.Example.ViewModel
{
	public class DataViewModel : INotifyPropertyChanged, IDisposable
	{
		private string _windowTitle;
		private string _reportContent;
		private List<Report> _reports;
		private Report _selectedReport;
		private IList _selectedReports;
		private string _projectLocalFolder;
		private ICommand _clearSelectionCommand;
		private ICommand _editReportCommand;
		private ICommand _removeReportCommand;
		private ICommand _renderReportCommand;
		private ICommand _openFolderCommand;
		private readonly ReportsViewerController _reportViewerController;

		public DataViewModel(List<Report> reports, ReportsViewerController reportViewerController)
		{
			Reports = reports;
			_reportViewerController = reportViewerController;			
		}
	
		public ICommand ClearSelectionCommand => _clearSelectionCommand ?? (_clearSelectionCommand = new CommandHandler(ClearSelection));

		public ICommand EditReportCommand => _editReportCommand ?? (_editReportCommand = new CommandHandler(EditReport));

		public ICommand RemoveReportCommand => _removeReportCommand ?? (_removeReportCommand = new CommandHandler(RemoveReport));

		public ICommand RenderReportCommand => _renderReportCommand ?? (_renderReportCommand = new CommandHandler(RenderReport));

		public ICommand OpenFolderCommand => _openFolderCommand ?? (_openFolderCommand = new CommandHandler(OpenFolder));

		public string WindowTitle
		{
			get => _windowTitle;
			set
			{
				_windowTitle = value;
				OnPropertyChanged(nameof(WindowTitle));
			}
		}

		public string ReportContent
		{
			get => _reportContent;
			set
			{
				_reportContent = value;
				OnPropertyChanged(nameof(ReportContent));
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
			}
		}

		public List<Report> Reports
		{
			get => _reports;
			set
			{
				_reports = value;
				OnPropertyChanged(nameof(Reports));
				OnPropertyChanged(nameof(StatusLabel));
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

		private void EditReport(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<EditReportAction>();
			action.Run();
		}

		private void RemoveReport(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<RemoveReportAction>();
			action.Run();
		}

		private void RenderReport(object parameter)
		{
			var report = parameter as Report;
			ReportContent = _reportViewerController.RenderReport(report);
			//var path = Path.Combine(ProjectLocalFolder, report.Path);
			//File.WriteAllText(path, ReportContent);
		}

		private void OpenFolder(object parameter)
		{
			var reports = _reportViewerController.GetSelectedReports();
			if(reports != null)
			{
				SelectedReport = reports[0];
			}
			if (SelectedReport?.Path == null || string.IsNullOrEmpty(ProjectLocalFolder)
			                                 || !Directory.Exists(ProjectLocalFolder))
			{
				return;
			}

			var path = Path.Combine(ProjectLocalFolder, SelectedReport.Path.Trim('\\'));

			if (File.Exists(path))
			{
				System.Diagnostics.Process.Start("explorer.exe", Path.GetDirectoryName(path));
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
