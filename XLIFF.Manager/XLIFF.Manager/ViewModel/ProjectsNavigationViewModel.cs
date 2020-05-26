using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.ViewModel
{
	public class ProjectsNavigationViewModel : BaseModel, IDisposable
	{
		private readonly List<ProjectModel> _projectModels;
		private string _filterString;
		private List<ProjectModel> _filteredProjectModels;
		private ProjectModel _selectedProjectModel;
		private IList _selectedProjectModels;
		private ICommand _clearSelectionCommand;
		private ICommand _clearFilterCommand;

		public ProjectsNavigationViewModel(List<ProjectModel> projectModels)
		{
			_projectModels = projectModels;
			FilteredProjectModels = _projectModels;
			FilterString = string.Empty;
		}

		public ICommand ClearSelectionCommand => _clearSelectionCommand ?? (_clearSelectionCommand = new CommandHandler(ClearSelection));

		public ICommand ClearFilterCommand => _clearFilterCommand ?? (_clearFilterCommand = new CommandHandler(ClearFilter));

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

				FilteredProjectModels = string.IsNullOrEmpty(_filterString)
					? _projectModels
					: _projectModels.Where(a => a.Name.ToLower().Contains(_filterString.ToLower())).ToList();
			}
		}

		public string StatusLabel
		{
			get
			{
				var message = string.Format(PluginResources.StatusLabel_Selected_0, _selectedProjectModels?.Count);
				return message;
			}
		}

		public List<ProjectModel> FilteredProjectModels
		{
			get => _filteredProjectModels;
			set
			{
				_filteredProjectModels = value;
				OnPropertyChanged(nameof(FilteredProjectModels));

				if (_filteredProjectModels?.Count > 0 && !_filteredProjectModels.Contains(SelectedProjectModel))
				{
					SelectedProjectModel = _filteredProjectModels[0];
				}
				else if (_filteredProjectModels?.Count == 0)
				{
					SelectedProjectModel = null;
				}

				OnPropertyChanged(nameof(StatusLabel));
			}
		}

		public IList SelectedProjectModels
		{
			get => _selectedProjectModels;
			set
			{
				_selectedProjectModels = value;
				OnPropertyChanged(nameof(SelectedProjectModels));

				if (ProjectFilesViewModel != null && _selectedProjectModels != null)
				{
					ProjectFilesViewModel.ProjectFiles =
						_selectedProjectModels.Cast<ProjectModel>().SelectMany(a => a.ProjectFileModels).ToList();
				}

				OnPropertyChanged(nameof(StatusLabel));
			}
		}

		public ProjectModel SelectedProjectModel
		{
			get => _selectedProjectModel;
			set
			{
				_selectedProjectModel = value;
				OnPropertyChanged(nameof(SelectedProjectModel));

				if (ProjectFilesViewModel != null && _selectedProjectModel != null)
				{
					ProjectFilesViewModel.ProjectFiles = _selectedProjectModel.ProjectFileModels;
				}
			}
		}

		private void ClearSelection(object parameter)
		{
			SelectedProjectModels.Clear();
			SelectedProjectModel = null;
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
