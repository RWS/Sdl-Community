using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel
{
	public class WizardPageExportFilesViewModel : WizardPageViewModelBase, IDisposable
	{	
		private IList _selectedProjectFiles;		
		private List<ProjectFileModel> _projectFiles;
		private ProjectFileModel _selectedProjectFile;
		private ICommand _checkAllCommand;
		private ICommand _checkSelectedComand;
		private bool _checkedAll;
		private bool _checkingAllAction;

		public WizardPageExportFilesViewModel(Window owner, object view, WizardContextModel wizardContext) : base(owner, view, wizardContext)
		{		
			ProjectFiles = wizardContext.ProjectFileModels;
			PropertyChanged += WizardPageExportFilesViewModel_PropertyChanged;			
		}
		
		public ICommand CheckAllCommand => _checkAllCommand ?? (_checkAllCommand = new RelayCommand(CheckAll));

		public ICommand CheckSelectedCommand => _checkSelectedComand ?? (_checkSelectedComand = new CommandHandler(CheckSelected));

		public List<ProjectFileModel> ProjectFiles
		{
			get => _projectFiles ?? (_projectFiles = new List<ProjectFileModel>());
			set
			{
				if (_projectFiles != null)
				{
					foreach (var projectFile in _projectFiles)
					{
						projectFile.PropertyChanged -= ProjectFile_PropertyChanged;
					}
				}

				_projectFiles = value;

				if (_projectFiles != null)
				{
					foreach (var projectFile in _projectFiles)
					{
						projectFile.PropertyChanged += ProjectFile_PropertyChanged;
					}
				}

				OnPropertyChanged(nameof(ProjectFiles));
				OnPropertyChanged(nameof(StatusLabel));
			}
		}

		public ProjectFileModel SelectedProjectFile
		{
			get => _selectedProjectFile;
			set
			{
				_selectedProjectFile = value;
				OnPropertyChanged(nameof(SelectedProjectFile));
			}
		}

		public IList SelectedProjectFiles
		{
			get => _selectedProjectFiles ?? (_selectedProjectFiles = new ObservableCollection<ProjectFileModel>());
			set
			{
				_selectedProjectFiles = value;
				OnPropertyChanged(nameof(SelectedProjectFiles));
			}
		}

		public Enumerators.Action Action { get; set; }

		public bool CheckedAll
		{
			get => _checkedAll;
			set
			{
				_checkedAll = value;
				OnPropertyChanged(nameof(CheckedAll));
			}
		}

		private void CheckAll()
		{
			try
			{
				_checkingAllAction = true;

				var value = CheckedAll;
				foreach (var file in ProjectFiles)
				{
					file.Selected = value;
				}

				VerifyIsValid();
				OnPropertyChanged(nameof(CheckedAll));
				OnPropertyChanged(nameof(StatusLabel));
			}
			finally
			{
				_checkingAllAction = false;
			}
		}

		public string StatusLabel
		{
			get
			{
				var message = string.Format(PluginResources.StatusLabel_Files_0_Selected_1,
					_projectFiles?.Count,
					_projectFiles?.Count(a => a.Selected));
				return message;
			}
		}

		private void UpdateCheckAll()
		{
			CheckedAll = ProjectFiles.Count == ProjectFiles.Count(a => a.Selected);
			OnPropertyChanged(nameof(StatusLabel));
		}

		private void CheckSelected(object parameter)
		{
			if (SelectedProjectFiles == null)
			{
				return;
			}

			var isChecked = Convert.ToBoolean(parameter);
			foreach (var selectedFile in SelectedProjectFiles.Cast<ProjectFileModel>())
			{
				selectedFile.Selected = isChecked;
			}

			UpdateCheckAll();
			VerifyIsValid();
		}


		public override string DisplayName => PluginResources.PageName_Files;

		public override bool IsValid { get; set; }

		public void VerifyIsValid()
		{
			IsValid = ProjectFiles.Count(a => a.Selected) > 0;
		}

		private void ProjectFile_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (!_checkingAllAction && e.PropertyName == nameof(ProjectFileModel.Selected))
			{
				UpdateCheckAll();
			}

			VerifyIsValid();
		}

		private void WizardPageExportFilesViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(CurrentPageChanged))
			{
				if (IsCurrentPage)
				{
					LoadView();
				}
				else
				{
					LeaveView();
				}
			}
		}

		private void LeaveView()
		{			
		}

		private void LoadView()
		{
			UpdateCheckAll();
			VerifyIsValid();
		}

		public void Dispose()
		{
			if (ProjectFiles != null)
			{
				foreach (var projectFile in ProjectFiles)
				{
					projectFile.PropertyChanged -= ProjectFile_PropertyChanged;
				}
			}

			SelectedProjectFile?.Dispose();
			PropertyChanged -= WizardPageExportFilesViewModel_PropertyChanged;			
		}
	}
}
