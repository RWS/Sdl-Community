using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.CustomEventArgs;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.ViewModel
{
	public class ProjectsNavigationViewModel : BaseModel, IDisposable
	{
		private List<Project> _projects;
		private string _filterString;
		private List<Project> _filteredProjects;
		private Project _selectedProject;
		private IList _selectedProjects;
		private ICommand _clearSelectionCommand;
		private ICommand _clearFilterCommand;
		public EventHandler<ProjectSelectionChangedEventArgs> ProjectSelectionChanged;

		public ProjectsNavigationViewModel(List<Project> projects)
		{
			_projects = projects;
			FilteredProjects = _projects;
			FilterString = string.Empty;
		}

		public ICommand ClearSelectionCommand => _clearSelectionCommand ?? (_clearSelectionCommand = new CommandHandler(ClearSelection));

		public ICommand ClearFilterCommand => _clearFilterCommand ?? (_clearFilterCommand = new CommandHandler(ClearFilter));

		public List<Project> Projects
		{
			get => _projects;
			set
			{
				_projects = value;

				OnPropertyChanged(nameof(Projects));

				FilterString = string.Empty;
				FilteredProjects = _projects;

				
			}
		}

		public ProjectFilesViewModel ProjectFilesViewModel { get; internal set; }

		public string FilterString
		{
			get => _filterString;
			set
			{
				if (_filterString == value)
				{
					return;
				}

				_filterString = value;
				OnPropertyChanged(nameof(FilterString));

				FilteredProjects = string.IsNullOrEmpty(_filterString)
					? _projects
					: _projects.Where(a => a.Name.ToLower().Contains(_filterString.ToLower())).ToList();
			}
		}

		public string StatusLabel
		{
			get
			{
				var message = string.Format(PluginResources.StatusLabel_Selected_0, _selectedProjects?.Count);
				return message;
			}
		}

		public List<Project> FilteredProjects
		{
			get => _filteredProjects;
			set
			{
				_filteredProjects = value;
				OnPropertyChanged(nameof(FilteredProjects));

				if (_filteredProjects?.Count > 0 && !_filteredProjects.Contains(SelectedProject))
				{
					SelectedProject = _filteredProjects[0];
				}
				else if (_filteredProjects?.Count == 0)
				{
					SelectedProject = null;
				}

				OnPropertyChanged(nameof(StatusLabel));
			}
		}

		public IList SelectedProjects
		{
			get => _selectedProjects;
			set
			{
				_selectedProjects = value;
				OnPropertyChanged(nameof(SelectedProjects));

				if (ProjectFilesViewModel != null && _selectedProjects != null)
				{
					ProjectFilesViewModel.ProjectFiles =
						_selectedProjects.Cast<Project>().SelectMany(a => a.ProjectFiles).ToList();
				}

				OnPropertyChanged(nameof(StatusLabel));
			}
		}

		public Project SelectedProject
		{
			get => _selectedProject;
			set
			{
				_selectedProject = value;
				OnPropertyChanged(nameof(SelectedProject));

				if (ProjectFilesViewModel != null && _selectedProject != null)
				{
					ProjectFilesViewModel.ProjectFiles = _selectedProject.ProjectFiles;
				}

				ProjectSelectionChanged?.Invoke(this, new ProjectSelectionChangedEventArgs{SelectedProject = _selectedProject});
			}
		}

		private void ClearSelection(object parameter)
		{
			SelectedProjects?.Clear();
			SelectedProject = null;
		}

		private void ClearFilter(object parameter)
		{
			FilterString = string.Empty;
		}

		public void Dispose()
		{
			ProjectFilesViewModel?.Dispose();
		}
	}
}
