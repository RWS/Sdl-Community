using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Actions.Export;
using Sdl.Community.XLIFF.Manager.Actions.Import;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Community.XLIFF.Manager.View;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.ViewModel
{
	public class ProjectFilesViewModel : BaseModel, IDisposable
	{
		private List<ProjectFile> _projectFileActions;
		private IList _selectedProjectFiles;
		private ProjectFile _selectedProjectFile;
		private bool _isProjectFileSelected;
		private ICommand _clearSelectionCommand;
		private ICommand _importFilesCommand;
		private ICommand _exportFilesCommand;
		private ICommand _openFolderCommand;
		private ICommand _viewReportCommand;

		public ProjectFilesViewModel(List<ProjectFile> projectFiles)
		{
			ProjectFiles = projectFiles;

			SelectedProjectFile = ProjectFiles?.Count > 0 ? projectFiles[0] : null;
			SelectedProjectFiles = new List<ProjectFile> { SelectedProjectFile };
		}

		public ICommand ExportFilesCommand => _exportFilesCommand ?? (_exportFilesCommand = new CommandHandler(ExportFiles));

		public ICommand ImportFilesCommand => _importFilesCommand ?? (_importFilesCommand = new CommandHandler(ImportFiles));

		public ICommand ClearSelectionCommand => _clearSelectionCommand ?? (_clearSelectionCommand = new CommandHandler(ClearSelection));

		public ICommand OpenFolderCommand => _openFolderCommand ?? (_openFolderCommand = new CommandHandler(OpenFolder));

		public ICommand ViewReportCommand => _viewReportCommand ?? (_viewReportCommand = new CommandHandler(ViewReport));

		public ProjectFileActivityViewModel ProjectFileActivityViewModel { get; internal set; }

		public void Refresh()
		{
			OnPropertyChanged(nameof(ProjectFiles));
		}

		public List<ProjectFile> ProjectFiles
		{
			get => _projectFileActions ?? (_projectFileActions = new List<ProjectFile>());
			set
			{
				_projectFileActions = value;
				OnPropertyChanged(nameof(ProjectFiles));
				OnPropertyChanged(nameof(StatusLabel));
			}
		}

		public IList SelectedProjectFiles
		{
			get => _selectedProjectFiles;
			set
			{
				_selectedProjectFiles = value;
				OnPropertyChanged(nameof(SelectedProjectFiles));
				OnPropertyChanged(nameof(StatusLabel));
			}
		}

		public ProjectFile SelectedProjectFile
		{
			get => _selectedProjectFile;
			set
			{
				_selectedProjectFile = value;
				OnPropertyChanged(nameof(SelectedProjectFile));

				if (ProjectFileActivityViewModel != null)
				{
					ProjectFileActivityViewModel.ProjectFileActivities = _selectedProjectFile?.ProjectFileActivities;
				}

				IsProjectFileSelected = _selectedProjectFile != null;
			}
		}

		private void ClearSelection(object parameter)
		{
			SelectedProjectFiles?.Clear();
			SelectedProjectFile = null;
		}

		public string StatusLabel
		{
			get
			{
				var message = string.Format(PluginResources.StatusLabel_Projects_0_Files_1_Selected_2,
					_projectFileActions.Select(a => a.Project).Distinct().Count(),
					_projectFileActions?.Count,
					_selectedProjectFiles?.Count);
				return message;
			}
		}

		public bool IsProjectFileSelected
		{
			get => _isProjectFileSelected;
			set
			{
				if (_isProjectFileSelected == value)
				{
					return;
				}

				_isProjectFileSelected = value;
				OnPropertyChanged(nameof(IsProjectFileSelected));
			}
		}

		private void ImportFiles(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<ImportFromXLIFFAction>();
			action.LaunchWizard();
		}

		private void ExportFiles(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<ExportToXLIFFAction>();
			action.LaunchWizard();
		}

		private void OpenFolder(object parameter)
		{
			if (SelectedProjectFile?.Project?.Path == null)
			{
				return;
			}

			var path = Path.Combine(SelectedProjectFile.Project.Path, SelectedProjectFile.Location.Trim('\\'));

			if (File.Exists(path))
			{
				System.Diagnostics.Process.Start("explorer.exe", Path.GetDirectoryName(path));
			}
		}

		private void ViewReport(object parameter)
		{
			if (string.IsNullOrEmpty(SelectedProjectFile?.Report))
			{
				return;
			}

			if (SelectedProjectFile?.Project?.Path == null)
			{
				return;
			}

			var path = Path.Combine(SelectedProjectFile.Project.Path, SelectedProjectFile.Report.Trim('\\'));
			if (File.Exists(path))
			{
				var viewModel = new ReportViewModel
				{
					HtmlUri = path,
					WindowTitle = SelectedProjectFile.Action == Enumerators.Action.Export
						? "Export to XLIFF Report"
						: "Import from XLIFF Report"
				};

				var view = new ReportWindow(viewModel);
				view.ShowDialog();
			}
		}

		public void Dispose()
		{
			ProjectFileActivityViewModel?.Dispose();
		}
	}
}
