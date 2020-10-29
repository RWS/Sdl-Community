using System;
using Sdl.Community.Transcreate.Model;

namespace Sdl.Community.Transcreate.ViewModel
{
	public class ProjectPropertiesViewModel : BaseModel, IDisposable
	{
		private Interfaces.IProject _project;

		public ProjectPropertiesViewModel(Interfaces.IProject project)
		{
			SelectedProject = project;
		}

		public Interfaces.IProject SelectedProject
		{
			get => _project;
			set
			{
				_project = value;
				OnPropertyChanged(nameof(Project));				
			}
		}

		public void Dispose()
		{
		}
	}
}
