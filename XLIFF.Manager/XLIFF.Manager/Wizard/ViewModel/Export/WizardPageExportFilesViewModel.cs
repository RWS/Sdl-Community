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

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Export
{
	public class WizardPageExportFilesViewModel : WizardPageViewModelBase, IDisposable
	{
		private IList _selectedProjectFiles;
		private List<ProjectFile> _projectFiles;
		private ProjectFile _selectedProjectFile;
		private ICommand _checkAllCommand;
		private ICommand _checkSelectedComand;
		private bool _checkedAll;
		private bool _checkingAllAction;

		public WizardPageExportFilesViewModel(Window owner, object view, WizardContext wizardContext)
			: base(owner, view, wizardContext)
		{
			ProjectFiles = wizardContext.ProjectFiles;

			VerifyProjectFiles();

			LoadPage += OnLoadPage;
			LeavePage += OnLeavePage;
		}

		public ICommand CheckAllCommand => _checkAllCommand ?? (_checkAllCommand = new RelayCommand(CheckAll));

		public ICommand CheckSelectedCommand => _checkSelectedComand ?? (_checkSelectedComand = new CommandHandler(CheckSelected));

		public List<ProjectFile> ProjectFiles
		{
			get => _projectFiles ?? (_projectFiles = new List<ProjectFile>());
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

		public ProjectFile SelectedProjectFile
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
			get => _selectedProjectFiles ?? (_selectedProjectFiles = new ObservableCollection<ProjectFile>());
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
			foreach (var selectedFile in SelectedProjectFiles.Cast<ProjectFile>())
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

		private void VerifyProjectFiles()
		{
			foreach (var projectFile in ProjectFiles)
			{
				if (projectFile.Action == Enumerators.Action.Export)
				{
					var activityfile = projectFile.ProjectFileActivities.OrderByDescending(a => a.Date).FirstOrDefault(a => a.Action == Enumerators.Action.Export);

					projectFile.Status = Enumerators.Status.Warning;
					projectFile.ShortMessage = string.Format(PluginResources.Message_Exported_on_0, activityfile?.DateToString);					
				}
				else if (projectFile.Action == Enumerators.Action.Import)
				{
					var activityfile = projectFile.ProjectFileActivities.OrderByDescending(a => a.Date).FirstOrDefault(a => a.Action == Enumerators.Action.Import);

					projectFile.Status = Enumerators.Status.Warning;
					projectFile.ShortMessage = string.Format(PluginResources.Message_Imported_on_0, activityfile?.DateToString);					
				}
				else
				{
					projectFile.Status = Enumerators.Status.Ready;
					projectFile.ShortMessage = string.Empty;
					projectFile.Report = string.Empty;
				}
			}
		}

		private void ProjectFile_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (!_checkingAllAction && e.PropertyName == nameof(ProjectFile.Selected))
			{
				UpdateCheckAll();
			}

			VerifyIsValid();
		}

		private void OnLoadPage(object sender, EventArgs e)
		{
			UpdateCheckAll();
			VerifyIsValid();
		}

		private void OnLeavePage(object sender, EventArgs e)
		{
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

			LoadPage -= OnLoadPage;
			LeavePage -= OnLeavePage;
		}
	}
}
