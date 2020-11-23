using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Sdl.Community.Transcreate.Actions;
using Sdl.Community.Transcreate.Commands;
using Sdl.Community.Transcreate.CustomEventArgs;
using Sdl.Community.Transcreate.Model;
using Sdl.Community.Transcreate.View;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Transcreate.ViewModel
{
	public class ProjectFilesViewModel : BaseModel, IDisposable
	{
		private List<ProjectFile> _projectFiles;
		private IList _selectedProjectFiles;
		private ProjectFile _selectedProjectFile;
		private bool _isProjectFileSelected;
		private bool _isMultipleProjectFilesSelected;
		private bool _isSingleProjectFileSelected;
		private ICommand _clearSelectionCommand;
		private ICommand _importFilesCommand;
		private ICommand _exportFilesCommand;
		private ICommand _openFolderCommand;
		private ICommand _viewReportCommand;
		private ICommand _openFileForTranslationCommand;
		private ICommand _openFileForReviewCommand;
		private ICommand _openFileForSignOffCommand;
		private ICommand _mouseDoubleClickCommand;
		

		public ProjectFilesViewModel(List<ProjectFile> projectFiles)
		{
			ProjectFiles = projectFiles;

			SelectedProjectFile = ProjectFiles?.Count > 0 ? projectFiles[0] : null;
			SelectedProjectFiles = new List<ProjectFile> { SelectedProjectFile };
		}

		public EventHandler<ProjectFileSelectionChangedEventArgs> ProjectFileSelectionChanged;

		public ICommand ExportFilesCommand => _exportFilesCommand ?? (_exportFilesCommand = new CommandHandler(ExportFiles));

		public ICommand ImportFilesCommand => _importFilesCommand ?? (_importFilesCommand = new CommandHandler(ImportFiles));

		public ICommand ClearSelectionCommand => _clearSelectionCommand ?? (_clearSelectionCommand = new CommandHandler(ClearSelection));

		public ICommand OpenFolderCommand => _openFolderCommand ?? (_openFolderCommand = new CommandHandler(OpenFolder));

		public ICommand ViewReportCommand => _viewReportCommand ?? (_viewReportCommand = new CommandHandler(ViewReport));

		public ICommand OpenFileForTranslationCommand => _openFileForTranslationCommand ?? (_openFileForTranslationCommand = new CommandHandler(OpenFileForTranslation));

		public ICommand OpenFileForReviewCommand => _openFileForReviewCommand ?? (_openFileForReviewCommand = new CommandHandler(OpenFileForReview));

		public ICommand OpenFileForSignOffCommand => _openFileForSignOffCommand ?? (_openFileForSignOffCommand = new CommandHandler(OpenFileForSignOff));

		public ICommand MouseDoubleClickCommand => _mouseDoubleClickCommand ?? (_mouseDoubleClickCommand = new CommandHandler(MouseDoubleClick));

		public ProjectFileActivityViewModel ProjectFileActivityViewModel { get; internal set; }

		public void Refresh()
		{
			OnPropertyChanged(nameof(ProjectFiles));
		}

		public List<ProjectFile> ProjectFiles
		{
			get => _projectFiles ?? (_projectFiles = new List<ProjectFile>());
			set
			{
				_projectFiles = value;
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

				IsMultipleProjectFilesSelected = _selectedProjectFiles?.Count > 1;

				ProjectFileSelectionChanged?.Invoke(this, new ProjectFileSelectionChangedEventArgs
				{
					SelectedFiles = _selectedProjectFiles?.Cast<ProjectFile>().ToList()
				});
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
				IsMultipleProjectFilesSelected = _selectedProjectFiles?.Count > 1;

				ProjectFileSelectionChanged?.Invoke(this, new ProjectFileSelectionChangedEventArgs
				{
					SelectedFiles = new List<ProjectFile> { _selectedProjectFile }
				});
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
					_projectFiles.Select(a => a.Project).Distinct().Count(),
					_projectFiles?.Count,
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

				IsSingleProjectFileSelected = !_isMultipleProjectFilesSelected && _isProjectFileSelected;
			}
		}

		public bool IsMultipleProjectFilesSelected
		{
			get => _isMultipleProjectFilesSelected;
			set
			{
				if (_isMultipleProjectFilesSelected == value)
				{
					return;
				}

				_isMultipleProjectFilesSelected = value;
				OnPropertyChanged(nameof(IsMultipleProjectFilesSelected));

				IsSingleProjectFileSelected = !_isMultipleProjectFilesSelected && _isProjectFileSelected;
			}
		}

		public bool IsSingleProjectFileSelected
		{
			get => _isSingleProjectFileSelected;
			set
			{
				if (_isSingleProjectFileSelected == value)
				{
					return;
				}

				_isSingleProjectFileSelected = value;
				OnPropertyChanged(nameof(IsSingleProjectFileSelected));
			}
		}

		private void ImportFiles(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<ImportAction>();
			action.LaunchWizard();
		}

		private void ExportFiles(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<ExportAction>();
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
					WindowTitle = "Report"
				};

				var view = new ReportWindow(viewModel);
				view.ShowDialog();
			}
		}

		private void MouseDoubleClick(object parameter)
		{
			if (SelectedProjectFile != null)
			{
				var action = SdlTradosStudio.Application.GetAction<OpenFileForTranslationAction>();
				action.OpenFile();

				OpenFileForTranslation(parameter);
			}
		}

		private void OpenFileForTranslation(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<OpenFileForTranslationAction>();
			action.OpenFile();
		}

		private void OpenFileForReview(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<OpenFileForReviewAction>();
			action.OpenFile();
		}

		private void OpenFileForSignOff(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<OpenFileForSignOffAction>();
			action.OpenFile();
		}

		public void Dispose()
		{
			ProjectFileActivityViewModel?.Dispose();
		}
	}
}
