using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.ViewModel
{
	public class ProjectsNavigationViewModel : BaseModel, IDisposable
	{
		private readonly List<ProjectModel> _projectInfoModels;
		private string _filterString;
		private string _statusLabel;
		private List<ProjectModel> _filteredProjectModels;
		private ProjectModel _selectedProjectModel;
		private IList _selectedProjectModels;

		public ProjectsNavigationViewModel(List<ProjectModel> projectInfoModels)
		{
			_projectInfoModels = projectInfoModels;
			FilteredProjectModels = _projectInfoModels;
			FilterString = string.Empty;
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

				FilteredProjectModels = string.IsNullOrEmpty(_filterString)
					? _projectInfoModels
					: _projectInfoModels.Where(a => a.Name.ToLower().Contains(_filterString)).ToList();
			}
		}

		public string StatusLabel
		{
			get => _statusLabel;
			set
			{
				if (_statusLabel == value)
				{
					return;
				}

				_statusLabel = value;
				OnPropertyChanged(nameof(StatusLabel));
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
					ProjectFilesViewModel.ProjectFileActions =
						_selectedProjectModels.Cast<ProjectModel>().SelectMany(a => a.ProjectFileActionModels).ToList();
				}
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
					ProjectFilesViewModel.ProjectFileActions = _selectedProjectModel.ProjectFileActionModels;
				}
			}
		}

		public void Dispose()
		{
		}
	}
}
