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
		private readonly List<ProjectModel> _projectInfoModels;
		private string _filterString;
		private List<ProjectModel> _filteredProjectModels;
		private ProjectModel _selectedProjectModel;
		private IList _selectedProjectModels;
		private ICommand _clearSelectionCommand;
		private ICommand _clearFilterCommand;
		
		public ProjectsNavigationViewModel(List<ProjectModel> projectInfoModels)
		{
			_projectInfoModels = projectInfoModels;
			FilteredProjectModels = _projectInfoModels;
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
					? _projectInfoModels
					: _projectInfoModels.Where(a => a.Name.ToLower().Contains(_filterString)).ToList();
			}
		}

		public string StatusLabel
		{
			get
			{
				var message = "Selected: " + _selectedProjectModels?.Count;
				return message;
			}			
		}

		//public List<ClientModel> ClientModels
		//{
		//	get
		//	{
		//		var clientModels = new List<ClientModel>();

		//		foreach (var filteredProjectModel in FilteredProjectModels)
		//		{
		//			var clientModel = clientModels.FirstOrDefault(a => a.Name == filteredProjectModel.ClientName);
		//			if (clientModel == null)
		//			{
		//				clientModels.Add(new ClientModel{Name = filteredProjectModel.ClientName, ProjectModels = new List<ProjectModel>{filteredProjectModel}});
		//			}
		//			else
		//			{
		//				clientModel.ProjectModels.Add(filteredProjectModel);
		//			}
		//		}

		//		return clientModels;
		//	}
		//}

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
					ProjectFilesViewModel.ProjectFileActions =
						_selectedProjectModels.Cast<ProjectModel>().SelectMany(a => a.ProjectFileActionModels).ToList();
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
					ProjectFilesViewModel.ProjectFileActions = _selectedProjectModel.ProjectFileActionModels;
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
