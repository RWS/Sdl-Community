using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using NLog;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Trados.Transcreate.Actions;
using Trados.Transcreate.Commands;
using Trados.Transcreate.CustomEventArgs;
using Trados.Transcreate.Interfaces;
using Trados.Transcreate.Model;
using Trados.Transcreate.Model.ProjectSettings;
using Trados.Transcreate.Service;

namespace Trados.Transcreate.ViewModel
{
	public class ProjectsNavigationViewModel : BaseModel, IDisposable
	{
		private readonly ProjectsController _projectsController;
		private readonly EditorController _editorController;
		private readonly ProjectAutomationService _projectAutomationService;
		private List<IProject> _projects;
		private string _filterString;
		private List<IProject> _filteredProjects;
		private List<CustomerGroup> _customerGroups;
		private IProject _selectedProject;
		private IList _selectedProjects;
		private bool _isProjectSelected;
		private List<NavigationNodeState> _navigationNodeStates;
		private ICommand _clearSelectionCommand;
		private ICommand _clearFilterCommand;
		private ICommand _openProjectFolderCommand;
		private ICommand _selectedItemChanged;
		private ICommand _createBackProjectsCommand;
		private ICommand _removeBackProjectsCommand;
		private ICommand _removeProjectCommand;

		public ProjectsNavigationViewModel( ProjectsController projectsController, EditorController editorController,
			ProjectAutomationService projectAutomationService)
		{
			_projectsController = projectsController;
			_editorController = editorController;
			_projectAutomationService = projectAutomationService;
		}

		public EventHandler<ProjectSelectionChangedEventArgs> ProjectSelectionChanged;

		public ICommand RemoveProjectCommand => _removeProjectCommand ?? (_removeProjectCommand = new CommandHandler(RemoveProject));

		public ICommand ClearSelectionCommand => _clearSelectionCommand ?? (_clearSelectionCommand = new CommandHandler(ClearSelection));

		public ICommand ClearFilterCommand => _clearFilterCommand ?? (_clearFilterCommand = new CommandHandler(ClearFilter));

		public ICommand CreateBackProjectsCommand => _createBackProjectsCommand ?? (_createBackProjectsCommand = new CommandHandler(CreateBackProjects));

		public ICommand RemoveBackProjectsCommand => _removeBackProjectsCommand ?? (_removeBackProjectsCommand = new CommandHandler(RemoveBackProjects));

		public ICommand OpenProjectFolderCommand => _openProjectFolderCommand ?? (_openProjectFolderCommand = new CommandHandler(OpenProjectFolder));

		public ICommand SelectedItemChangedCommand => _selectedItemChanged ?? (_selectedItemChanged = new CommandHandler(SelectedItemChanged));

		public bool IsEnabledCreateBackProjects
		{
			get
			{
				return SelectedProject is Project && !(SelectedProject is BackTranslationProject);
			}
		}

		public bool IsEnabledDeleteProject
		{
			get
			{
				return SelectedProject is Project && !(SelectedProject is BackTranslationProject);
			}
		}

		public bool IsEnabledRemoveBackProjects { get; set; }

