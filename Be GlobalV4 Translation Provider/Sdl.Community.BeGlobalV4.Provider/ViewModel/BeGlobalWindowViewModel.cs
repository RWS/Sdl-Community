using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.Community.BeGlobalV4.Provider.Ui;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class BeGlobalWindowViewModel : BaseViewModel
	{
		public BeGlobalTranslationOptions Options { get; set; }
		public LoginViewModel LoginViewModel { get; set; }
		public ObservableCollection<TranslationModel> TranslationOptions { get; set; }

		//public SettingsViewModel SettingsViewModel { get; set; }	
		private ICommand _okCommand;
		private readonly bool _tellMeAction;
		private  bool _reSendChecked;
		private readonly BeGlobalWindow _mainWindow;
		private readonly NormalizeSourceTextHelper _normalizeSourceTextHelper;
		private readonly LanguagePair[] _languagePairs;
		private TranslationModel _selectedModel;

		public BeGlobalWindowViewModel(BeGlobalWindow mainWindow, BeGlobalTranslationOptions options, LanguagePair[] languagePairs)
		{
			LoginViewModel = new LoginViewModel(options);
			Options = options;
			_mainWindow = mainWindow;
			_languagePairs = languagePairs;
			_normalizeSourceTextHelper = new NormalizeSourceTextHelper();
			TranslationOptions = new ObservableCollection<TranslationModel>();

			var beGlobalTranslator = new BeGlobalV4Translator(Options.Model);
			var accountId = beGlobalTranslator.GetUserInformation();

			var subscriptionInfo = beGlobalTranslator.GetLanguagePairs(accountId.ToString());

			GetEngineModels(subscriptionInfo.LanguagePairs);
			SetEngineModel();
		}

		//TODO: fix this this constructor used for TELL ME
		//public BeGlobalWindowViewModel(BeGlobalWindow mainWindow, BeGlobalTranslationOptions options, bool tellMeAction)
		//{
		//	LoginViewModel = new LoginViewModel(options);
		//	TranslationOptions = new ObservableCollection<TranslationModel>();
		//	Options = options;
		//	if (options != null)
		//	{
		//		var mtModel = TranslationOptions?.FirstOrDefault(m => m.Model.Equals(options.Model));
		//		if (mtModel != null)
		//		{
		//			var selectedModelIndex = TranslationOptions.IndexOf(mtModel);
		//			SettingsViewModel.SelectedModelOption = SettingsViewModel.TranslationOptions[selectedModelIndex];
		//		}
		//		else if(SettingsViewModel.TranslationOptions.Count.Equals(0))
		//		{
		//			var translationModel = new TranslationModel
		//			{
		//				Model = options.Model,
		//				DisplayName = options.Model
		//			};
		//			SettingsViewModel.TranslationOptions.Add(translationModel);
		//			SettingsViewModel.SelectedModelOption = translationModel;
		//		}
		//	}
		//	_mainWindow = mainWindow;
		//	_tellMeAction = tellMeAction;
		//	_normalizeSourceTextHelper = new NormalizeSourceTextHelper();
		//}

		public ICommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(Ok));
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
					Options.Model = value.Model;
				}
				OnPropertyChanged(nameof(SelectedModelOption));
			}
		}


		private void GetEngineModels(List<BeGlobalLanguagePair> beGlobalLanguagePairs)
		{
			var sourceLanguage = _normalizeSourceTextHelper.GetCorespondingLangCode(_languagePairs?[0].SourceCulture);
			var pairsWithSameSource = beGlobalLanguagePairs.Where(l => l.SourceLanguageId.Equals(sourceLanguage)).ToList();
			if (_languagePairs?.Count() > 0)
			{
				foreach (var languagePair in _languagePairs)
				{
					var targetLanguage =
						_normalizeSourceTextHelper.GetCorespondingLangCode(languagePair.TargetCulture);

					var serviceLanguagePairs = pairsWithSameSource.Where(t => t.TargetLanguageId.Equals(targetLanguage)).ToList();

					foreach (var serviceLanguagePair in serviceLanguagePairs)
					{
						if (TranslationOptions != null)
						{
							var engineExists = TranslationOptions.Any(e => e.Model.Equals(serviceLanguagePair.Model));
							if (!engineExists)
							{
								TranslationOptions.Add(new TranslationModel
								{
									Model = serviceLanguagePair.Model,
									DisplayName = serviceLanguagePair.DisplayName
								});
							}
						}
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
						Options.Model = TranslationOptions?[0].Model;
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

	}
}
