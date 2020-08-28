using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.MtEnhancedProvider.Annotations;
using Sdl.Community.MtEnhancedProvider.Commands;
using Sdl.Community.MtEnhancedProvider.Helpers;
using Sdl.Community.MtEnhancedProvider.Model;
using Sdl.Community.MtEnhancedProvider.Model.Interface;
using Sdl.Community.MtEnhancedProvider.ViewModel.Interface;

namespace Sdl.Community.MtEnhancedProvider.ViewModel
{
	public class MainWindowViewModel: ModelBase, IMainWindow
	{
		private ViewDetails _selectedView;
		private bool _dialogResult;
		private string _errorMessage;
		private readonly IProviderControlViewModel _providerControlViewModel;
		private readonly ISettingsControlViewModel _settingsControlViewModel;

		public delegate void CloseWindowEventRaiser();
		public event CloseWindowEventRaiser CloseEventRaised;

		public MainWindowViewModel(IMtTranslationOptions options,IProviderControlViewModel providerControlViewModel,ISettingsControlViewModel settingsControlViewModel)
		{
			Options = options;
			_providerControlViewModel = providerControlViewModel;
			_settingsControlViewModel = settingsControlViewModel;

			SaveCommand = new RelayCommand(Save);
			ShowSettingsViewCommand =  new CommandHandler(ShowSettingsPage, true);
			ShowMainViewCommand = new CommandHandler(ShowProvidersPage,true);

			providerControlViewModel.ShowSettingsCommand = ShowSettingsViewCommand;
			settingsControlViewModel.ShowMainWindowCommand = ShowMainViewCommand;

			AvailableViews = new List<ViewDetails>
			{
				new ViewDetails
				{
					Name = PluginResources.PluginsView,
					ViewModel = providerControlViewModel.ViewModel
				},
				new ViewDetails
				{
					Name = PluginResources.SettingsView,
					ViewModel = settingsControlViewModel.ViewModel
				}
			};

			ShowProvidersPage();
		}

		public ViewDetails SelectedView
		{
			get => _selectedView;
			set
			{
				_selectedView = value;
				ErrorMessage = string.Empty;
				OnPropertyChanged(nameof(SelectedView));
			}
		}

		public List<ViewDetails> AvailableViews { get; set; }
		public ICommand ShowSettingsViewCommand { get; set; }
		public ICommand ShowMainViewCommand { get; set; }
		public ICommand SaveCommand { get; set; }
		public IMtTranslationOptions Options { get;set; }

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
		
		public string ErrorMessage
		{
			get => _errorMessage;
			set
			{
				if (_errorMessage == value) return;
				_errorMessage = value;
				OnPropertyChanged(nameof(ErrorMessage));
			}
		}

		public void AddEncriptionMetaToResponse(string errorMessage)
		{
			var htmlStart = "<html> \n <meta http-equiv=\'Content-Type\' content=\'text/html;charset=UTF-8\'>\n <body style=\"font-family:Segoe Ui!important;color:red!important;font-size:13px!important\">\n";

			ErrorMessage = $"{errorMessage.Insert(0, htmlStart)}\n</body></html>";
		}

		public bool IsWindowValid()
		{
			ErrorMessage = string.Empty;

			switch (_providerControlViewModel.SelectedTranslationOption.ProviderType)
			{
				case MtTranslationOptions.ProviderType.MicrosoftTranslator:
					if (!ValidMicrosoftOptions()) return false;
					break;
				case MtTranslationOptions.ProviderType.GoogleTranslate:
					if (!ValidGoogleOptions()) return false;
					break;
			}
			if (ValidSettingsPageOptions())
			{
				return true;
			}
			return  false;
		}

