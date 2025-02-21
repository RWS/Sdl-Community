using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageMappingProvider.Database.Interface;
using MicrosoftTranslatorProvider.Commands;
using MicrosoftTranslatorProvider.Interfaces;
using MicrosoftTranslatorProvider.Model;
using MicrosoftTranslatorProvider.Service;
using Sdl.LanguagePlatform.Core;

namespace MicrosoftTranslatorProvider.ViewModel
{
	public class MicrosoftConfigurationViewModel : BaseViewModel
    {
		readonly ILanguageMappingDatabase _languageMappingDatabase;
		readonly LanguagePair[] _languagePairs;

		string _loadingAction;

		public MicrosoftConfigurationViewModel(ITranslationOptions translationOptions, LanguagePair[] languagePairs, ILanguageMappingDatabase languageMappingDatabase)
		{
			LoadingAction = "Initializing...";
			_languagePairs = languagePairs;
            TranslationOptions = translationOptions;
			_languageMappingDatabase = languageMappingDatabase;
			InitializeCommands();
			LoadingAction = string.Empty;
			LoadPairMapping();
        }

		public ITranslationOptions TranslationOptions { get; private set; }

        public ObservableCollection<PairModel> PairModels { get; private set; }

		public string LoadingAction
		{
			get => _loadingAction;
			set
			{
				_loadingAction = value;
				OnPropertyChanged();
			}
		}

		public ICommand ResetAndIdentifyPairsCommand { get; private set; }

		private void InitializeCommands()
		{
			ResetAndIdentifyPairsCommand = new RelayCommand(ResetAndIdentifyPairs);
		}

		private void ResetAndIdentifyPairs(object parameter)
		{
			if (parameter is string parameterString
			 && parameterString.Equals("Reset to default"))
			{
				PairModels.Clear();
			}

			CreatePairMappings();
		}

		private async void LoadPairMapping()
		{
			if (TranslationOptions.PairModels is null || !TranslationOptions.PairModels.Any())
			{
				CreatePairMappings();
				return;
			}

			LoadingAction = "Loading resources...";
			await Task.Delay(0);
			PairModels = new(TranslationOptions.PairModels.Select(pm => pm.Clone()));
            LoadingAction = string.Empty;
		}

		public async void CreatePairMappings()
		{
			var originalPairMappings = PairModels;
			PairModels = [];
			LoadingAction = "Getting supported languages...";
			var supportedLanguages = await MicrosoftService.GetSupportedLanguageCodes();
			LoadingAction = "Getting mapped languages...";
			var mappedLanguages = _languageMappingDatabase.GetMappedLanguages();
			foreach (var languagePair in _languagePairs)
			{
				var mappedLanguagePairs = mappedLanguages.Where(mappedLang => mappedLang.TradosCode.Equals(languagePair.SourceCultureName) || mappedLang.TradosCode.Equals(languagePair.TargetCultureName));
				var mappedSource = mappedLanguagePairs.FirstOrDefault(mappedLang => mappedLang.TradosCode.Equals(languagePair.SourceCultureName));
				var mappedTarget = mappedLanguagePairs.FirstOrDefault(mappedLang => mappedLang.TradosCode.Equals(languagePair.TargetCultureName));
				var displayName = $"{mappedSource.Name} ({mappedSource.Region}) - {mappedTarget?.Name} ({mappedTarget?.Region})";
				var sourceLanguageCode = mappedSource.LanguageCode;
				var targetLanguageCode = mappedTarget.LanguageCode;
				var isSupported = !string.IsNullOrEmpty(sourceLanguageCode)
							   && !string.IsNullOrEmpty(targetLanguageCode)
							   && supportedLanguages.Contains(sourceLanguageCode)
							   && supportedLanguages.Contains(targetLanguageCode);

				var currentModel = originalPairMappings?.FirstOrDefault(pair => pair.DisplayName.Equals(displayName));
				if (currentModel is not null
				 && sourceLanguageCode.Equals(currentModel.SourceLanguageCode)
				 && targetLanguageCode.Equals(currentModel.TargetLanguageCode))
				{
					PairModels.Add(currentModel.Clone());
					continue;
				}

				var pairModel = new PairModel()
				{
					DisplayName = displayName,
					IsSupported = isSupported,
					SourceLanguageCode = sourceLanguageCode,
					SourceLanguageName = mappedSource.Name,
					TargetLanguageCode = targetLanguageCode,
					TargetLanguageName = mappedTarget.Name,
					TradosLanguagePair = languagePair
				};

				PairModels.Add(pairModel);
			}

			OnPropertyChanged(nameof(PairModels));
			if (!TranslationOptions.ProviderSettings.ConfigureLanguages)
			{
				TranslationOptions.ProviderSettings.ConfigureLanguages = PairModels.Any(pm => string.IsNullOrEmpty(pm.SourceLanguageCode) || string.IsNullOrEmpty(pm.TargetLanguageCode));
			}

			LoadingAction = string.Empty;
        }
    }
}