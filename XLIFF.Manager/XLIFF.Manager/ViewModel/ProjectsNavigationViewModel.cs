using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.ViewModel
{
	public class ProjectsNavigationViewModel: BaseModel, IDisposable
	{
		private string _filterString;
		private string _statusLabel;
		private List<ProjectInfoModel> _projectInfoModels;
		private ProjectInfoModel _selectedProjectInfoModel;

		public ProjectsNavigationViewModel()
		{
			//TODO
		}

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


		public List<ProjectInfoModel> ProjectInfoModels
		{
			get => _projectInfoModels;
			set
			{
				_projectInfoModels = value;
				OnPropertyChanged(nameof(ProjectInfoModels));
			}
		}

		public List<ProjectInfoModel> SelectedProjectInfoModels { get; set; }

		public ProjectInfoModel SelectedProjectInfoModel
		{
			get => _selectedProjectInfoModel;
			set
			{
				_selectedProjectInfoModel = value;
				OnPropertyChanged(nameof(SelectedProjectInfoModel));
			}
		}



		public void Dispose()
		{			
		}	
	}
}
