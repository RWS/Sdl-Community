using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CustomViewExample.Model;

namespace CustomViewExample.ViewModel
{
	public class NavigationViewModel : BaseViewModel
	{
		private ObservableCollection<CustomViewProject> _projects;
		private CustomViewProject _selectedProject;

		public NavigationViewModel(List<CustomViewProject> customViewProjects)
		{
			Name = "Navigation Pane";

			Projects = new ObservableCollection<CustomViewProject>(customViewProjects);
			SelectedProject = customViewProjects.FirstOrDefault();
		}

		public event EventHandler<CustomViewProject> SelectedProjectChanged;

		public ObservableCollection<CustomViewProject> Projects
		{
			get => _projects;
			set
			{
				if (_projects == value)
				{
					return;
				}

				_projects = value;
				OnPropertyChanged(nameof(Projects));
				OnPropertyChanged(nameof(StatusMessage));
			}
		}

		public CustomViewProject SelectedProject
		{
			get => _selectedProject;
			set
			{
				if (_selectedProject == value)
				{
					return;
				}

				_selectedProject = value;
				OnPropertyChanged(nameof(SelectedProject));

				SelectedProjectChanged?.Invoke(this, _selectedProject);
			}
		}

		public string StatusMessage => $"Projects {Projects.Count}";
	}
}
