using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel
{
	public class WizardPageExportFilesViewModel : WizardPageViewModelBase
	{
		private IList _selectedProjectFileActions;
		private List<ProjectFileActionModel> _projectFileActions;
		private ProjectFileActionModel _selectedProjectFileAction;
		private ICommand _clearSelectionCommand;
		private ICommand _checkAllCommand;
		private bool _isValid;
		private bool _checkedAll;

		public WizardPageExportFilesViewModel(object view, TransactionModel transactionModel) : base(view, transactionModel)
		{
			ProjectFileActions = transactionModel.ProjectFileActions;
			VerifyIsValid();
		}

		public ICommand ClearSelectionCommand => _clearSelectionCommand ?? (_clearSelectionCommand = new CommandHandler(ClearSelection));

		public ICommand CheckAllCommand => _checkAllCommand ?? (_checkAllCommand = new RelayCommand(CheckAll));

		public List<ProjectFileActionModel> ProjectFileActions
		{
			get => _projectFileActions ?? (_projectFileActions = new List<ProjectFileActionModel>());
			set
			{
				_projectFileActions = value;
				OnPropertyChanged(nameof(ProjectFileActions));
				OnPropertyChanged(nameof(StatusLabel));
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

		public IList SelectedProjectFileActions
		{
			get => _selectedProjectFileActions;
			set
			{
				_selectedProjectFileActions = value;

				OnPropertyChanged(nameof(SelectedProjectFileActions));
				OnPropertyChanged(nameof(StatusLabel));

				foreach (var projectFileAction in ProjectFileActions)
				{
					projectFileAction.Selected = _selectedProjectFileActions.Contains(projectFileAction);
				}

				UpdateCheckAll();
				VerifyIsValid();
			}
		}

		public Enumerators.Action Action { get; set; }

		public bool CheckedAll
		{
			get { return _checkedAll; }
			set
			{
				_checkedAll = value;
				OnPropertyChanged(nameof(CheckedAll));
			}
		}

		private void CheckAll()
		{
			if (!CheckedAll)
			{
				_selectedProjectFileActions.Clear();
			}
			else
			{
				foreach (var projectFileActionModel in ProjectFileActions)
				{
					if (!_selectedProjectFileActions.Contains(projectFileActionModel))
					{
						_selectedProjectFileActions.Add(projectFileActionModel);
					}
				}
			}

			OnPropertyChanged(nameof(SelectedProjectFileActions));
			OnPropertyChanged(nameof(CheckedAll));
			VerifyIsValid();
		}

		public string StatusLabel
		{
			get
			{
				var message = string.Format(PluginResources.StatusLabel_Files_0_Selected_1,
					_projectFileActions?.Count,
					_selectedProjectFileActions?.Count);
				return message;
			}
		}

		private void UpdateCheckAll()
		{
			var count = ProjectFileActions.Count();
			CheckedAll = count == SelectedProjectFileActions.Count;

			OnPropertyChanged(nameof(StatusLabel));
		}

		private void ClearSelection(object parameter)
		{
			SelectedProjectFileActions.Clear();
			SelectedProjectFileAction = null;
		}

		public override string DisplayName => "Files";

		public override bool IsValid
		{
			get => _isValid;
			set => _isValid = value;
		}

		private void VerifyIsValid()
		{
			IsValid = SelectedProjectFileActions?.Count > 0;
		}
	}
}
