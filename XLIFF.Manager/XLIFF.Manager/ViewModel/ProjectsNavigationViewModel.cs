using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Actions.Export;
using Sdl.Community.XLIFF.Manager.Actions.Import;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.CustomEventArgs;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Community.XLIFF.Manager.Model.ProjectSettings;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.ViewModel
{
	public class ProjectsNavigationViewModel : BaseModel, IDisposable
	{
		private readonly ProjectsController _projectsController;
		private List<Project> _projects;
		private string _filterString;
		private List<Project> _filteredProjects;
		private Project _selectedProject;
		private IList _selectedProjects;
		private bool _isProjectSelected;
		private ICommand _clearSelectionCommand;
		private ICommand _clearFilterCommand;
		private ICommand _removeProjectDataCommand;
		private ICommand _openProjectFolderCommand;
		private ICommand _importFilesCommand;
		private ICommand _exportFilesCommand;
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
			var action = SdlTradosStudio.Application.GetAction<ImportFromXLIFFAction>();			
			action.LaunchWizard();
		}

		private void ExportFiles(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<ExportToXLIFFAction>();
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
				PluginResources.XLIFFManager_Name, MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (response == MessageBoxResult.No)
			{
				return;
			}

			var selectedProject = _projectsController.GetProjects()
				.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == SelectedProject.Id);

			if (selectedProject != null)
			{
				var settingsBundle = selectedProject.GetSettings();
				var xliffManagerProject = settingsBundle.GetSettingsGroup<XliffManagerProject>();

				xliffManagerProject.ProjectFilesJson.Value = string.Empty;

				selectedProject.UpdateSettings(settingsBundle);
				selectedProject.Save();

				var xliffFolderPath = Path.Combine(SelectedProject.Path, "XLIFF.Manager");
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

		public void Dispose()
		{
			ProjectFilesViewModel?.Dispose();			
		}
	}
}
