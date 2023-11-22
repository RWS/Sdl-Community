using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Windows.Input;
using MicrosoftTranslatorProvider.Commands;
using MicrosoftTranslatorProvider.Extensions;
using MicrosoftTranslatorProvider.Helpers;
using MicrosoftTranslatorProvider.Interface;
using MicrosoftTranslatorProvider.Interfaces;
using MicrosoftTranslatorProvider.Model;
using MicrosoftTranslatorProvider.Studio.TranslationProvider;
using MicrosoftTranslatorProvider.View;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace MicrosoftTranslatorProvider.ViewModel
{
	public class MainWindowViewModel : BaseModel, IMainWindow
	{
		private const string ViewDetails_Provider = nameof(ProviderViewModel);
		private const string ViewDetails_Settings = nameof(SettingsViewModel);
		private const string ViewDetails_PrivateEndpoint = nameof(PrivateEndpointViewModel);

		private readonly ISettingsViewModel _settingsControlViewModel;
		private readonly IProviderViewModel _providerControlViewModel;
		private readonly IPrivateEndpointViewModel _privateEndpointViewModel;
		private readonly ITranslationProviderCredentialStore _credentialStore;
		private readonly LanguagePair[] _languagePairs;
		private readonly bool _editProvider;

		private List<string> _endpoints;
		private string _selectedEndpoint;
		private bool _usePrivateEndpoint;

		private bool _dialogResult;
		private bool _canSwitchProvider;
		private ViewDetails _selectedView;
		private string _multiButtonContent;
		private bool _canAccessLanguageMappingProvider;

		private ICommand _saveCommand;
		private ICommand _navigateToCommand;
		private ICommand _switchViewCommand;
		private ICommand _openLanguageMappingProviderCommand;

		public event CloseWindowEventRaiser CloseEventRaised;
		public delegate void CloseWindowEventRaiser();

		public MainWindowViewModel(ITranslationOptions options,
								   ITranslationProviderCredentialStore credentialStore,
								   LanguagePair[] languagePairs,
								   bool editProvider = false)
		{
			TranslationOptions = options;
			CanAccessLanguageMappingProvider = File.Exists(Constants.DatabaseFilePath);
			_providerControlViewModel = new ProviderViewModel(options, languagePairs, editProvider);
			_settingsControlViewModel = new SettingsViewModel(options);
			_privateEndpointViewModel = new PrivateEndpointViewModel();
			_credentialStore = credentialStore;
			_languagePairs = languagePairs;
			_editProvider = editProvider;
			

			AvailableViews = new List<ViewDetails>
			{
				new ViewDetails
				{
					Name = nameof(ProviderViewModel),
					ViewModel = _providerControlViewModel.ViewModel
				},
				new ViewDetails
				{
					Name = nameof(SettingsViewModel),
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
			SwitchView(TranslationOptions.UsePrivateEndpoint ? ViewDetails_PrivateEndpoint : ViewDetails_Provider);
			SetCredentialsOnUI();

			if (_editProvider)
			{
				DatabaseExtensions.CreateDatabase(TranslationOptions);
				CanAccessLanguageMappingProvider = File.Exists(Constants.DatabaseFilePath);
			}
		}

		public bool DialogResult
		{
			get => _dialogResult;
			set
			{
				if (_dialogResult == value) return;
				_dialogResult = value;
				OnPropertyChanged();
			}
		}

		public string MultiButtonContent
		{
			get => _multiButtonContent;
			set
			{
				if (_multiButtonContent == value) return;
				_multiButtonContent = value;
				OnPropertyChanged();
			}
		}

		public bool CanAccessLanguageMappingProvider
		{
			get => _canAccessLanguageMappingProvider;
			set
			{
				if (_canAccessLanguageMappingProvider == value) return;
				_canAccessLanguageMappingProvider = value;
				OnPropertyChanged();
			}
		}

		public ViewDetails SelectedView
		{
			get => _selectedView;
			set
			{
				_selectedView = value;
				OnPropertyChanged();
			}
		}

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

		public string SelectedEndpoint
		{
			get => _selectedEndpoint;
			set
			{
				if (_selectedEndpoint == value) return;
				_selectedEndpoint = value;
				UsePrivateEndpoint = _selectedEndpoint.Equals("Private Endpoint");
				SwitchView(_selectedEndpoint.Equals("Microsoft") ? nameof(ProviderViewModel) : nameof(PrivateEndpointViewModel));
				OnPropertyChanged();
			}
		}

		public bool CanSwitchProvider
		{
			get => _canSwitchProvider;
			set
			{
				if (value == _canSwitchProvider) return;
				_canSwitchProvider = value;
				OnPropertyChanged();
			}
		}

		public List<ViewDetails> AvailableViews { get; set; }

		public ITranslationOptions TranslationOptions { get; set; }

		public ICommand SaveCommand => _saveCommand ??= new RelayCommand(Save);
		public ICommand NavigateToCommand => _navigateToCommand ??= new RelayCommand(NavigateTo);
		public ICommand SwitchViewCommand => _switchViewCommand ??= new RelayCommand(SwitchView);

		public ICommand OpenLanguageMappingProviderCommand => _openLanguageMappingProviderCommand ??= new RelayCommand(OpenLanguageMappingProvider);

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
			if (string.IsNullOrEmpty(_providerControlViewModel.ApiKey))
			{
				ErrorHandler.HandleError(PluginResources.ApiKeyError, "API Key");
				return false;
			}

			return AreMicrosoftCredentialsValid();
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
			DatabaseExtensions.CreateDatabase(TranslationOptions);
			DialogResult = true;
			CloseEventRaised?.Invoke();
		}

		private bool AreMicrosoftCredentialsValid()
		{
			try
			{
				if (TranslationOptions.UsePrivateEndpoint)
				{
					return true;
				}

				var apiConnecter = new MicrosoftApi(_providerControlViewModel.ApiKey, _providerControlViewModel.SelectedRegion?.Key);
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
				TranslationOptions.SendPlainTextOnly = _settingsControlViewModel.SendPlainText;
				TranslationOptions.ResendDrafts = _settingsControlViewModel.ReSendDraft;
				TranslationOptions.UsePreEdit = _settingsControlViewModel.DoPreLookup;
				TranslationOptions.PreLookupFilename = _settingsControlViewModel.PreLookupFileName;
				TranslationOptions.UsePostEdit = _settingsControlViewModel.DoPostLookup;
				TranslationOptions.PostLookupFilename = _settingsControlViewModel.PostLookupFileName;
				TranslationOptions.CustomProviderName = _settingsControlViewModel.CustomProviderName;
				TranslationOptions.UseCustomProviderName = _settingsControlViewModel.UseCustomProviderName;
			}

			if (TranslationOptions != null && TranslationOptions.LanguagesSupported == null)
			{
				TranslationOptions.LanguagesSupported = new List<string>();
			}

			if (_languagePairs == null)
			{
				return;
			}

			foreach (var languagePair in _languagePairs)
			{
				if (!TranslationOptions.LanguagesSupported.Contains(languagePair.TargetCultureName))
				{
					TranslationOptions?.LanguagesSupported?.Add(languagePair.TargetCultureName);
				}
			}
		}

		private void SetMicrosoftProviderOptions()
		{
			TranslationOptions.ApiKey = _providerControlViewModel.ApiKey;
			TranslationOptions.Region = _providerControlViewModel.SelectedRegion.Key;
			TranslationOptions.PersistMicrosoftCredentials = _providerControlViewModel.PersistMicrosoftKey;
			TranslationOptions.LanguageMappings = _providerControlViewModel.LanguageMappings;
			TranslationOptions.UsePrivateEndpoint = UsePrivateEndpoint;
			TranslationOptions.PrivateEndpoint = _privateEndpointViewModel.Endpoint;
			TranslationOptions.Parameters = _privateEndpointViewModel.Parameters.ToList();
		}

		private void NavigateTo(object parameter)
		{
			Process.Start(parameter as string);
		}

		private void SwitchView(object parameter)
		{
			try
			{
				string requestedType;
				if (parameter is string parameterString)
				{
					requestedType = parameterString;
				}
				else
				{
					requestedType = SelectedView.Name == ViewDetails_Provider || SelectedView.Name == ViewDetails_PrivateEndpoint
						? ViewDetails_Settings
						: SelectedEndpoint == "Microsoft" ? ViewDetails_Provider : ViewDetails_PrivateEndpoint;
				}
				MultiButtonContent = requestedType == ViewDetails_Provider || requestedType == ViewDetails_PrivateEndpoint ? "Settings" : "Provider";
				SelectedView = AvailableViews.FirstOrDefault(x => x.Name == requestedType);
				CanSwitchProvider = _editProvider || SelectedView.Name == ViewDetails_Settings;
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
				_providerControlViewModel.ApiKey = _editProvider || persistApiKey ? genericCredentials["API-Key"] : string.Empty;
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

			var currentCredentials = new GenericCredentials("mstpusername", "mstppassword")
			{
				["Persist-ApiKey"] = persistApiKey.ToString(),
				["API-Key"] = _providerControlViewModel.ApiKey,
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

		private void OpenLanguageMappingProvider(object parameter)
		{
			var lmpViewModel = new LanguageMappingProviderViewModel(TranslationOptions, _editProvider);
			var lmpView = new LanguageMappingProviderView() { DataContext = lmpViewModel };
			lmpViewModel.CloseEventRaised += () =>
			{
				lmpView.Close();
			};

			var dialog = lmpView.ShowDialog();
		}
	}
}