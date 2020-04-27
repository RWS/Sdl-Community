using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.ViewModel
{
	public class ProjectFilesViewModel : INotifyPropertyChanged, IDisposable
	{
		private List<ProjectFileActionModel> _projectFileActions;

		private ProjectFileActionModel _selectedProjectFileAction;

		public ProjectFilesViewModel()
		{
			//TODO
		}

		public List<ProjectFileActionModel> ProjectFileActions
		{
			get => _projectFileActions ?? (_projectFileActions = new List<ProjectFileActionModel>());
			set
			{
				_projectFileActions = value;
				OnPropertyChanged(nameof(ProjectFileActions));
			}
		}

		public ProjectFileActionModel SelectedProjectFileAction
		{
			get => _selectedProjectFileAction;
			set
			{
				_selectedProjectFileAction = value;
				OnPropertyChanged(nameof(SelectedProjectFileAction));
			}
		}

		public void Dispose()
		{
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
