using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Input;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.Community.BeGlobalV4.Provider.Ui;
using Sdl.LanguagePlatform.Core;
using static Sdl.Community.BeGlobalV4.Provider.Helpers.Enums;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class BeGlobalWindowViewModel : BaseViewModel
	{
		private readonly LanguagePair[] _languagePairs;
		private readonly BeGlobalWindow _mainWindow;
		private readonly NormalizeSourceTextHelper _normalizeSourceTextHelper;
		private ICommand _okCommand;
		private bool _reSendChecked;
		private TranslationModel _selectedModel;
		private BeGlobalLoginOptions _selectedLoginOption;
		private string _clientIdValue;
		private string _clientSecretValue;
		private string _loginMethod;

		public BeGlobalWindowViewModel(BeGlobalWindow mainWindow, BeGlobalTranslationOptions options, LanguagePair[] languagePairs)
		{
			Options = options;
			_mainWindow = mainWindow;
			_languagePairs = languagePairs;
			_normalizeSourceTextHelper = new NormalizeSourceTextHelper();
			TranslationOptions = new ObservableCollection<TranslationModel>();
			_loginMethod = Enums.LoginOptions.APICredentials.ToString();

			var beGlobalTranslator = new BeGlobalV4Translator(Options?.Model);
			var accountId = beGlobalTranslator.GetUserInformation();

			var subscriptionInfo = beGlobalTranslator.GetLanguagePairs(accountId.ToString());
			if (Options != null)
			{
				ReSendChecked = options.ResendDrafts;
			}
			SetLoginOptions();
			GetEngineModels(subscriptionInfo);
			SetEngineModel();
		}

		public ICommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(Ok));
		public BeGlobalTranslationOptions Options { get; set; }

		public bool ReSendChecked
		{
			get => _reSendChecked;
			set
			{
				_reSendChecked = value;
				if (Options?.Model != null)
				{
					Options.ResendDrafts = value;
				}
				OnPropertyChanged(nameof(ReSendChecked));
			}
		}

		public TranslationModel SelectedModelOption
		{
			get => _selectedModel;
			set
			{
				_selectedModel = value;
				if (Options?.Model != null)
				{
					SetOptions(value);
				}
				OnPropertyChanged(nameof(SelectedModelOption));
			}
		}

		public string ClientIdValue
		{
			get => _clientIdValue;
			set
			{
				_clientIdValue = value;			
				OnPropertyChanged(nameof(ClientIdValue));
			}
		}

		public string ClientSecretValue
		{
			get => _clientSecretValue;
			set
			{
				_clientSecretValue = value;
				OnPropertyChanged(nameof(ClientSecretValue));
			}
		}

		public ObservableCollection<BeGlobalLoginOptions> LoginOptions { get; set; }
	    public BeGlobalLoginOptions SelectedLoginOption
		{
			get => _selectedLoginOption;
			set
			{
				_selectedLoginOption = value;
				LoginMethod = _selectedLoginOption.LoginOption;
				OnPropertyChanged(nameof(SelectedLoginOption));
			}
		}

		// LoginMethod is used to display/hide the ClientId,ClientSecret fields based on which authentication mode is selected
		public string LoginMethod
		{
			get => _loginMethod;
			set
			{
				if (_loginMethod == value)
				{
					return;
				}
				_loginMethod = value;
				OnPropertyChanged(nameof(LoginMethod));
			}
		}

		public ObservableCollection<TranslationModel> TranslationOptions { get; set; }

		private void GetEngineModels(SubscriptionInfo subscriptionInfo)
		{
			var sourceLanguage = _normalizeSourceTextHelper.GetCorespondingLangCode(_languagePairs?[0].SourceCulture);
			var pairsWithSameSource = subscriptionInfo.LanguagePairs.Where(l => l.SourceLanguageId.Equals(sourceLanguage)).ToList();
			if (_languagePairs?.Count() > 0)
			{
				foreach (var languagePair in _languagePairs)
				{
					var targetLanguage = _normalizeSourceTextHelper.GetCorespondingLangCode(languagePair.TargetCulture);

					var serviceLanguagePairs = pairsWithSameSource.Where(t => t.TargetLanguageId.Equals(targetLanguage)).ToList();

					foreach (var serviceLanguagePair in serviceLanguagePairs)
					{
						var existingTranslationModel = TranslationOptions.FirstOrDefault(e => e.Model.Equals(serviceLanguagePair.Model));
						TranslationModel newTranslationModel = null;
						if (existingTranslationModel == null)
						{
							newTranslationModel = new TranslationModel
							{
								Model = serviceLanguagePair.Model,
								DisplayName = serviceLanguagePair.DisplayName,
							};
							TranslationOptions.Add(newTranslationModel);
						}

						(existingTranslationModel ?? newTranslationModel).LanguagesSupported.Add(languagePair.TargetCulture.Name, serviceLanguagePair.Name);
					}
				}
			}
		}

		private void Ok(object parameter)
		{
			WindowCloser.SetDialogResult(_mainWindow, true);
			_mainWindow.Close();
		}

		private void SetEngineModel()
		{
			if (Options?.Model == null)
			{
				if (TranslationOptions?.Count > 0)
				{
					SelectedModelOption = TranslationOptions?[0];
					if (Options != null)
					{
						SetOptions(TranslationOptions?[0]);
					}
				}
			}
			else
			{
				var mtModel = TranslationOptions?.FirstOrDefault(m => m.Model.Equals(Options.Model));
				if (mtModel != null)
				{
					var selectedModelIndex = TranslationOptions.IndexOf(mtModel);
					SelectedModelOption = TranslationOptions[selectedModelIndex];
				}
			}
		}

		private void SetLoginOptions()
		{
			LoginOptions = new ObservableCollection<BeGlobalLoginOptions>();
			foreach (LoginOptions enumVal in Enum.GetValues(typeof(LoginOptions)))
			{
				var displayName = Enums.GetDisplayName(enumVal);
				displayName = !string.IsNullOrEmpty(displayName) ? displayName : enumVal.ToString();

				var beGlobalLoginOption = new BeGlobalLoginOptions { LoginOption = displayName };
				LoginOptions.Add(beGlobalLoginOption);
			}
			SelectedLoginOption = new BeGlobalLoginOptions { LoginOption = (Enums.GetDisplayName(Enums.LoginOptions.APICredentials)) };
		}

		private void SetOptions(TranslationModel translationModel)
		{
			Options.Model = translationModel.Model;
			Options.DisplayName = translationModel.DisplayName;
			Options.LanguagesSupported = translationModel.LanguagesSupported;
		}
	}
}