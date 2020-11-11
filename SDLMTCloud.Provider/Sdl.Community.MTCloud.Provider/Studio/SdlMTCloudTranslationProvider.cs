using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using NLog;
using Sdl.Community.MTCloud.Languages.Provider.Interfaces;
using Sdl.Community.MTCloud.Languages.Provider.Model;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using LogManager = NLog.LogManager;

namespace Sdl.Community.MTCloud.Provider.Studio
{
	public class SdlMTCloudTranslationProvider : ITranslationProvider
	{
		private readonly EditorController _editorController;
		private LanguagePair _languageDirection;
		private LanguageMappingsService _languageMappingsService;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private RateItController _rateItController;

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

		public Options Options
		{
			get => TranslationService.Options;
			set => TranslationService.Options = value;
		}

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
				_logger.Error($"{Constants.SupportsLanguageDirection} {e.Message}\n {e.StackTrace}");
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
			Options = new Options();
			try
			{
				Options = JsonConvert.DeserializeObject<Options>(translationProviderState);
			}
			catch
			{
				// ignore any casting errors and simply create a new options instance
			}
			finally
			{
				ActivateRatingController();
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
			var mapping = new InternalLanguageMapping
			{
				SourceLanguageCode = mappedLanguages?.FirstOrDefault(s => s.TradosCode.Equals(languageDirection.SourceCulture?.Name))
			};

			if (mapping.SourceLanguageCode == null)
			{
				return null;
			}

			mapping.SourceLanguageMappings = LanguageMappingsService.GetMTCloudLanguages(mapping.SourceLanguageCode, languageDirection.SourceCulture);
			mapping.SelectedSourceLanguageMapping = mapping.SourceLanguageMappings.FirstOrDefault(a => a.IsLocale) ?? mapping.SourceLanguageMappings[0];

			mapping.TargetLanguageCode = mappedLanguages.FirstOrDefault(s => s.TradosCode.Equals(languageDirection.TargetCulture?.Name));
			if (mapping.TargetLanguageCode == null)
			{
				return null;
			}

			mapping.Name = $"{languageDirection.SourceCulture?.DisplayName} - {languageDirection.TargetCulture?.DisplayName}";
			mapping.SavedLanguageMappingModel = Options.LanguageMappings.FirstOrDefault(a => a.Name.Equals(mapping.Name, StringComparison.InvariantCultureIgnoreCase));

			mapping.TargetLanguageMappings = LanguageMappingsService.GetMTCloudLanguages(mapping.TargetLanguageCode, languageDirection.TargetCulture);
			mapping.SelectedTargetLanguageMapping = mapping.TargetLanguageMappings.FirstOrDefault(a => a.IsLocale) ?? mapping.TargetLanguageMappings[0];

			// assign the selected target langauge
			mapping.SelectedTargetLanguageMapping = mapping.TargetLanguageMappings.FirstOrDefault(a => a.CodeName.Equals(mapping.SavedLanguageMappingModel?.SelectedTarget?.CodeName))
			                                        ?? mapping.SelectedTargetLanguageMapping;

			mapping.EngineModels = LanguageMappingsService.GetTranslationModels(mapping.SelectedSourceLanguageMapping, mapping.SelectedTargetLanguageMapping,
				mapping.SourceLanguageCode.TradosCode, mapping.TargetLanguageCode.TradosCode);

			if (mapping.SavedLanguageMappingModel?.SelectedModel.DisplayName != PluginResources.Message_No_model_available)
			{
				ValidateEngineExistence(mapping);
			}

			if (mapping.EngineModels.Any())
			{
				var languageMappingModel = GetLanguageMappingModel(mapping);
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

		private LanguageMappingModel GetLanguageMappingModel(InternalLanguageMapping mapping)
		{
			// assign the selected model
			var selectedModel = mapping.EngineModels.FirstOrDefault(a => a.DisplayName.Equals(mapping.SavedLanguageMappingModel?.SelectedModel?.DisplayName, StringComparison.InvariantCultureIgnoreCase))
			                    ?? mapping.EngineModels.FirstOrDefault(a => a.Model != null && a.Model.Equals("generic", StringComparison.InvariantCultureIgnoreCase))
			                    ?? mapping.EngineModels[0];

			var dictionaries = LanguageMappingsService.GetDictionaries(mapping.SelectedSourceLanguageMapping, mapping.SelectedTargetLanguageMapping);

			// assign the selected dictionary
			var selectedDictionary =
				dictionaries.FirstOrDefault(a => a.Name.Equals(mapping.SavedLanguageMappingModel?.SelectedDictionary?.Name))
				?? dictionaries[0];

			var languageMappingModel = new LanguageMappingModel
			{
				Name = mapping.Name,
				SourceLanguages = mapping.SourceLanguageMappings,
				TargetLanguages = mapping.TargetLanguageMappings,
				SelectedSource = mapping.SelectedSourceLanguageMapping,
				SelectedTarget = mapping.SelectedTargetLanguageMapping,
				SourceTradosCode = mapping.SourceLanguageCode.TradosCode,
				TargetTradosCode = mapping.TargetLanguageCode.TradosCode,
				Models = mapping.EngineModels,
				SelectedModel = selectedModel,
				Dictionaries = dictionaries,
				SelectedDictionary = selectedDictionary
			};

			return languageMappingModel;
		}

		private void ValidateEngineExistence(InternalLanguageMapping mapping)
		{
			// attempt to recover the language model from the secondary target language code
			if (NoEngineFound(mapping.EngineModels) && mapping.TargetLanguageMappings.Count > 1)
			{
				var secondaryLanguageCode = mapping.TargetLanguageMappings.FirstOrDefault(a => a.CodeName != mapping.SelectedTargetLanguageMapping.CodeName);

				var secondaryEngineModels = LanguageMappingsService.GetTranslationModels(mapping.SelectedSourceLanguageMapping, secondaryLanguageCode,
					mapping.SourceLanguageCode.TradosCode, mapping.TargetLanguageCode.TradosCode);

				if (secondaryEngineModels.Any())
				{
					mapping.EngineModels = secondaryEngineModels;
					mapping.SelectedTargetLanguageMapping = secondaryLanguageCode;
				}
			}

			// attempt to recover the language model from the secondary source language code
			if (NoEngineFound(mapping.EngineModels) && mapping.SourceLanguageMappings.Count > 1)
			{
				var secondarySourceLanguageCode = mapping.SourceLanguageMappings.FirstOrDefault(a => a.CodeName != mapping.SelectedSourceLanguageMapping.CodeName);

				var secondaryEngineModels = LanguageMappingsService.GetTranslationModels(secondarySourceLanguageCode, mapping.SelectedTargetLanguageMapping,
					mapping.SourceLanguageCode.TradosCode, mapping.TargetLanguageCode.TradosCode);

				if (secondaryEngineModels.Any())
				{
					mapping.EngineModels = secondaryEngineModels;
					mapping.SelectedSourceLanguageMapping = secondarySourceLanguageCode;
				}
				else if (mapping.TargetLanguageMappings.Count > 1)
				{
					// attempt to recover the language model from the secondary source OR target language code
					var secondaryTargetLanguageCode = mapping.TargetLanguageMappings.FirstOrDefault(a => a.CodeName != mapping.SelectedTargetLanguageMapping?.CodeName);

					secondaryEngineModels = LanguageMappingsService.GetTranslationModels(
						secondarySourceLanguageCode, secondaryTargetLanguageCode, mapping.SourceLanguageCode.TradosCode,
						mapping.TargetLanguageCode.TradosCode);

					if (secondaryEngineModels.Any())
					{
						mapping.EngineModels = secondaryEngineModels;
						mapping.SelectedSourceLanguageMapping = secondarySourceLanguageCode;
						mapping.SelectedTargetLanguageMapping = secondaryTargetLanguageCode;
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

		private void ActivateRatingController()
		{
			if (_rateItController != null)
			{
				_rateItController.RateIt.IsSendFeedbackEnabled = Options.SendFeedback;
				return;
			}

			try
			{
				Application.Current?.Dispatcher?.Invoke(() =>
				{
					_rateItController = SdlTradosStudio.Application.GetController<RateItController>();

					if (_rateItController != null)
					{
						_rateItController.RateIt.SetTranslationService(TranslationService);
						_rateItController.RateIt.IsSendFeedbackEnabled = Options.SendFeedback; 
						_rateItController.Activate();
					}
				});
			}
			catch
			{
				// catch all; unable to locate the controller
			}
		}
	}
}