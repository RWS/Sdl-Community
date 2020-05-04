using System;
using System.Collections;
using System.Collections.Generic;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.ViewModel
{
	public class ProjectFilesViewModel : BaseModel, IDisposable
	{
		private List<ProjectFileActionModel> _projectFileActions;
		private IList _selectedProjectFileActions;
		private ProjectFileActionModel _selectedProjectFileAction;

		public ProjectFilesViewModel(List<ProjectFileActionModel> projectFileActions)
		{
			ProjectFileActions = projectFileActions;
			SelectedProjectFileAction = ProjectFileActions?.Count > 0 ? projectFileActions[0] : null;			
			SelectedProjectFileActions = new List<ProjectFileActionModel> {SelectedProjectFileAction};
		}

		public ProjectFileActivityViewModel ProjectFileActivityViewModel { get; internal set; }

		public List<ProjectFileActionModel> ProjectFileActions
		{
			get => _projectFileActions ?? (_projectFileActions = new List<ProjectFileActionModel>());
			set
			{
				_projectFileActions = value;
				OnPropertyChanged(nameof(ProjectFileActions));
			}
		}

		public IList SelectedProjectFileActions
		{
			get => _selectedProjectFileActions;
			set
			{
				_selectedProjectFileActions = value;
				OnPropertyChanged(nameof(SelectedProjectFileActions));
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
					ProjectFileActivityViewModel.ProjectFileActivities = _selectedProjectFileAction.ProjectFileActivityModels;
				}
			}
		}

		public void Dispose()
		{
		}
	}
}
