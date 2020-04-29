using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.ViewModel
{
	public class ProjectFilesViewModel : BaseModel, IDisposable
	{
		private List<ProjectFileActionModel> _projectFileActions;
		private List<ProjectFileActionModel> _selectedProjectFileActions;
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

		public IList SelectedProjectFileActions { get; set; }

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
	}
}
