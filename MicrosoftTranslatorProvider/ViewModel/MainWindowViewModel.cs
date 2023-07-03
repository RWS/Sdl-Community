using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Windows.Input;
using System.Xml.Linq;
using MicrosoftTranslatorProvider.Commands;
using MicrosoftTranslatorProvider.Helpers;
using MicrosoftTranslatorProvider.Interface;
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
		private const string ViewDetails_PrivateEndpoint = nameof(PrivateEndpointViewModel);

		private readonly ISettingsControlViewModel _settingsControlViewModel;
		private readonly IProviderControlViewModel _providerControlViewModel;
		private readonly IPrivateEndpointViewModel _privateEndpointViewModel;
		private readonly ITranslationProviderCredentialStore _credentialStore;
		private readonly LanguagePair[] _languagePairs;
		private readonly HtmlUtil _htmlUtil;
		private readonly bool _showSettingsViews;

		private ViewDetails _selectedView;
		private bool _dialogResult;
		private string _multiButtonContent;

		private ICommand _saveCommand;
		private ICommand _navigateToCommand;
		private ICommand _switchViewCommand;

		public event CloseWindowEventRaiser CloseEventRaised;
		public delegate void CloseWindowEventRaiser();

		public MainWindowViewModel(ITranslationOptions options,
								   ITranslationProviderCredentialStore credentialStore,
								   LanguagePair[] languagePairs,
								   RegionsProvider regionsProvider,
								   HtmlUtil htmlUtil,
								   bool showSettingsView = false)
		{
			Options = options;
			_providerControlViewModel = new ProviderControlViewModel(options, regionsProvider, languagePairs);
			_settingsControlViewModel = new SettingsControlViewModel(options, credentialStore, new OpenFileDialogService(), false);
			_privateEndpointViewModel = new PrivateEndpointViewModel();
			_credentialStore = credentialStore;
			_languagePairs = languagePairs;
			_htmlUtil = htmlUtil;
			_showSettingsViews = showSettingsView;

			AvailableViews = new List<ViewDetails>
			{
				new ViewDetails
				{
					Name = nameof(ProviderControlViewModel),
					ViewModel = _providerControlViewModel.ViewModel
				},
				new ViewDetails
				{
					Name = nameof(SettingsControlViewModel),
					ViewModel = _settingsControlViewModel.ViewModel
				},
				new ViewDetails()
				{
					Name = nameof(PrivateEndpointViewModel),
					ViewModel = _privateEndpointViewModel.ViewModel
				}
			};

			Endpoints = new List<string>() { "Microsoft", "Private Endpoint" };
			SelectedEndpoint = Endpoints.First();
			SwitchView(showSettingsView ? ViewDetails_Provider : ViewDetails_Settings);
			ShowProvidersPage();
			SetCredentialsOnUI();
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
			var settingsAreValid = ValidSettingsPageOptions();
			return UsePrivateEndpoint ? settingsAreValid && ValidPrivateEndpointOptions()
					                  : settingsAreValid && ValidMicrosoftOptions();		
		}

		private bool ValidPrivateEndpointOptions()
		{
			return !string.IsNullOrEmpty(_privateEndpointViewModel.Endpoint);
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
			if (!IsWindowValid())
			{
				return;
			}

			SetMicrosoftProviderOptions();
			SetGeneralProviderOptions();
			SaveCredentials();
			DialogResult = true;
			CloseEventRaised?.Invoke();
		}

		private bool AreMicrosoftCredentialsValid()
		{
			try
			{
				if (Options.UsePrivateEndpoint)
				{
					return true;
				}

				var apiConnecter = new MicrosoftApi(_providerControlViewModel.ClientID, _providerControlViewModel.Region?.Key, _htmlUtil);
				apiConnecter.RefreshAuthToken();

				return true;
			}
			catch (Exception e)
			{
				const string AccessDeniedMessage = "401 (Access Denied)";
				const string InvalidRegionMessage = "remote name could not be resolved";
				var originalError = e;
				do
				{
					if (e.Message.Contains(InvalidRegionMessage))
					{
						ErrorHandler.HandleError("Couldn't connect on the selected region, please try again using the region that is associated with your account.", "Connection failed");
						return false;
					}

					if (e.Message.Contains(AccessDeniedMessage))
					{
						ErrorHandler.HandleError("Couldn't connect with the current configuration, please check your API Key and Region and try again.", "Connection failed");
						return false;
					}

					e = e.InnerException;
				} while (e is not null);

				ErrorHandler.HandleError(originalError);
				return false;
			}
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
				if (!Options.LanguagesSupported.ContainsKey(languagePair.TargetCultureName))
				{
					Options?.LanguagesSupported?.Add(languagePair.TargetCultureName, _providerControlViewModel.SelectedTranslationOption.Name);
				}
			}
		}

		private void SetMicrosoftProviderOptions()
		{
			Options.ClientID = _providerControlViewModel.ClientID;
			Options.Region = _providerControlViewModel.Region.Key;
			Options.UseCategoryID = _providerControlViewModel.UseCategoryID;
			Options.CategoryID = _providerControlViewModel.CategoryID;
			Options.PersistMicrosoftCredentials = _providerControlViewModel.PersistMicrosoftKey;

			Options.LanguageMappings = _providerControlViewModel.LanguageMappings;

			Options.UsePrivateEndpoint = UsePrivateEndpoint;
			Options.PrivateEndpoint = _privateEndpointViewModel.Endpoint;
			Options.Parameters = _privateEndpointViewModel.Parameters.ToList();
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
														  : SelectedView.Name == ViewDetails_Provider ? ViewDetails_Settings
																									  : ViewDetails_Provider;
				MultiButtonContent = requestedType == ViewDetails_Provider ? "Settings" : "Provider";
				SelectedView = AvailableViews.FirstOrDefault(x => x.Name == requestedType);
			}
			catch { }
		}

		private void SetCredentialsOnUI()
		{
			try
			{
				var uri = new TranslationProviderUriBuilder(Constants.MicrosoftProviderScheme);
				var genericCredentials = new GenericCredentials(_credentialStore.GetCredential(uri.Uri).Credential);
				if (genericCredentials is null)
				{
					return;
				}

				bool.TryParse(genericCredentials["Persist-ApiKey"], out var persistApiKey);
				_providerControlViewModel.PersistMicrosoftKey = persistApiKey;
				_providerControlViewModel.ClientID = persistApiKey ? genericCredentials["API-Key"] : string.Empty;

				bool.TryParse(genericCredentials["Use-PrivateEndpoint"], out var usePrivateEndpoint);
				bool.TryParse(genericCredentials["Persist-PrivateEndpoint"], out var persistsPrivateEndpoint);

				bool.TryParse(genericCredentials["UseCategoryID"], out var useCategoryId);
				_providerControlViewModel.UseCategoryID = useCategoryId;
				_providerControlViewModel.CategoryID = useCategoryId ? genericCredentials["CategoryID"] : string.Empty;

				_providerControlViewModel.Region = _showSettingsViews
												 ? _providerControlViewModel.Regions.FirstOrDefault(x => x.Key.Equals(genericCredentials["Region"])) ?? _providerControlViewModel.Regions.FirstOrDefault()
												 : _providerControlViewModel.Regions.FirstOrDefault();

				_privateEndpointViewModel.Endpoint = genericCredentials["Endpoint"];

				var headers = genericCredentials.ToCredentialString().Split(';').Where(x => x.StartsWith("header_"));
				foreach (var header in headers)
				{
					var pair = header.Split('=');
					_privateEndpointViewModel.Headers.Add(new()
					{
						
						Key = HttpUtility.UrlDecode(pair[0].Replace("header_", string.Empty)),
						Value = HttpUtility.UrlDecode(pair[1].Replace("header_", string.Empty))
					});
				}

				var parameters = genericCredentials.ToCredentialString().Split(';').Where(x => x.StartsWith("parameter_"));
				foreach (var parameter in parameters)
				{
					var pair = parameter.Split('=');
					var key = pair[0].Replace("parameter_", string.Empty);
					var value = pair[1];

					if (key.StartsWith("from") || key.StartsWith("to"))
					{
						continue;
					}

					_privateEndpointViewModel.Parameters.Add(new()
					{
						Key = HttpUtility.UrlDecode(key),
						Value = HttpUtility.UrlDecode(value)
					});
				}

				_privateEndpointViewModel.Headers = new ObservableCollection<UrlMetadata>(_privateEndpointViewModel.Headers.Where(x => x.Key is not null && x.Value is not null));
				_privateEndpointViewModel.Parameters = new ObservableCollection<UrlMetadata>(_privateEndpointViewModel.Parameters.Where(x => x.Key is not null && x.Value is not null));
			}
			catch { }
		}

		private void SaveCredentials()
		{
			var uri = new TranslationProviderUriBuilder(Constants.MicrosoftProviderScheme);
			_credentialStore.RemoveCredential(uri.Uri);

			var persistApiKey = _providerControlViewModel.PersistMicrosoftKey;
			var useCategoryId = _providerControlViewModel.UseCategoryID;

			var currentCredentials = new GenericCredentials("mstpusername", "mstppassword")
			{
				["Persist-ApiKey"] = persistApiKey.ToString(),
				["API-Key"] = persistApiKey
							? _providerControlViewModel.ClientID
							: string.Empty,

				["UseCategoryID"] = _providerControlViewModel.UseCategoryID.ToString(),
				["CategoryID"] = useCategoryId
							   ? _providerControlViewModel.CategoryID
							   : string.Empty,

				["Region"] = _providerControlViewModel.Region.Key,

				["Endpoint"] = _privateEndpointViewModel.Endpoint,
			};

			foreach (var header in _privateEndpointViewModel?.Headers)
			{
				if (header.Key is null || header.Value is null)
				{
					continue;
				}

				currentCredentials[$"header_{header.Key}"] = header.Value;
			}

			foreach (var parameter in _privateEndpointViewModel?.Parameters)
			{
				if (parameter.Key is null || parameter.Value is null)
				{
					continue;
				}

				currentCredentials[$"parameter_{parameter.Key}"] = parameter.Value;
			}

			var credentials = new TranslationProviderCredential(currentCredentials.ToString(), true);
			_credentialStore.AddCredential(uri.Uri, credentials);
		}

		private bool _usePrivateEndpoint;
		public bool UsePrivateEndpoint
		{
			get => _usePrivateEndpoint;
			set
			{
				if (_usePrivateEndpoint == value) return;
				_usePrivateEndpoint = value;
				OnPropertyChanged();
			}
		}

		private List<string> _endpoints;
		public List<string> Endpoints
		{
			get => _endpoints;
			set
			{
				if (_endpoints == value) return;
				_endpoints = value;
				OnPropertyChanged();
			}
		}

		private string _selectedEndpoint;
		public string SelectedEndpoint
		{
			get => _selectedEndpoint;
			set
			{
				if (_selectedEndpoint == value) return;
				_selectedEndpoint = value;
				UsePrivateEndpoint = _selectedEndpoint.Equals("Private Endpoint");
				SwitchView(_selectedEndpoint.Equals("Microsoft") ? nameof(ProviderControlViewModel) : nameof(PrivateEndpointViewModel));
				OnPropertyChanged();
			}
		}
	}
}