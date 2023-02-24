using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using MicrosoftTranslatorProvider.Commands;
using MicrosoftTranslatorProvider.Helpers;
using MicrosoftTranslatorProvider.Interfaces;
using MicrosoftTranslatorProvider.Model;
using MicrosoftTranslatorProvider.Service;
using MicrosoftTranslatorProvider.Studio.TranslationProvider;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace MicrosoftTranslatorProvider.ViewModel
{
	public class MainWindowViewModel : BaseModel, IMainWindow
	{
		private const string ViewDetails_Provider = nameof(ProviderControlViewModel);
		private const string ViewDetails_Settings = nameof(SettingsControlViewModel);

		private readonly ISettingsControlViewModel _settingsControlViewModel;
		private readonly IProviderControlViewModel _providerControlViewModel;
		private readonly ITranslationProviderCredentialStore _credentialStore;
		private readonly LanguagePair[] _languagePairs;
		private readonly HtmlUtil _htmlUtil;
		private readonly bool _isTellMeAction;

		private ViewDetails _selectedView;
		private bool _dialogResult;
		private string _multiButtonContent;

		private ICommand _saveCommand;
		private ICommand _navigateToCommand;
		private ICommand _switchViewCommand;

		public event CloseWindowEventRaiser CloseEventRaised;
		public delegate void CloseWindowEventRaiser();

		public MainWindowViewModel(ITranslationOptions options,
								   IProviderControlViewModel providerControlViewModel,
								   ISettingsControlViewModel settingsControlViewModel,
								   ITranslationProviderCredentialStore credentialStore,
								   LanguagePair[] languagePairs,
								   HtmlUtil htmlUtil,
								   bool showSettingsView = false)
		{
			Options = options;
			_providerControlViewModel = providerControlViewModel;
			_settingsControlViewModel = settingsControlViewModel;
			_credentialStore = credentialStore;
			_languagePairs = languagePairs;
			_htmlUtil = htmlUtil;

			AvailableViews = new List<ViewDetails>
			{
				new ViewDetails
				{
					Name = nameof(ProviderControlViewModel),
					ViewModel = providerControlViewModel.ViewModel
				},
				new ViewDetails
				{
					Name = nameof(SettingsControlViewModel),
					ViewModel = settingsControlViewModel.ViewModel
				}
			};

			SwitchView(showSettingsView ? ViewDetails_Provider : ViewDetails_Settings);
			ShowProvidersPage();
		}

		public string MultiButtonContent
		{
			get => _multiButtonContent;
			set
			{
				if (_multiButtonContent == value) return;
				_multiButtonContent = value;
				OnPropertyChanged(nameof(MultiButtonContent));
			}
		}

		public ViewDetails SelectedView
		{
			get => _selectedView;
			set
			{
				_selectedView = value;
				OnPropertyChanged(nameof(SelectedView));
			}
		}

		public List<ViewDetails> AvailableViews { get; set; }
		public ITranslationOptions Options { get; set; }

		public ICommand SaveCommand => _saveCommand ??= new RelayCommand(Save);
		public ICommand NavigateToCommand => _navigateToCommand ??= new RelayCommand(NavigateTo);
		public ICommand SwitchViewCommand => _switchViewCommand ??= new RelayCommand(SwitchView);

		public bool DialogResult
		{
			get => _dialogResult;
			set
			{
				if (_dialogResult == value) return;
				_dialogResult = value;
				OnPropertyChanged(nameof(DialogResult));
			}
		}

		public bool IsWindowValid()
		{
			var microsoftOptions = ValidMicrosoftOptions();
			return microsoftOptions ? ValidSettingsPageOptions() : microsoftOptions;
		}

		private bool ValidSettingsPageOptions()
		{
			if (_settingsControlViewModel.DoPreLookup && string.IsNullOrEmpty(_settingsControlViewModel.PreLookupFileName))
			{
				ErrorHandler.HandleError(PluginResources.PreLookupEmptyMessage, "Pre-lookup");
				return false;
			}

			if (_settingsControlViewModel.DoPreLookup && !File.Exists(_settingsControlViewModel.PreLookupFileName))
			{
				ErrorHandler.HandleError(PluginResources.PreLookupWrongPathMessage, "Pre-lookup");
				return false;
			}

			if (_settingsControlViewModel.DoPostLookup && string.IsNullOrEmpty(_settingsControlViewModel.PostLookupFileName))
			{
				ErrorHandler.HandleError(PluginResources.PostLookupEmptyMessage, "Post-lookup");
				return false;
			}

			if (_settingsControlViewModel.DoPostLookup && !File.Exists(_settingsControlViewModel.PostLookupFileName))
			{
				ErrorHandler.HandleError(PluginResources.PostLookupWrongPathMessage, "Post-lookup");
				return false;
			}

			return true;
		}

		private bool ValidMicrosoftOptions()
		{
			if (string.IsNullOrEmpty(_providerControlViewModel.ClientID))
			{
				ErrorHandler.HandleError(PluginResources.ApiKeyError, "API Key");
				return false;
			}

			if (_providerControlViewModel.UseCategoryID && string.IsNullOrEmpty(_providerControlViewModel.CategoryID))
			{
				ErrorHandler.HandleError(PluginResources.CatIdError, "CategoryID");
				return false;
			}

			if (_providerControlViewModel.UsePrivateEndpoint && string.IsNullOrEmpty(_providerControlViewModel.PrivateEndpoint))
			{
				ErrorHandler.HandleError("Private endpoint can not be empty if is enabled", "Private-endpoint");
				return false;
			}

			return AreMicrosoftCredentialsValid();
		}

		private void ShowSettingsPage()
		{
			SelectedView = AvailableViews[1];
		}

		private void ShowProvidersPage()
		{
			SelectedView = AvailableViews[0];
		}

		private void Save(object window)
		{
			if (_isTellMeAction)
			{
				SetGeneralProviderOptions();
				DialogResult = true;
				CloseEventRaised?.Invoke();
				return;
			}

			if (!IsWindowValid())
			{
				return;
			}

			SetMicrosoftProviderOptions();
			SetGeneralProviderOptions();
			DeleteCredentialsIfNecessary();
			DialogResult = true;
			CloseEventRaised?.Invoke();
		}

		private void DeleteCredentialsIfNecessary()
		{
			var isMicrosoftProvider = _providerControlViewModel.SelectedTranslationOption.ProviderType == MTETranslationOptions.ProviderType.MicrosoftTranslator;
			if (isMicrosoftProvider && !Options.PersistMicrosoftCredentials)
			{
				RemoveCredentialsFromStore(new Uri(Constants.MicrosoftProviderFullScheme));
			}

			if (isMicrosoftProvider && !Options.PersistPrivateEndpoint)
			{
				RemoveCredentialsFromStore(new Uri(Constants.MicrosoftProviderPrivateEndpointScheme));
			}
		}

		private void RemoveCredentialsFromStore(Uri providerUri)
		{
			if (_credentialStore.GetCredential(providerUri) is null)
			{
				return;
			}

			_credentialStore.RemoveCredential(providerUri);
		}

		private bool AreMicrosoftCredentialsValid()
		{
			try
			{
				var apiConnecter = new ProviderConnecter(_providerControlViewModel.ClientID, _providerControlViewModel.Region?.Key, _htmlUtil, _providerControlViewModel.PrivateEndpoint);
				apiConnecter.RefreshAuthToken();

				return true;
			}
			catch (Exception e)
			{
				ErrorHandler.HandleError(e);
			}

			return false;
		}

		private void SetGeneralProviderOptions()
		{
			if (_settingsControlViewModel != null)
			{
				Options.SendPlainTextOnly = _settingsControlViewModel.SendPlainText;
				Options.ResendDrafts = _settingsControlViewModel.ReSendDraft;
				Options.UsePreEdit = _settingsControlViewModel.DoPreLookup;
				Options.PreLookupFilename = _settingsControlViewModel.PreLookupFileName;
				Options.UsePostEdit = _settingsControlViewModel.DoPostLookup;
				Options.PostLookupFilename = _settingsControlViewModel.PostLookupFileName;
				Options.CustomProviderName = _settingsControlViewModel.CustomProviderName;
				Options.UseCustomProviderName = _settingsControlViewModel.UseCustomProviderName;
			}

			if (Options != null && Options.LanguagesSupported == null)
			{
				Options.LanguagesSupported = new Dictionary<string, string>();
			}

			if (_languagePairs == null)
			{
				return;
			}

			foreach (var languagePair in _languagePairs)
			{
				Options?.LanguagesSupported.Add(languagePair.TargetCultureName, _providerControlViewModel.SelectedTranslationOption.Name);
			}
		}

		private void SetMicrosoftProviderOptions()
		{
			Options.ClientID = _providerControlViewModel.ClientID;
			Options.Region = _providerControlViewModel.Region.Key;
			Options.UseCategoryID = _providerControlViewModel.UseCategoryID;
			Options.CategoryID = _providerControlViewModel.CategoryID;
			Options.PersistMicrosoftCredentials = _providerControlViewModel.PersistMicrosoftKey;
			Options.PersistPrivateEndpoint = _providerControlViewModel.PersistPrivateEndpoint;
			Options.PrivateEndpoint = _providerControlViewModel.PrivateEndpoint;
		}

		private void NavigateTo(object parameter)
		{
			Process.Start(parameter as string);
		}

		private void SwitchView(object parameter)
		{

			try
			{
				var requestedType = parameter is not null ? parameter as string
														  : SelectedView.Name != ViewDetails_Provider ? ViewDetails_Provider
																									  : ViewDetails_Settings;
				MultiButtonContent = requestedType == ViewDetails_Provider ? "Settings" : "Provider";
				SelectedView = AvailableViews.FirstOrDefault(x => x.Name == requestedType);
			}
			catch { }
		}
	}
}