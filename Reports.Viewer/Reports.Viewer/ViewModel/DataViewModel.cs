using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.Reports.Viewer.Actions;
using Sdl.Community.Reports.Viewer.Commands;
using Sdl.Community.Reports.Viewer.Model;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Reports.Viewer.ViewModel
{
	public class DataViewModel : INotifyPropertyChanged, IDisposable
	{
		private string _windowTitle;
		private List<Report> _reports;
		private Report _selectedReport;
		private IList _selectedReports;
		private ICommand _clearSelectionCommand;
		private ICommand _editReportCommand;
		private ICommand _removeReportCommand;
		private ICommand _openFolderCommand;
	
		public ICommand ClearSelectionCommand => _clearSelectionCommand ?? (_clearSelectionCommand = new CommandHandler(ClearSelection));

		public ICommand EditReportCommand => _editReportCommand ?? (_editReportCommand = new CommandHandler(EditReport));

		public ICommand RemoveReportCommand => _removeReportCommand ?? (_removeReportCommand = new CommandHandler(RemoveReport));

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

		private void EditReport(object parameter)
		{
			MessageBox.Show("TODO");
		}

		private void RemoveReport(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<RemoveReportAction>();
			action.Run();
		}

		private void OpenFolder(object parameter)
		{
			MessageBox.Show("TODO");
			return;

			//if (SelectedReport?.Path == null || SelectedReport?.Project == null)
			//{
			//	return;
			//}


			//var projectInfo = SelectedReport?.Project.GetProjectInfo();
			//var path = System.IO.Path.Combine(projectInfo.LocalProjectFolder, SelectedReport.Path.Trim('\\'));

			//if (File.Exists(path))
			//{
			//	System.Diagnostics.Process.Start("explorer.exe", System.IO.Path.GetDirectoryName(path));
			//}
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
