using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.Transcreate.Actions;
using Sdl.Community.Transcreate.Commands;
using Sdl.Community.Transcreate.CustomEventArgs;
using Sdl.Community.Transcreate.Model;
using Sdl.Community.Transcreate.Model.ProjectSettings;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Transcreate.ViewModel
{
	public class ProjectsNavigationViewModel : BaseModel, IDisposable
	{
		private readonly ProjectsController _projectsController;
		private List<Project> _projects;
		private string _filterString;
		private List<Project> _filteredProjects;
		private List<CustomerGroup> _customerGroups;
		private Project _selectedProject;
		private IList _selectedProjects;
		private bool _isProjectSelected;
		private ICommand _clearSelectionCommand;
		private ICommand _clearFilterCommand;
		private ICommand _removeProjectDataCommand;
		private ICommand _openProjectFolderCommand;
		private ICommand _importFilesCommand;
		private ICommand _exportFilesCommand;
		private ICommand _selectedItemChanged;
		public EventHandler<ProjectSelectionChangedEventArgs> ProjectSelectionChanged;

		public ProjectsNavigationViewModel(List<Project> projects, ProjectsController projectsController)
		{
			_projects = projects;
			_projectsController = projectsController;

			FilteredProjects = _projects;
			FilterString = string.Empty;
		}

		public ICommand ExportFilesCommand => _exportFilesCommand ?? (_exportFilesCommand = new CommandHandler(ExportFiles));

		public ICommand ImportFilesCommand => _importFilesCommand ?? (_importFilesCommand = new CommandHandler(ImportFiles));

		public ICommand ClearSelectionCommand => _clearSelectionCommand ?? (_clearSelectionCommand = new CommandHandler(ClearSelection));

		public ICommand ClearFilterCommand => _clearFilterCommand ?? (_clearFilterCommand = new CommandHandler(ClearFilter));

		public ICommand RemoveProjectDataCommand => _removeProjectDataCommand ?? (_removeProjectDataCommand = new CommandHandler(RemoveProjectData));

		public ICommand OpenProjectFolderCommand => _openProjectFolderCommand ?? (_openProjectFolderCommand = new CommandHandler(OpenProjectFolder));

		public ICommand SelectedItemChangedCommand => _selectedItemChanged ?? (_selectedItemChanged = new CommandHandler(SelectedItemChanged));

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

		public ProjectPropertiesViewModel ProjectPropertiesViewModel { get; internal set; }

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


				var customerGroups = new List<CustomerGroup>();
				if (_filteredProjects != null)
				{
					foreach (var filteredProject in _filteredProjects)
					{
						var customer = customerGroups.FirstOrDefault(a => a.Customer?.Name == filteredProject.Customer?.Name);
						if (customer == null)
						{
							customer = new CustomerGroup
							{
								Customer = filteredProject.Customer,
								Projects = new List<Project> { filteredProject }
							};

							customerGroups.Add(customer);
						}
						else
						{
							customer.Projects.Add(filteredProject);
						}

						if (filteredProject.IsSelected)
						{
							customer.IsExpanded = true;
						}
					}
				}

				CustomerGroups = customerGroups;


				OnPropertyChanged(nameof(StatusLabel));
			}
		}

		public List<CustomerGroup> CustomerGroups
		{
			get => _customerGroups;
			set
			{
				_customerGroups = value;
				OnPropertyChanged(nameof(CustomerGroups));
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

				if (ProjectPropertiesViewModel != null)
				{
					ProjectPropertiesViewModel.SelectedProject = _selectedProject;
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

				if (ProjectPropertiesViewModel != null)
				{
					ProjectPropertiesViewModel.SelectedProject = _selectedProject;
				}

				ProjectSelectionChanged?.Invoke(this, new ProjectSelectionChangedEventArgs { SelectedProject = _selectedProject });

				IsProjectSelected = _selectedProject != null;
			}
		}

		public bool IsProjectSelected
		{
			get => _isProjectSelected;
			set
			{
				if (_isProjectSelected == value)
				{
					return;
				}

				_isProjectSelected = value;
				OnPropertyChanged(nameof(IsProjectSelected));
			}
		}

		private void ImportFiles(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<ImportAction>();
			action.LaunchWizard();
		}

		private void ExportFiles(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<ExportAction>();
			action.LaunchWizard();
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

		private void RemoveProjectData(object parameter)
		{
			var message1 = PluginResources.Message_ActionWillRemoveAllProjectData;
			var message2 = PluginResources.Message_DoYouWantToProceed;

			var response = MessageBox.Show(message1 + Environment.NewLine + Environment.NewLine + message2,
				PluginResources.TranscreateManager_Name, MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (response == MessageBoxResult.No)
			{
				return;
			}

			var selectedProject = _projectsController.GetProjects()
				.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == SelectedProject.Id);

			if (selectedProject != null)
			{
				var settingsBundle = selectedProject.GetSettings();
				var managerProject = settingsBundle.GetSettingsGroup<SDLTranscreateProject>();

				managerProject.ProjectFilesJson.Value = string.Empty;

				selectedProject.UpdateSettings(settingsBundle);
				selectedProject.Save();

				var xliffFolderPath = Path.Combine(SelectedProject.Path, "Transcreate");
				if (Directory.Exists(xliffFolderPath))
				{
					try
					{
						Directory.Delete(xliffFolderPath, true);
					}
					catch
					{
						// ignore; catch all
					}
				}

				// TODO: remove reports


				Projects = Projects.Where(a => a.Id != SelectedProject.Id).ToList();
			}
		}

		private void OpenProjectFolder(object parameter)
		{
			if (Directory.Exists(SelectedProject.Path))
			{
				System.Diagnostics.Process.Start("explorer.exe", SelectedProject.Path);
			}
		}

		private void SelectedItemChanged(object parameter)
		{
			if (parameter is RoutedPropertyChangedEventArgs<object> property)
			{
				if (property.NewValue is Project project)
				{
					SelectedProject = project;
				}
			}
		}

		public void Dispose()
		{
			ProjectFilesViewModel?.Dispose();
			ProjectPropertiesViewModel?.Dispose();
		}
	}
}