		private bool ValidGoogleOptions()
		{
			if (_providerControlViewModel.SelectedGoogleApiVersion.Version == Enums.GoogleApiVersion.V2)
			{
				if (string.IsNullOrEmpty(_providerControlViewModel.ApiKey))
				{
					AddEncriptionMetaToResponse(PluginResources.ApiKeyError);
					return false;
				}
			}
			else
			{
				if (string.IsNullOrEmpty(_providerControlViewModel.JsonFilePath))
				{
					AddEncriptionMetaToResponse(PluginResources.EmptyJsonFilePathMsg);
					return false;
				}
				if (!File.Exists(_providerControlViewModel.JsonFilePath))
				{
					AddEncriptionMetaToResponse(PluginResources.WrongJsonFilePath);
					return false;
				}
				if (string.IsNullOrEmpty(_providerControlViewModel.ProjectName))
				{
					AddEncriptionMetaToResponse(PluginResources.InvalidProjectName);
				}
			}
			return true;
		}

		private bool ValidSettingsPageOptions()
		{
			if (_settingsControlViewModel.DoPreLookup)
			{
				if (string.IsNullOrEmpty(_settingsControlViewModel.PreLookupFileName))
				{
					AddEncriptionMetaToResponse(PluginResources.PreLookupEmptyMessage);
					return false;
				}
				if (!File.Exists(_settingsControlViewModel.PreLookupFileName))
				{
					AddEncriptionMetaToResponse(PluginResources.PreLookupWrongPathMessage);
					return false;
				}
			}
			if (!_settingsControlViewModel.DoPostLookup) return true;
			if (string.IsNullOrEmpty(_settingsControlViewModel.PostLookupFileName))
			{
				ErrorMessage = PluginResources.PostLookupEmptyMessage;
				return false;
			}
			if (File.Exists(_settingsControlViewModel.PostLookupFileName)) return true;
			ErrorMessage = PluginResources.PostLookupWrongPathMessage;
			return false;
		}

		private bool ValidMicrosoftOptions()
		{
			if (string.IsNullOrEmpty(_providerControlViewModel.ClientId))
			{
				AddEncriptionMetaToResponse(PluginResources.ApiKeyError);
				return false;
			}
			if (_providerControlViewModel.UseCatId && string.IsNullOrEmpty(_providerControlViewModel.CatId))
			{
				AddEncriptionMetaToResponse(PluginResources.CatIdError);
				return false;
			}
			return true;
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
			if (IsWindowValid())
			{
				SetMicrosoftProviderOptions();
				SetGoogleProviderOptions();

				SetGeneralProviderOptions();

				//TODO: Investigate why we have LanguagesSupported, this dictionary it seems to be used
				//Options.LanguagesSupported = _correspondingLanguages?.ToDictionary(lp => lp.TargetCultureName,
				//	lp => Options.SelectedProvider.ToString());
				DialogResult = true;
				CloseEventRaised?.Invoke();
			}
		}

		private void SetGeneralProviderOptions()
		{
			Options.SendPlainTextOnly = _settingsControlViewModel.SendPlainText;
			Options.ResendDrafts = _settingsControlViewModel.ReSendDraft;
			Options.UsePreEdit = _settingsControlViewModel.DoPreLookup;
			Options.PreLookupFilename = _settingsControlViewModel.PreLookupFileName;
			Options.UsePostEdit = _settingsControlViewModel.DoPostLookup;
			Options.PostLookupFilename = _settingsControlViewModel.PostLookupFileName;
		}

		private void SetGoogleProviderOptions()
		{
			//Google options-V2
			Options.ApiKey = _providerControlViewModel.ApiKey;
			Options.PersistGoogleKey = _providerControlViewModel.PersistGoogleKey;
			Options.SelectedGoogleVersion = _providerControlViewModel.SelectedGoogleApiVersion.Version;

			//Google options-V3
			Options.JsonFilePath = _providerControlViewModel.JsonFilePath;
			Options.ProjectName = _providerControlViewModel.ProjectName;
			Options.SelectedProvider = _providerControlViewModel.SelectedTranslationOption.ProviderType;
		}

		private void SetMicrosoftProviderOptions()
		{
			Options.ClientId = _providerControlViewModel.ClientId;
			Options.UseCatID = _providerControlViewModel.UseCatId;
			Options.CatId = _providerControlViewModel.CatId;
			Options.PersistMicrosoftCreds = _providerControlViewModel.PersistMicrosoftKey;
		}
	}
}
