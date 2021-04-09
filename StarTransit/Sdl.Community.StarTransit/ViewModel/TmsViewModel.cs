using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.StarTransit.Command;
using Sdl.Community.StarTransit.Interface;
using Sdl.Community.StarTransit.Shared.Models;

namespace Sdl.Community.StarTransit.ViewModel
{
	public class TmsViewModel : WizardViewModelBase
	{
		private int _currentPageNumber;
		private string _displayName;
		private string _tooltip;
		private bool _isNextEnabled;
		private bool _isPreviousEnabled;
		private bool _isValid;
		private readonly IWizardModel _wizardModel;
		private LanguagePair _selectedLanguagePair;
		private ICommand _selectTmCommand;
		private readonly IOpenFileDialogService _fileDialogService;

		public TmsViewModel(IWizardModel wizardModel,IOpenFileDialogService fileDialogService, object view) : base(view)
		{
			_currentPageNumber = 2;
			_displayName = PluginResources.Wizard_TM_DisplayName;
			_tooltip = PluginResources.Wizard_Tms_Tooltip;
			_wizardModel = wizardModel;
			IsPreviousEnabled = true;
			_fileDialogService = fileDialogService;
			PropertyChanged += TmsViewModelChanged;
		}

		public List<LanguagePair> LanguagePairsTmOptions
		{
			get => _wizardModel.PackageModel.Result?.LanguagePairs;
			set
			{
				_wizardModel.PackageModel.Result.LanguagePairs = value;
				OnPropertyChanged(nameof(LanguagePairsTmOptions));
			}
		}

		public LanguagePair SelectedLanguagePair
		{
			get => _selectedLanguagePair;
			set
			{
				_selectedLanguagePair = value;
				OnPropertyChanged(nameof(SelectedLanguagePair));
			}
		}

		public ICommand SelectTmCommand => _selectTmCommand ?? (_selectTmCommand = new CommandHandler(SelectTm));


		private void SelectTm(object selectedLanguageOptions)
		{
			var selectedTm = _fileDialogService.ShowDialog("TM Files (.sdltm)|*.sdltm");
			var language = (LanguagePair) selectedLanguageOptions;
			language.TmPath = selectedTm;
			language.TmName = Path.GetFileNameWithoutExtension(selectedTm);
			OnPropertyChanged(nameof(LanguagePairsTmOptions));
		}

		public override string DisplayName
		{
			get => _displayName;
			set
			{
				if (_displayName == value)
				{
					return;
				}

				_displayName = value;
				OnPropertyChanged(nameof(DisplayName));
			}
		}

		public override string Tooltip
		{
			get => _tooltip;
			set
			{
				if (_tooltip == value) return;
				_tooltip = value;
				OnPropertyChanged(Tooltip);
			}
		}
		public override bool IsValid
		{
			get => _isValid;
			set
			{
				if (_isValid == value)
					return;

				_isValid = value;
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public int CurrentPageNumber
		{
			get => _currentPageNumber;
			set
			{
				_currentPageNumber = value;
				OnPropertyChanged(nameof(CurrentPageNumber));
			}
		}

		public bool IsNextEnabled
		{
			get => _isNextEnabled;
			set
			{
				if (_isNextEnabled == value)
					return;

				_isNextEnabled = value;
				OnPropertyChanged(nameof(IsNextEnabled));
			}
		}

		public bool IsPreviousEnabled
		{
			get => _isPreviousEnabled;
			set
			{
				if (_isPreviousEnabled == value)
					return;

				_isPreviousEnabled = value;
				OnPropertyChanged(nameof(IsPreviousEnabled));
			}
		}
		public override bool OnChangePage(int position, out string message)
		{
			message = string.Empty;

			var pagePosition = PageIndex - 1;
			if (position == pagePosition)
			{
				return false;
			}

			if (!IsValid && position > pagePosition)
			{
				message = PluginResources.Wizard_ValidationMessage;
				return false;
			}
			return true;
		}
		private void TmsViewModelChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(CurrentPageChanged)) return;
			if (!IsCurrentPage) return;
			foreach (var languagePairsTmOption in LanguagePairsTmOptions)
			{
				languagePairsTmOption.SelectTmCommand = SelectTmCommand;
			}

			//SelectedLanguagePair = LanguagePairsTmOptions?[0];
		}
	}
}
