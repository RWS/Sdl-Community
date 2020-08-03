using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sdl.Community.MTCloud.Languages.Provider.Interfaces;
using Sdl.Community.MTCloud.Languages.Provider.Model;
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

		public ILanguageMappingsService LanguageMappingsService => _languageMappingsService ??
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
				Options = JsonConvert.DeserializeObject<Options>(translationProviderState);
			}
			catch
			{
				// ignore any casting errors and simply create a new options instance
				Options = new Options();
			}
		}

		private MTCloudLanguagePair GetMTCloudLanguagePair(LanguagePair languagePair)
		{
			MTCloudLanguagePair mtCloudLanguagePair = null;

			if (languagePair != null && LanguageMappingsService.SubscriptionInfo.LanguagePairs?.Count > 0)
			{
				mtCloudLanguagePair = GetMTCloudLanguagePair();
				
				var hasOptionsLanguageMapping = HasOptionsLanguageMapping(languagePair);
				if (mtCloudLanguagePair != null && hasOptionsLanguageMapping)
				{
					return mtCloudLanguagePair;
				}

				var languages = LanguageProvider.GetMappedLanguages();

				var languageMappingModel = GetLanguageMappingModel(languagePair, languages);
				if (languageMappingModel != null)
				{
					Options.LanguageMappings.Add(languageMappingModel);
					mtCloudLanguagePair = GetMTCloudLanguagePair();
				}
			}

			return mtCloudLanguagePair;
		}

		public LanguageMappingModel GetLanguageMappingModel(LanguagePair languageDirection, List<MappedLanguage> mappedLanguages)
		{
			var internalLanguageMapping = new InternalLanguageMapping
			{
				SourceLanguageCode = mappedLanguages?.FirstOrDefault(s => s.TradosCode.Equals(languageDirection.SourceCulture?.Name))
			};

			if (internalLanguageMapping.SourceLanguageCode == null)
			{
				return null;
			}

			internalLanguageMapping.SourceLanguageMappings = LanguageMappingsService.GetMTCloudLanguages(internalLanguageMapping.SourceLanguageCode, languageDirection.SourceCulture);
			internalLanguageMapping.SelectedSourceLanguageMapping = internalLanguageMapping.SourceLanguageMappings.FirstOrDefault(a => a.IsLocale) ?? internalLanguageMapping.SourceLanguageMappings[0];

			internalLanguageMapping.TargetLanguageCode = mappedLanguages.FirstOrDefault(s => s.TradosCode.Equals(languageDirection.TargetCulture?.Name));
			if (internalLanguageMapping.TargetLanguageCode == null)
			{
				return null;
			}

			internalLanguageMapping.Name = $"{languageDirection.SourceCulture?.DisplayName} - {languageDirection.TargetCulture?.DisplayName}";
			internalLanguageMapping.SavedLanguageMappingModel = Options.LanguageMappings.FirstOrDefault(a => a.Name.Equals(internalLanguageMapping.Name, StringComparison.InvariantCultureIgnoreCase));

			internalLanguageMapping.TargetLanguageMappings = LanguageMappingsService.GetMTCloudLanguages(internalLanguageMapping.TargetLanguageCode, languageDirection.TargetCulture);
			internalLanguageMapping.SelectedTargetLanguageMapping = internalLanguageMapping.TargetLanguageMappings.FirstOrDefault(a => a.IsLocale) ?? internalLanguageMapping.TargetLanguageMappings[0];

			// assign the selected target langauge
			internalLanguageMapping.SelectedTargetLanguageMapping = internalLanguageMapping.TargetLanguageMappings.FirstOrDefault(a => a.CodeName.Equals(internalLanguageMapping.SavedLanguageMappingModel?.SelectedTarget?.CodeName))
			                                                        ?? internalLanguageMapping.SelectedTargetLanguageMapping;

			internalLanguageMapping.EngineModels = LanguageMappingsService.GetTranslationModels(internalLanguageMapping.SelectedSourceLanguageMapping, internalLanguageMapping.SelectedTargetLanguageMapping,
				internalLanguageMapping.SourceLanguageCode.TradosCode, internalLanguageMapping.TargetLanguageCode.TradosCode);

			if (internalLanguageMapping.SavedLanguageMappingModel?.SelectedModel.DisplayName != PluginResources.Message_No_model_available)
			{
				ValidateEngineExistence(internalLanguageMapping);
			}

			if (internalLanguageMapping.EngineModels.Any())
			{
				var languageMappingModel = GetLanguageMappingModel(internalLanguageMapping);
				return languageMappingModel;
			}

			return null;
		}

		public void UpdateLanguageMappingModel(LanguageMappingModel languageMappingModel)
		{
			var translationModels = LanguageMappingsService.GetTranslationModels(
				languageMappingModel.SelectedSource, languageMappingModel.SelectedTarget, languageMappingModel.SourceTradosCode,
				languageMappingModel.TargetTradosCode);

			if (translationModels.Any())
			{
				// assign the selected model
				var selectedModel = translationModels.FirstOrDefault(a => string.Compare(a.DisplayName, languageMappingModel.SelectedModel?.DisplayName, StringComparison.InvariantCultureIgnoreCase) == 0)
				                    ?? translationModels.FirstOrDefault(a => string.Compare(a.Model, "generic", StringComparison.InvariantCultureIgnoreCase) == 0)
				                    ?? translationModels[0];

				languageMappingModel.Models = translationModels;
				languageMappingModel.SelectedModel = selectedModel;
			}

			var dictionaries = LanguageMappingsService.GetDictionaries(languageMappingModel.SelectedSource, languageMappingModel.SelectedTarget);

			// assign the selected dictionary
			var selectedDictionary = dictionaries.FirstOrDefault(a => a.Name.Equals(languageMappingModel.SelectedDictionary?.Name)) ?? dictionaries[0];

			languageMappingModel.Dictionaries = dictionaries;
			languageMappingModel.SelectedDictionary = selectedDictionary;
		}

		private LanguageMappingModel GetLanguageMappingModel(InternalLanguageMapping internalLanguageMapping)
		{
			// assign the selected model
			var selectedModel = internalLanguageMapping.EngineModels.FirstOrDefault(a => a.DisplayName.Equals(internalLanguageMapping.SavedLanguageMappingModel?.SelectedModel?.DisplayName, StringComparison.InvariantCultureIgnoreCase))
			                    ?? internalLanguageMapping.EngineModels.FirstOrDefault(a => a.Model.Equals("generic", StringComparison.InvariantCultureIgnoreCase))
			                    ?? internalLanguageMapping.EngineModels[0];

			var dictionaries = LanguageMappingsService.GetDictionaries(internalLanguageMapping.SelectedSourceLanguageMapping, internalLanguageMapping.SelectedTargetLanguageMapping);

			// assign the selected dictionary
			var selectedDictionary =
				dictionaries.FirstOrDefault(a => a.Name.Equals(internalLanguageMapping.SavedLanguageMappingModel?.SelectedDictionary?.Name))
				?? dictionaries[0];

			var languageMappingModel = new LanguageMappingModel
			{
				Name = internalLanguageMapping.Name,
				SourceLanguages = internalLanguageMapping.SourceLanguageMappings,
				TargetLanguages = internalLanguageMapping.TargetLanguageMappings,
				SelectedSource = internalLanguageMapping.SelectedSourceLanguageMapping,
				SelectedTarget = internalLanguageMapping.SelectedTargetLanguageMapping,
				SourceTradosCode = internalLanguageMapping.SourceLanguageCode.TradosCode,
				TargetTradosCode = internalLanguageMapping.TargetLanguageCode.TradosCode,
				Models = internalLanguageMapping.EngineModels,
				SelectedModel = selectedModel,
				Dictionaries = dictionaries,
				SelectedDictionary = selectedDictionary
			};

			return languageMappingModel;
		}

		private void ValidateEngineExistence(InternalLanguageMapping internalLanguageMapping)
		{
			// attempt to recover the language model from the secondary target language code
			if (NoEngineFound(internalLanguageMapping.EngineModels) && internalLanguageMapping.TargetLanguageMappings.Count > 1)
			{
				var secondaryLanguageCode = internalLanguageMapping.TargetLanguageMappings.FirstOrDefault(a => a.CodeName != internalLanguageMapping.SelectedTargetLanguageMapping.CodeName);

				var secondaryEngineModels = LanguageMappingsService.GetTranslationModels(internalLanguageMapping.SelectedSourceLanguageMapping, secondaryLanguageCode,
					internalLanguageMapping.SourceLanguageCode.TradosCode, internalLanguageMapping.TargetLanguageCode.TradosCode);

				if (secondaryEngineModels.Any())
				{
					internalLanguageMapping.EngineModels = secondaryEngineModels;
					internalLanguageMapping.SelectedTargetLanguageMapping = secondaryLanguageCode;
				}
			}

			// attempt to recover the language model from the secondary source language code
			if (NoEngineFound(internalLanguageMapping.EngineModels) && internalLanguageMapping.SourceLanguageMappings.Count > 1)
			{
				internalLanguageMapping.SecondarySourceLanguageCode = internalLanguageMapping.SourceLanguageMappings.FirstOrDefault(a => a.CodeName != internalLanguageMapping.SelectedSourceLanguageMapping.CodeName);

				internalLanguageMapping.SecondaryEngineModels = LanguageMappingsService.GetTranslationModels(internalLanguageMapping.SecondarySourceLanguageCode, internalLanguageMapping.SelectedTargetLanguageMapping,
					internalLanguageMapping.SourceLanguageCode.TradosCode, internalLanguageMapping.TargetLanguageCode.TradosCode);

				if (internalLanguageMapping.SecondaryEngineModels.Any())
				{
					internalLanguageMapping.EngineModels = internalLanguageMapping.SecondaryEngineModels;
					internalLanguageMapping.SelectedSourceLanguageMapping = internalLanguageMapping.SecondarySourceLanguageCode;
				}
				else if (internalLanguageMapping.TargetLanguageMappings.Count > 1)
				{
					// attempt to recover the language model from the secondary source OR target language code
					internalLanguageMapping.SecondaryTargetLanguageCode = internalLanguageMapping.TargetLanguageMappings.FirstOrDefault(a => a.CodeName != internalLanguageMapping.SelectedTargetLanguageMapping?.CodeName);

					internalLanguageMapping.SecondaryEngineModels = LanguageMappingsService.GetTranslationModels(
						internalLanguageMapping.SecondarySourceLanguageCode, internalLanguageMapping.SecondaryTargetLanguageCode, internalLanguageMapping.SourceLanguageCode.TradosCode,
						internalLanguageMapping.TargetLanguageCode.TradosCode);

					if (internalLanguageMapping.SecondaryEngineModels.Any())
					{
						internalLanguageMapping.EngineModels = internalLanguageMapping.SecondaryEngineModels;
						internalLanguageMapping.SelectedSourceLanguageMapping = internalLanguageMapping.SecondarySourceLanguageCode;
						internalLanguageMapping.SelectedTargetLanguageMapping = internalLanguageMapping.SecondaryTargetLanguageCode;
					}
				}
			}
		}

		private bool NoEngineFound(IReadOnlyList<TranslationModel> engineModels)
		{
			return engineModels.Count == 1 &&
				   engineModels[0].DisplayName == PluginResources.Message_No_model_available;
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

		private bool HasOptionsLanguageMapping(LanguagePair languagePair)
		{
			return Options.LanguageMappings.Any(l => l.SourceTradosCode.Equals(languagePair.SourceCulture.Name)
			                                        && l.TargetTradosCode.Equals(languagePair.TargetCulture.Name));
		}
	}
}