using System;
using System.Collections.Generic;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.ViewModel
{
	public class ProjectFileActivityViewModel: BaseModel, IDisposable
	{
		private List<ProjectFileActivityModel> _projectFileActivities;
		private ProjectFileActivityModel _selectedProjectFileActivity;

		public ProjectFileActivityViewModel(List<ProjectFileActivityModel> projectFileActivities)
		{
			ProjectFileActivities = projectFileActivities;
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
