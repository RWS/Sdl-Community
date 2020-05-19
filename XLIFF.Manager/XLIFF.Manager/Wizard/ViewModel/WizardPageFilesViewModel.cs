using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel
{
	public class WizardPageFilesViewModel: WizardPageViewModelBase
	{
		private IList _selectedProjectFileActions;
		private List<ProjectFileActionModel> _projectFileActions;
		private ProjectFileActionModel _selectedProjectFileAction;
		private ICommand _clearSelectionCommand;
		private bool _isValid;
			
		public WizardPageFilesViewModel(object view, TransactionModel transactionModel) : base(view, transactionModel)
		{			
			ProjectFileActions = transactionModel.ProjectFileActions;
			VerifyIsValid();
		}

		public ICommand ClearSelectionCommand => _clearSelectionCommand ?? (_clearSelectionCommand = new CommandHandler(ClearSelection));

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
			}
		}

		public Enumerators.Action Action { get; set; }

		public string StatusLabel
		{
			get
			{
				var message = string.Format(PluginResources.StatusLabel_Projects_0_Files_1_Selected_2,
					_projectFileActions.Select(a => a.ProjectModel).Distinct().Count(),
					_projectFileActions?.Count,
					_selectedProjectFileActions?.Count);
				return message;
			}
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
			IsValid = true;
		}
	}
}
