using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.ViewModel
{
	public class ProjectFilesViewModel : BaseModel, IDisposable
	{
		private List<ProjectFile> _projectFileActions;
		private IList _selectedProjectFiles;
		private ProjectFile _selectedProjectFile;
		private ICommand _clearSelectionCommand;

		public ProjectFilesViewModel(List<ProjectFile> projectFiles)
		{
			ProjectFiles = projectFiles;
			SelectedProjectFile = ProjectFiles?.Count > 0 ? projectFiles[0] : null;
			SelectedProjectFiles = new List<ProjectFile> { SelectedProjectFile };
		}

		public ICommand ClearSelectionCommand => _clearSelectionCommand ?? (_clearSelectionCommand = new CommandHandler(ClearSelection));

		public ProjectFileActivityViewModel ProjectFileActivityViewModel { get; internal set; }

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
					_projectFileActions.Select(a => a.ProjectModel).Distinct().Count(), 
					_projectFileActions?.Count, 
					_selectedProjectFiles?.Count);
				return message;
			}
		}
		
		public void Dispose()
		{
			ProjectFileActivityViewModel?.Dispose();
		}
	}
}
