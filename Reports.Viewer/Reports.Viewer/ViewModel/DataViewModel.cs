using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.Reports.Viewer.Actions;
using Sdl.Community.Reports.Viewer.Commands;
using Sdl.Community.Reports.Viewer.CustomEventArgs;
using Sdl.Reports.Viewer.API.Model;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Reports.Viewer.ViewModel
{
	public class DataViewModel : INotifyPropertyChanged, IDisposable
	{
		private string _windowTitle;
		private ObservableCollection<Report> _reports;
		private Report _selectedReport;
		private IList _selectedReports;
		private string _projectLocalFolder;
		private ICommand _clearSelectionCommand;
		private ICommand _editReportCommand;
		private ICommand _removeReportCommand;
		private ICommand _openFolderCommand;
		private ICommand _printReportCommand;
		private ICommand _printPreviewCommand;
		private ICommand _pageSetupCommand;
		private ICommand _saveAsCommand;
		private ICommand _dragDropCommand;
		private ICommand _mouseDoubleClick;

		public event EventHandler<ReportSelectionChangedEventArgs> ReportSelectionChanged;

		public ICommand ClearSelectionCommand => _clearSelectionCommand ?? (_clearSelectionCommand = new CommandHandler(ClearSelection));

		public ICommand EditReportCommand => _editReportCommand ?? (_editReportCommand = new CommandHandler(EditReport));

		public ICommand RemoveReportCommand => _removeReportCommand ?? (_removeReportCommand = new CommandHandler(RemoveReport));

		public ICommand OpenFolderCommand => _openFolderCommand ?? (_openFolderCommand = new CommandHandler(OpenFolder));

		public ICommand PrintReportCommand => _printReportCommand ?? (_printReportCommand = new CommandHandler(PrintReport));

		public ICommand PrintPreviewCommand => _printPreviewCommand ?? (_printPreviewCommand = new CommandHandler(PrintPreview));

		public ICommand PageSetupCommand => _pageSetupCommand ?? (_pageSetupCommand = new CommandHandler(PageSetup));

		public ICommand SaveAsCommand => _saveAsCommand ?? (_saveAsCommand = new CommandHandler(SaveAs));

		public ICommand DragDropCommand => _dragDropCommand ?? (_dragDropCommand = new CommandHandler(DragDrop));

		public ICommand MouseDoubleClickCommand => _mouseDoubleClick ?? (_mouseDoubleClick = new CommandHandler(MouseDoubleClick));

		public string WindowTitle
		{
			get => _windowTitle;
			set
			{
				_windowTitle = value;
				OnPropertyChanged(nameof(WindowTitle));
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

		public ObservableCollection<Report> Reports
		{
			get => _reports;
			set
			{
				_reports = value;
				OnPropertyChanged(nameof(Reports));
				OnPropertyChanged(nameof(StatusLabel));
			}
		}

		public bool IsReportsSelected => SelectedReports?.Cast<Report>().ToList().Count > 0;

		public bool IsReportSelected => SelectedReports?.Cast<Report>().ToList().Count == 1;

		public Report SelectedReport
		{
			get => _selectedReport;
			set
			{
				_selectedReport = value;
				OnPropertyChanged(nameof(SelectedReport));

				ReportSelectionChanged?.Invoke(this, new ReportSelectionChangedEventArgs
				{
					SelectedReport = _selectedReport,
					SelectedReports = SelectedReports?.Cast<Report>().ToList()
				});

				OnPropertyChanged(nameof(IsReportSelected));
				OnPropertyChanged(nameof(StatusLabel));
			}
		}

		public IList SelectedReports
		{
			get => _selectedReports;
			set
			{
				_selectedReports = value;
				OnPropertyChanged(nameof(SelectedReports));


				_selectedReport = _selectedReports?.Cast<Report>().ToList().FirstOrDefault();
				ReportSelectionChanged?.Invoke(this, new ReportSelectionChangedEventArgs
				{
					SelectedReport = _selectedReport,
					SelectedReports = _selectedReports?.Cast<Report>().ToList()
				});

				OnPropertyChanged(nameof(IsReportSelected));
				OnPropertyChanged(nameof(StatusLabel));
			}
		}

		public string StatusLabel
		{
			get
			{
				var message = string.Format(PluginResources.StatusLabel_ReportsSelected,
					_reports?.Count ?? 0,
					_selectedReports?.Count ?? 0);
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

		private void OpenFolder(object parameter)
		{
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

		private void PrintReport(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<PrintReportAction>();
			action.Run();
		}

		private void PrintPreview(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<PrintPreviewReportAction>();
			action.Run();
		}

		private void PageSetup(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<PageSetupAction>();
			action.Run();
		}

		private void SaveAs(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<SaveAsReportAction>();
			action.Run();
		}

		private void DragDrop(object parameter)
		{
			var report = new Report();

			if (parameter == null || !(parameter is DragEventArgs eventArgs))
			{
				return;
			}

			var fileDrop = eventArgs.Data.GetData(DataFormats.FileDrop, false);
			if (fileDrop is string[] files && files.Length > 0 && files.Length <= 2)
			{
				foreach (var fullPath in files)
				{
					var fileAttributes = File.GetAttributes(fullPath);
					if (!fileAttributes.HasFlag(FileAttributes.Directory))
					{
						if (string.IsNullOrEmpty(report.XsltPath) &&
							(fullPath.ToLower().EndsWith(".xslt")
							 || fullPath.ToLower().EndsWith(".xsl")))
						{
							report.XsltPath = fullPath;
						}
						if (string.IsNullOrEmpty(report.Path) &&
							(fullPath.ToLower().EndsWith(".html")
							 || fullPath.ToLower().EndsWith(".htm")
							 || fullPath.ToLower().EndsWith(".xml")))
						{
							report.Path = fullPath;
						}
					}
				}
			}

			var action = SdlTradosStudio.Application.GetAction<AddReportAction>();
			action.Run(report);
		}

		private void MouseDoubleClick(object parameter)
		{
			if (SelectedReport != null)
			{
				var action = SdlTradosStudio.Application.GetAction<EditReportAction>();
				action.Run();
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
