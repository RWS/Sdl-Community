using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Sdl.Community.MtEnhancedProvider.Annotations;
using Sdl.Community.MtEnhancedProvider.Commands;
using Sdl.Community.MtEnhancedProvider.Model;
using Sdl.Community.MtEnhancedProvider.Model.Interface;
using Sdl.Community.MtEnhancedProvider.ViewModel.Interface;

namespace Sdl.Community.MtEnhancedProvider.ViewModel
{
	public class MainWindowViewModel: ModelBase, IMainWindow
	{
		private ViewDetails _selectedView;
		private bool _dialogResult;
		private readonly IProviderControlViewModel _providerControlViewModel;
		private readonly ISettingsControlViewModel _settingsControlViewModel;

		public MainWindowViewModel(IMtTranslationOptions options,IProviderControlViewModel providerControlViewModel,ISettingsControlViewModel settingsControlViewModel)
		{
			Options = options;
			_providerControlViewModel = providerControlViewModel;
			_settingsControlViewModel = settingsControlViewModel;

			SaveCommand = new CommandHandler(Save,true);
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

		private void ShowSettingsPage()
		{
			SelectedView = AvailableViews[1];
		}

		private void ShowProvidersPage()
		{
			SelectedView = AvailableViews[0];
		}

		private void Save()
		{
			//TODO: Validate the form
			SetMicrosoftProviderOptions();
			SetGoogleProviderOptions();

			SetGeneralProviderOptions();

			//TODO: Investigate why we have LanguagesSupported, this dictionary it seems to be used
			//Options.LanguagesSupported = _correspondingLanguages?.ToDictionary(lp => lp.TargetCultureName,
			//	lp => Options.SelectedProvider.ToString());
			DialogResult = true;
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