		public List<IProject> Projects
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
				var message = string.Format(PluginResources.StatusLabel_Selected_0,
					_selectedProjects != null
						? _selectedProjects?.Count
						: (_selectedProject != null ? 1 : 0));
				return message;
			}
		}

		public List<IProject> FilteredProjects
		{
			get => _filteredProjects;
			set
			{
				_filteredProjects = value;
				OnPropertyChanged(nameof(FilteredProjects));

				if (_filteredProjects?.Count > 0 && !_filteredProjects.Contains(SelectedProject))
				{
					SelectedProject = GetSelectedFilteredProject();
					if (_navigationNodeStates == null && SelectedProject != null)
					{
						_navigationNodeStates = new List<NavigationNodeState>();
						_navigationNodeStates.Add(new NavigationNodeState
						{
							Id = SelectedProject.Customer?.Id,
							IsSelected = false,
							IsExpanded = true
						});

						_navigationNodeStates.Add(new NavigationNodeState
						{
							Id = SelectedProject.Id,
							IsSelected = true,
							IsExpanded = false
						});
					}
				}
				else if (_filteredProjects?.Count == 0)
				{
					SelectedProject = null;
				}

				CustomerGroups = BuildGroups();
				OnPropertyChanged(nameof(StatusLabel));
			}
		}

		private IProject GetSelectedFilteredProject()
		{
			IProject selectedProject = null;
			foreach (var filteredProject in _filteredProjects)
			{
				if (filteredProject.IsSelected)
				{
					selectedProject = filteredProject;
				}
				else
				{
					foreach (var backTranslationProject in filteredProject.BackTranslationProjects.Where(p => p.IsSelected))
					{
						selectedProject = backTranslationProject;
						break;
					}
				}

				if (selectedProject != null)
				{
					break;
				}
			}

			return selectedProject;
		}

		private List<CustomerGroup> BuildGroups()
		{
			_navigationNodeStates = _customerGroups?.Count > 0 ? GetReportStates(_customerGroups) : _navigationNodeStates;

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
							Projects = new List<IProject> { filteredProject },
						};
						customerGroups.Add(customer);
					}
					else
					{
						customer.Projects.Add(filteredProject);
					}

					var customerState = _navigationNodeStates?.FirstOrDefault(a => a.Id == customer?.Customer.Id);
					if (customerState != null)
					{
						customer.IsExpanded = customerState.IsExpanded;
						customer.IsSelected = customerState.IsSelected;
					}

					var projectState = _navigationNodeStates?.FirstOrDefault(a => a.Id == filteredProject.Id);
					if (projectState != null)
					{
						filteredProject.IsExpanded = projectState.IsExpanded;
						filteredProject.IsSelected = projectState.IsSelected;

						if (!customer.IsExpanded && filteredProject.IsExpanded)
						{
							customer.IsExpanded = true;
						}
					}

					foreach (var backTranslationProject in filteredProject.BackTranslationProjects)
					{
						var backTranslationProjectState = _navigationNodeStates?.FirstOrDefault(a => a.Id == backTranslationProject.Id);
						if (backTranslationProjectState != null)
						{
							backTranslationProject.IsExpanded = backTranslationProjectState.IsExpanded;
							backTranslationProject.IsSelected = backTranslationProjectState.IsSelected;

							if (!filteredProject.IsExpanded && backTranslationProject.IsExpanded)
							{
								filteredProject.IsExpanded = true;
							}
						}
					}
				}
			}

			return customerGroups;
		}

		private List<NavigationNodeState> GetReportStates(IEnumerable<CustomerGroup> customerGroups)
		{
			var navigationNodeStates = new List<NavigationNodeState>();
			if (customerGroups == null)
			{
				return navigationNodeStates;
			}

			foreach (var customerGroup in customerGroups)
			{
				if (!navigationNodeStates.Exists(a => string.Compare(a.Id, customerGroup.Customer?.Id, StringComparison.CurrentCultureIgnoreCase) == 0))
				{
					navigationNodeStates.Add(new NavigationNodeState
					{
						Id = customerGroup.Customer?.Id,
						IsSelected = customerGroup.IsSelected,
						IsExpanded = customerGroup.IsExpanded
					});
				}

				foreach (var project in customerGroup.Projects)
				{
					if (!navigationNodeStates.Exists(a => string.Compare(a.Id, project.Id, StringComparison.CurrentCultureIgnoreCase) == 0))
					{
						navigationNodeStates.Add(new NavigationNodeState
						{
							Id = project.Id,
							IsSelected = project.IsSelected,
							IsExpanded = project.IsExpanded
						});
					}

					foreach (var backTranslationProject in project.BackTranslationProjects)
					{
						if (!navigationNodeStates.Exists(a => a.Id == backTranslationProject.Id))
						{
							navigationNodeStates.Add(new NavigationNodeState
							{
								Id = backTranslationProject.Id,
								IsSelected = backTranslationProject.IsSelected
							});
						}
					}
				}
			}

			return navigationNodeStates;
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
					ProjectFilesViewModel.ProjectFiles = _selectedProjects.Cast<Project>()
							.SelectMany(a => a.ProjectFiles).ToList();
				}

				OnPropertyChanged(nameof(StatusLabel));
			}
		}

		public IProject SelectedProject
		{
			get => _selectedProject;
			set
			{
				_selectedProject = value;
				OnPropertyChanged(nameof(SelectedProject));

				if (ProjectFilesViewModel != null)
				{
					ProjectFilesViewModel.ProjectFiles = _selectedProject?.ProjectFiles ?? new List<ProjectFile>();
				}

				ProjectSelectionChanged?.Invoke(this, new ProjectSelectionChangedEventArgs { SelectedProject = _selectedProject });

				IsProjectSelected = _selectedProject != null;

				if (SelectedProject == null || SelectedProject is BackTranslationProject)
				{
					IsEnabledRemoveBackProjects = false;
				}
				else
				{
					IsEnabledRemoveBackProjects = SelectedProject.BackTranslationProjects.Count > 0;
				}

				OnPropertyChanged(nameof(IsEnabledCreateBackProjects));
				OnPropertyChanged(nameof(IsEnabledRemoveBackProjects));
				OnPropertyChanged(nameof(IsEnabledDeleteProject));
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

		private void ClearSelection(object parameter)
		{
			SelectedProjects?.Clear();
			SelectedProject = null;
		}

		private void ClearFilter(object parameter)
		{
			FilterString = string.Empty;
		}

		private void CreateBackProjects(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<CreateBackTranslationAction>();
			action.Run();

			IsEnabledRemoveBackProjects = true;

			OnPropertyChanged(nameof(IsEnabledCreateBackProjects));
			OnPropertyChanged(nameof(IsEnabledRemoveBackProjects));
		}

		private void RemoveProject(object parameter)
		{
			if (SelectedProject == null)
			{
				return;
			}
			
			var message1 = PluginResources.Message_ActionWillRemoveAllProjectData;
			var message2 = PluginResources.Message_DoYouWantToProceed;

			var response = MessageBox.Show(message1 + Environment.NewLine + Environment.NewLine + message2,
				PluginResources.TranscreateManager_Name, MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (response == MessageBoxResult.No)
			{
				return;
			}

			var fileBasedProject = _projectsController.GetProjects()
				.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == SelectedProject.Id);

			if (fileBasedProject != null)
			{
				RemoveBackProjects(fileBasedProject);

				CloseProjectDocuments(fileBasedProject);

				_projectAutomationService.UpdateProjectSettingsBundle(fileBasedProject, null, null, null);

				var projectFolder = SelectedProject.Path;
				_projectsController.Close(fileBasedProject);

				Dispatcher.CurrentDispatcher.BeginInvoke(new Action(delegate
				{
					DeleteFolder(projectFolder);

				}), DispatcherPriority.ContextIdle);
			}
		}

		private void RemoveBackProjects(object parameter)
		{
			var message1 = PluginResources.Message_ActionWillRemoveAllBackProjectData;
			var message2 = PluginResources.Message_DoYouWantToProceed;

			var response = MessageBox.Show(message1 + Environment.NewLine + Environment.NewLine + message2,
				PluginResources.TranscreateManager_Name, MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (response == MessageBoxResult.No)
			{
				return;
			}

			var fileBasedProject = _projectsController.GetProjects()
				.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == SelectedProject.Id);

			if (fileBasedProject != null)
			{
				RemoveBackProjects(fileBasedProject);
			}
		}

		private void RemoveBackProjects(FileBasedProject fileBasedProject)
		{
			CloseProjectDocuments(fileBasedProject);

			// close the back-projects from the projects controller
			foreach (var backTranslationProject in SelectedProject.BackTranslationProjects)
			{
				var backProject = _projectsController.GetProjects()
					.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == backTranslationProject.Id);
				if (backProject != null)
				{
					CloseProjectDocuments(backProject);
					_projectsController.Close(backProject);
				}
			}

			var settingsBundle = fileBasedProject.GetSettings();
			var managerProject = settingsBundle.GetSettingsGroup<SDLTranscreateBackProjects>();

			managerProject.BackProjectsJson.Value = string.Empty;

			fileBasedProject.UpdateSettings(settingsBundle);
			fileBasedProject.Save();

			var folderPath = Path.Combine(SelectedProject.Path, "BackProjects");
			DeleteFolder(folderPath);

			//TODO remove reports

			SelectedProject.BackTranslationProjects = new List<BackTranslationProject>();
			var action = SdlTradosStudio.Application.GetAction<CreateBackTranslationAction>();
			action.Enabled = true;

			IsEnabledRemoveBackProjects = false;
			OnPropertyChanged(nameof(IsEnabledCreateBackProjects));
			OnPropertyChanged(nameof(IsEnabledRemoveBackProjects));
		}

		private static void DeleteFolder(string folderPath)
		{
			if (Directory.Exists(folderPath))
			{
				try
				{
					Directory.Delete(folderPath, true);
				}
				catch (Exception ex)
				{
					LogManager.GetCurrentClassLogger().Error(ex);
				}
			}
		}

		private void CloseProjectDocuments(FileBasedProject selectedProject)
		{
			var documents = _editorController.GetDocuments();
			foreach (var document in documents)
			{
				if (document.Project.GetProjectInfo().Id.ToString() == selectedProject.GetProjectInfo().Id.ToString())
				{
					_editorController.Save(document);
					_editorController.Close(document);
				}
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
				if (property.NewValue is IProject project)
				{
					SelectedProject = project;
				}
				else
				{
					SelectedProject = null;
				}
			}
		}

		public void Dispose()
		{
			ProjectFilesViewModel?.Dispose();
		}
	}
}
