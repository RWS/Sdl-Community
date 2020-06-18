using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.ViewModel
{
	public class ProjectFileActivityViewModel: BaseModel, IDisposable
	{
		private List<ProjectFileActivity> _projectFileActivities;
		private ProjectFileActivity _selectedProjectFileActivity;
		private IList _selectedProjectFileActivities;
		private bool _isProjectFileSelected;
		private ICommand _openFolderCommand;

		public ProjectFileActivityViewModel(List<ProjectFileActivity> projectFileActivities)
		{
			ProjectFileActivities = projectFileActivities;
			SelectedProjectFileActivity = ProjectFileActivities?.Count > 0 ? ProjectFileActivities[0] : null;
			SelectedProjectFileActivities = new List<ProjectFileActivity> { SelectedProjectFileActivity };
		}

		public ICommand OpenFolderCommand => _openFolderCommand ?? (_openFolderCommand = new CommandHandler(OpenFolder));


		public List<ProjectFileActivity> ProjectFileActivities
		{
			get => _projectFileActivities ?? (_projectFileActivities = new List<ProjectFileActivity>());
			set
			{
				_projectFileActivities = value;
				OnPropertyChanged(nameof(ProjectFileActivities));
			}  
		}

		public IList SelectedProjectFileActivities
		{
			get => _selectedProjectFileActivities;
			set
			{
				_selectedProjectFileActivities = value;
				OnPropertyChanged(nameof(SelectedProjectFileActivities));
			}
		}

		public ProjectFileActivity SelectedProjectFileActivity
		{
			get => _selectedProjectFileActivity;
			set
			{
				_selectedProjectFileActivity = value;
				OnPropertyChanged(nameof(SelectedProjectFileActivity));

				IsProjectFileSelected = _selectedProjectFileActivity != null;
			}
		}

		public bool IsProjectFileSelected
		{
			get => _isProjectFileSelected;
			set
			{
				if (_isProjectFileSelected == value)
				{
					return;
				}

				_isProjectFileSelected = value;
				OnPropertyChanged(nameof(IsProjectFileSelected));
			}
		}

		private void OpenFolder(object parameter)
		{
			System.Diagnostics.Process.Start("explorer.exe", SelectedProjectFileActivity.Path);
		}

		public void Dispose()
		{
		}		
	}
}
