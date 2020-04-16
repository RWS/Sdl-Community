using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sdl.Community.MTCloud.Languages.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Studio
{
	public class SdlMTCloudTranslationProvider : ITranslationProvider
	{
		private readonly EditorController _editorController;
		private LanguagePair _languageDirection;
		private LanguageMappingsService _languageMappingsService;

		public SdlMTCloudTranslationProvider(Uri uri, string translationProviderState, ITranslationService translationService,
		 ILanguageProvider languageProvider, EditorController editorController)
		{
			Uri = uri;

			LanguageProvider = languageProvider;
			TranslationService = translationService;

			_editorController = editorController;

			LoadState(translationProviderState);
		}

		public ProviderStatusInfo StatusInfo => new ProviderStatusInfo(true, PluginResources.Plugin_NiceName);

		public ITranslationProviderLanguageDirection LanguageDirectionProvider { get; private set; }

		public Uri Uri { get; internal set; }

		public string Name => PluginResources.Plugin_NiceName;

		public bool SupportsTaggedInput => true;

		public bool SupportsScoring => false;

		public bool SupportsSearchForTranslationUnits => true;

		public bool SupportsMultipleResults => false;

		public bool SupportsFilters => false;

		public bool SupportsPenalties => true;

		public bool SupportsStructureContext => false;

		public bool SupportsDocumentSearches => false;

		public bool SupportsUpdate => false;

		public bool SupportsPlaceables => false;

		public bool SupportsTranslation => true;

		public bool SupportsFuzzySearch => false;

		public bool SupportsConcordanceSearch => false;

		public bool SupportsSourceConcordanceSearch => false;

		public bool SupportsTargetConcordanceSearch => false;

		public bool SupportsWordCounts => false;

		public TranslationMethod TranslationMethod => TranslationMethod.MachineTranslation;

		public bool IsReadOnly => true;

		public Options Options { get; set; }

		public ITranslationService TranslationService { get; }

		public LanguageMappingsService LanguageMappingsService => _languageMappingsService ??
																  (_languageMappingsService = new LanguageMappingsService(TranslationService));

		public ILanguageProvider LanguageProvider { get; }
		
		public bool SupportsLanguageDirection(LanguagePair languageDirection)
		{
			try
			{
				_languageDirection = languageDirection;
				var supportedLanguage = GetMTCloudLanguagePair(_languageDirection);
				if (supportedLanguage != null)
				{
					return true;
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{Constants.SupportsLanguageDirection} {e.Message}\n {e.StackTrace}");
			}

			return false;
		}

		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
		{
			if (LanguageDirectionProvider != null &&
				LanguageDirectionProvider.SourceLanguage?.Name == languageDirection.SourceCulture.Name &&
				LanguageDirectionProvider.TargetLanguage?.Name == languageDirection.TargetCulture.Name)
			{
				return LanguageDirectionProvider;
			}
		
			LanguageDirectionProvider = new SdlMTCloudLanguageDirection(this, languageDirection, _editorController);

			return LanguageDirectionProvider;
		}

		public void RefreshStatusInfo()
		{
		}

		public string SerializeState()
		{
			var json = JsonConvert.SerializeObject(Options ?? new Options());
			return json;
		}

		public void LoadState(string translationProviderState)
		{
			try
			{
				if (string.IsNullOrEmpty(translationProviderState))
				{
					Options = new Options();
					return;
				}

				Options = JsonConvert.DeserializeObject<Options>(translationProviderState);
			}
			catch
			{
				// ignore any casting errors and simply create a new options instance
				Options = new Options();
			}
		}

		private MTCloudLanguagePair GetMTCloudLanguagePair(LanguagePair languageDirection)
		{
			MTCloudLanguagePair languagePair = null;

			if (languageDirection != null && LanguageMappingsService.SubscriptionInfo.LanguagePairs?.Count > 0)
			{
				languagePair = GetMTCloudLanguagePair();

				if (languagePair != null)
				{
					return languagePair;
				}

				var languages = LanguageProvider.GetLanguages();
				
				var languageMappingModel = GetLanguageMappingModel(languageDirection, languages);
				if (languageMappingModel != null)
				{
					Options.LanguageMappings.Add(languageMappingModel);
					languagePair = GetMTCloudLanguagePair();
				}				
			}

			return languagePair;
		}
	
		public LanguageMappingModel GetLanguageMappingModel(LanguagePair languageDirection, List<Languages.Provider.Model.Language> languages)
		{			
			var sourceLanguage = languageDirection.SourceCulture;
			var sourceLanguageCode = languages?.FirstOrDefault(s => s.TradosCode.Equals(sourceLanguage?.Name));

			var sourceLanguageMappings = LanguageMappingsService.GetMTCloudLanguage(sourceLanguageCode, sourceLanguage);
			var sourceLanguageMappingSelected = sourceLanguageMappings.FirstOrDefault(a => a.IsLocale) ?? sourceLanguageMappings[0];

			if (sourceLanguageCode == null)
			{
				return null;
			}

			var targetLanguage = languageDirection.TargetCulture;
			var targetLanguageCode = languages.FirstOrDefault(s => s.TradosCode.Equals(languageDirection.TargetCulture?.Name));

			if (targetLanguageCode == null)
			{
				return null;
			}

			var name = $"{sourceLanguage?.DisplayName} - {languageDirection.TargetCulture?.DisplayName}";
			var savedLanguageMappingModel = Options.LanguageMappings.FirstOrDefault(a => a.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

			var targetLanguageMappings = LanguageMappingsService.GetMTCloudLanguage(targetLanguageCode, targetLanguage);
			var targetLanguageMappingSelected = targetLanguageMappings.FirstOrDefault(a => a.IsLocale) ?? targetLanguageMappings[0];

			// assign the selected target langauge
			targetLanguageMappingSelected = targetLanguageMappings.FirstOrDefault(a =>
												a.CodeName.Equals(savedLanguageMappingModel?.SelectedTarget?.CodeName))
											?? targetLanguageMappingSelected;

			var engineModels = LanguageMappingsService.GetTranslationModels(sourceLanguageMappingSelected, targetLanguageMappingSelected,
				sourceLanguageCode.TradosCode, targetLanguageCode.TradosCode);

			// attempt to recover the language model from the secondary language code if it exists!
			if (engineModels.Count == 1 && engineModels[0].DisplayName == PluginResources.Message_No_model_available && targetLanguageMappings.Count > 1
				&& savedLanguageMappingModel?.SelectedModel.DisplayName != PluginResources.Message_No_model_available)
			{
				var secondaryLanguageCode = targetLanguageMappings.FirstOrDefault(a => a.CodeName != targetLanguageMappingSelected.CodeName);

				var secondaryEngineModels = LanguageMappingsService.GetTranslationModels(
					sourceLanguageMappingSelected, secondaryLanguageCode, sourceLanguageCode.TradosCode, targetLanguageCode.TradosCode);

				if (secondaryEngineModels.Any())
				{
					engineModels = secondaryEngineModels;
					targetLanguageMappingSelected = secondaryLanguageCode;
				}
			}

			if (engineModels.Any())
			{
				// assign the selected model
				var selectedModel =
					engineModels.FirstOrDefault(a => a.DisplayName.Equals(savedLanguageMappingModel?.SelectedModel?.DisplayName, StringComparison.InvariantCultureIgnoreCase))
					?? engineModels.FirstOrDefault(a => a.Model.Equals("generic", StringComparison.InvariantCultureIgnoreCase))
					?? engineModels[0];

				var dictionaries = LanguageMappingsService.GetDictionaries(sourceLanguageMappingSelected, targetLanguageMappingSelected);

				// assign the selected dictionary
				var selectedDictionary =
					dictionaries.FirstOrDefault(a => a.Name.Equals(savedLanguageMappingModel?.SelectedDictionary?.Name))
					?? dictionaries[0];

				var languageMappingModel = new LanguageMappingModel
				{
					Name = name,
					SourceLanguages = sourceLanguageMappings,
					TargetLanguages = targetLanguageMappings,
					SelectedSource = sourceLanguageMappingSelected,
					SelectedTarget = targetLanguageMappingSelected,
					SourceTradosCode = sourceLanguageCode.TradosCode,
					TargetTradosCode = targetLanguageCode.TradosCode,
					Models = engineModels,
					SelectedModel = selectedModel,
					Dictionaries = dictionaries,
					SelectedDictionary = selectedDictionary
				};

				return languageMappingModel;
			}

			return null;
		}

		private MTCloudLanguagePair GetMTCloudLanguagePair()
		{
			var languagePair = LanguageMappingsService.SubscriptionInfo.LanguagePairs
				.FirstOrDefault(o => Options.LanguageMappings
					.Any(l => l.SourceLanguages.Any(a =>
						          a.CodeName.Equals(o.SourceLanguageId, StringComparison.InvariantCultureIgnoreCase))
					          && l.TargetLanguages.Any(a =>
						          a.CodeName.Equals(o.TargetLanguageId, StringComparison.InvariantCultureIgnoreCase))));
			return languagePair;
		}
	}
}