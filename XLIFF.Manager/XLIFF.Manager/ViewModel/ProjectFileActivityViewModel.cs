using System;
using System.Collections;
using System.Collections.Generic;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.ViewModel
{
	public class ProjectFileActivityViewModel: BaseModel, IDisposable
	{
		private List<ProjectFileActivity> _projectFileActivities;
		private ProjectFileActivity _selectedProjectFileActivity;
		private IList _selectedProjectFileActivities;

		public ProjectFileActivityViewModel(List<ProjectFileActivity> projectFileActivities)
		{
			ProjectFileActivities = projectFileActivities;
			SelectedProjectFileActivity = ProjectFileActivities?.Count > 0 ? ProjectFileActivities[0] : null;
			SelectedProjectFileActivities = new List<ProjectFileActivity> { SelectedProjectFileActivity };
		}

		public List<ProjectFileActivity> ProjectFileActivities
		{
			get => _projectFileActivities ?? (_projectFileActivities = new List<ProjectFileActivity>());
			set
			{
				_projectFileActivities = value;
				OnPropertyChanged(nameof(ProjectFileActivities));
			}  
		}

		public IList SelectedProjectFileActivities
		{
			get => _selectedProjectFileActivities;
			set
			{
				_selectedProjectFileActivities = value;
				OnPropertyChanged(nameof(SelectedProjectFileActivities));
			}
		}

		public ProjectFileActivity SelectedProjectFileActivity
		{
			get => _selectedProjectFileActivity;
			set
			{
				_selectedProjectFileActivity = value;
				OnPropertyChanged(nameof(SelectedProjectFileActivity));
			}
		}

		public void Dispose()
		{
		}		
	}
}
