using Sdl.LanguagePlatform.TranslationMemory;
using SDLTM.Import.Model;

namespace SDLTM.Import.ViewModel
{
	public class ImportSettingsViewModel: WizardViewModelBase
	{
		private int _currentPageNumber;
		private string _tooltip;
		private string _displayName;
		private bool _isValid;
		private bool _isNextEnabled;
		private bool _isPreviousEnabled;
		private readonly WizardModel _wizardModel;

		public ImportSettingsViewModel(WizardModel wizardModel, object view) : base(view)
		{
			_wizardModel = wizardModel;
			_currentPageNumber = 4;
			_isPreviousEnabled = true;
			_isNextEnabled = true;
			_isValid = true;
			_wizardModel.ImportSettings.UseBilingualInfo = true;
			_wizardModel.ImportSettings.TuUpdateMode = ImportSettings.TUUpdateMode.AddNew;
			_displayName = PluginResources.Wizard_ImportSettingsDisplayName;
			_tooltip = PluginResources.Wizard_ImportSettingsDisplayName;
		}

		public ImportSettings.TUUpdateMode TuUpdateMode
		{
			get => _wizardModel.ImportSettings.TuUpdateMode;
			set
			{
				if (_wizardModel.ImportSettings.TuUpdateMode == value) return;
				_wizardModel.ImportSettings.TuUpdateMode = value;
				OnPropertyChanged(nameof(TuUpdateMode));
			}
		}

		public bool ImportPlain
		{
			get => _wizardModel.ImportSettings.ImportPlain;
			set
			{
				if (_wizardModel.ImportSettings.ImportPlain == value) return;
				_wizardModel.ImportSettings.ImportPlain = value;
				OnPropertyChanged(nameof(ImportPlain));
			}
		}

		public bool ExcludeVariantsForXliff
		{
			get => _wizardModel.ImportSettings.ExcludeVariantsForXliff;
			set
			{
				if (_wizardModel.ImportSettings.ExcludeVariantsForXliff == value) return;
				_wizardModel.ImportSettings.ExcludeVariantsForXliff = value;
				OnPropertyChanged(nameof(ExcludeVariantsForXliff));
			}
		}

		public bool ExcludeVariantsForTmx
		{
			get => _wizardModel.ImportSettings.ExcludeVariantsForTmx;
			set
			{
				if (_wizardModel.ImportSettings.ExcludeVariantsForTmx == value) return;
				_wizardModel.ImportSettings.ExcludeVariantsForTmx = value;
				OnPropertyChanged(nameof(ExcludeVariantsForTmx));
			}
		}

		public bool UseBilingualInfo
		{
			get => _wizardModel.ImportSettings.UseBilingualInfo;
			set
			{
				if (_wizardModel.ImportSettings.UseBilingualInfo == value) return;
				_wizardModel.ImportSettings.UseBilingualInfo = value;
				OnPropertyChanged(nameof(UseBilingualInfo));
			}
		}

		public override bool OnChangePage(int position, out string message)
		{
			message = string.Empty;

			var pagePosition = PageIndex - 1;
			return position != pagePosition;
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
	}
}
