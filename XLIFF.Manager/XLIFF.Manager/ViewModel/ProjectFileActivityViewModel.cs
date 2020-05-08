using System;
using System.Collections;
using System.Collections.Generic;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.ViewModel
{
	public class ProjectFileActivityViewModel: BaseModel, IDisposable
	{
		private List<ProjectFileActivityModel> _projectFileActivities;
		private ProjectFileActivityModel _selectedProjectFileActivity;
		private IList _selectedProjectFileActivities;

		public ProjectFileActivityViewModel(List<ProjectFileActivityModel> projectFileActivities)
		{
			ProjectFileActivities = projectFileActivities;
			SelectedProjectFileActivity = ProjectFileActivities?.Count > 0 ? ProjectFileActivities[0] : null;
			SelectedProjectFileActivities = new List<ProjectFileActivityModel> { SelectedProjectFileActivity };
		}

		public List<ProjectFileActivityModel> ProjectFileActivities
		{
			get => _projectFileActivities ?? (_projectFileActivities = new List<ProjectFileActivityModel>());
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

		public ProjectFileActivityModel SelectedProjectFileActivity
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
