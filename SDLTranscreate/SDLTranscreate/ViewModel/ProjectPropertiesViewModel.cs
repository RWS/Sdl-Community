using System;
using Sdl.Community.Transcreate.Model;

namespace Sdl.Community.Transcreate.ViewModel
{
	public class ProjectPropertiesViewModel : BaseModel, IDisposable
	{
		private Project _projects;

		public ProjectPropertiesViewModel(Project project)
		{
			SelectedProject = project;
		}

		public Project SelectedProject
		{
			get => _projects;
			set
			{
				_projects = value;
				OnPropertyChanged(nameof(Project));				
			}
		}

		public void Dispose()
		{
		}
	}
}
