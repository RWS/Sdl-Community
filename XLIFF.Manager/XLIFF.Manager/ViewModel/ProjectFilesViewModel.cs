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
		private List<ProjectFileActionModel> _projectFileActions;
		private IList _selectedProjectFileActions;
		private ProjectFileActionModel _selectedProjectFileAction;
		private ICommand _clearSelectionCommand;

		public ProjectFilesViewModel(List<ProjectFileActionModel> projectFileActions)
		{
			ProjectFileActions = projectFileActions;
			SelectedProjectFileAction = ProjectFileActions?.Count > 0 ? projectFileActions[0] : null;
			SelectedProjectFileActions = new List<ProjectFileActionModel> { SelectedProjectFileAction };
		}

		public ICommand ClearSelectionCommand => _clearSelectionCommand ?? (_clearSelectionCommand = new CommandHandler(ClearSelection));

		public ProjectFileActivityViewModel ProjectFileActivityViewModel { get; internal set; }

		public List<ProjectFileActionModel> ProjectFileActions
		{
			get => _projectFileActions ?? (_projectFileActions = new List<ProjectFileActionModel>());
			set
			{
				_projectFileActions = value;
				OnPropertyChanged(nameof(ProjectFileActions));
				OnPropertyChanged(nameof(StatusLabel));
			}
		}

		public IList SelectedProjectFileActions
		{
			get => _selectedProjectFileActions;
			set
			{
				_selectedProjectFileActions = value;
				OnPropertyChanged(nameof(SelectedProjectFileActions));
				OnPropertyChanged(nameof(StatusLabel));			
			}
		}

		public ProjectFileActionModel SelectedProjectFileAction
		{
			get => _selectedProjectFileAction;
			set
			{
				_selectedProjectFileAction = value;
				OnPropertyChanged(nameof(SelectedProjectFileAction));

				if (ProjectFileActivityViewModel != null)
				{
					ProjectFileActivityViewModel.ProjectFileActivities = _selectedProjectFileAction?.ProjectFileActivityModels;
				}
			}
		}

		private void ClearSelection(object parameter)
		{
			SelectedProjectFileActions.Clear();
			SelectedProjectFileAction = null;
		}

		public string StatusLabel
		{
			get
			{				
				var message = string.Format(PluginResources.StatusLabel_Projects_0_Files_1_Selected_2, 
					_projectFileActions.Select(a => a.ProjectModel).Distinct().Count(), 
					_projectFileActions?.Count, 
					_selectedProjectFileActions?.Count);
				return message;
			}
		}

		public void Dispose()
		{
			ProjectFileActivityViewModel?.Dispose();
		}
	}
}
