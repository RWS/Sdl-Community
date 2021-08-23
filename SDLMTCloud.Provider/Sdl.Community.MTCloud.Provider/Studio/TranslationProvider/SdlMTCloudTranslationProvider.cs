using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using NLog;
using Sdl.Community.MTCloud.Languages.Provider.Interfaces;
using Sdl.Community.MTCloud.Languages.Provider.Model;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.Settings.Events;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using LogManager = NLog.LogManager;

namespace Sdl.Community.MTCloud.Provider.Studio.TranslationProvider
{
	public class SdlMTCloudTranslationProvider : ITranslationProvider
	{
		private readonly EditorController _editorController;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly RateItController _rateItController;
		private ProjectInfo _currentProject;
		private LanguagePair _languageDirection;
		private LanguageMappingsService _languageMappingsService;

		public SdlMTCloudTranslationProvider(Uri uri, string translationProviderState, ITranslationService translationService,
		 ILanguageProvider languageProvider)
		{
			Uri = uri;
			LanguageProvider = languageProvider;
			TranslationService = translationService;

			Options = new Options
			{
				AutoSendFeedback = true,
				ResendDraft = true,
				SendFeedback = true,
			};

			_editorController = MtCloudApplicationInitializer.EditorController;
			_rateItController = MtCloudApplicationInitializer.RateItController;

			var projectsController = MtCloudApplicationInitializer.ProjectsController;

			if (projectsController is not null)
			{
				projectsController.CurrentProjectChanged -= ProjectsController_CurrentProjectChanged;
				projectsController.CurrentProjectChanged += ProjectsController_CurrentProjectChanged;
				_currentProject = projectsController?.CurrentProject?.GetProjectInfo();
			}

			LoadState(translationProviderState);
			ActivateRatingController();

			MtCloudApplicationInitializer.Subscribe<TranslationProviderStatusChanged>(Settings_TranslationProviderStatusChanged);
		}

		public bool IsReadOnly => true;

		public ITranslationProviderLanguageDirection LanguageDirectionProvider { get; private set; }

		public ILanguageMappingsService LanguageMappingsService => _languageMappingsService ??= new LanguageMappingsService(TranslationService);

		public ILanguageProvider LanguageProvider { get; }

		public string Name => PluginResources.Plugin_NiceName;

		public Options Options
		{
			get => TranslationService.Options;
			set
			{
				TranslationService.Options = value;
				if (TranslationService.Options != null)
				{
					TranslationService.Options.PropertyChanged +=
						(s, e) => { SwitchRateTranslationsControllerVisibility(((Options)s).SendFeedback); };
				}
			}
		}

		public ProviderStatusInfo StatusInfo => new ProviderStatusInfo(true, PluginResources.Plugin_NiceName);

		public bool SupportsConcordanceSearch => false;

		public bool SupportsDocumentSearches => false;

		public bool SupportsFilters => false;

		public bool SupportsFuzzySearch => false;

		public bool SupportsMultipleResults => false;

		public bool SupportsPenalties => true;

		public bool SupportsPlaceables => false;

		public bool SupportsScoring => false;

		public bool SupportsSearchForTranslationUnits => true;

		public bool SupportsSourceConcordanceSearch => false;

		public bool SupportsStructureContext => false;

		public bool SupportsTaggedInput => true;

		public bool SupportsTargetConcordanceSearch => false;

		public bool SupportsTranslation => true;

		public bool SupportsUpdate => false;

		public bool SupportsWordCounts => false;

		public TranslationMethod TranslationMethod => TranslationMethod.MachineTranslation;

		public ITranslationService TranslationService { get; }

		public Uri Uri { get; internal set; }

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

		public void LoadState(string translationProviderState)
		{
			if (string.IsNullOrWhiteSpace(translationProviderState)) return;
			try
			{
				Options = JsonConvert.DeserializeObject<Options>(translationProviderState);
			}
			catch
			{
			}
		}

		public void RefreshStatusInfo()
		{
		}

		public string SerializeState()
		{
			var json = JsonConvert.SerializeObject(Options ?? new Options());
			return json;
		}

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

		private static bool? GetTpStatus()
		{
			var tpStatus =
				Application.Current.Dispatcher.Invoke(
					() =>
						MtCloudApplicationInitializer.GetProjectInProcessing()?.GetTranslationProviderConfiguration().Entries
							.FirstOrDefault(
								e => e.MainTranslationProvider.Uri.ToString().Contains(PluginResources.SDLMTCloudUri))?.MainTranslationProvider
							.Enabled);
			return tpStatus;
		}

		private void ActivateRatingController()
		{
			if (!MtCloudApplicationInitializer.IsStudioRunning()) return;

			var tpStatus = GetTpStatus();
			if (tpStatus is null) return;

			try
			{
				Application.Current?.Dispatcher?.Invoke(SetTranslationServiceOnRateItControl);
			}
			catch
			{
				// catch all; unable to locate the controller
			}

			SwitchRateTranslationsControllerVisibility(tpStatus.Value);
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
			return Options?.LanguageMappings?.Any(l => l.SourceTradosCode.Equals(languagePair.SourceCulture.Name)
													&& l.TargetTradosCode.Equals(languagePair.TargetCulture.Name)) ?? false;
		}

		private bool NoEngineFound(IReadOnlyList<TranslationModel> engineModels)
		{
			return engineModels.Count == 1 &&
				   engineModels[0].DisplayName == PluginResources.Message_No_model_available;
		}

		private void ProjectsController_CurrentProjectChanged(object sender, EventArgs e)
		{
			if (sender is not ProjectsController projController) return;
			if (projController.CurrentProject is null) return;

			var newProject = projController.CurrentProject.GetProjectInfo();
			if (newProject == _currentProject) return;
			_currentProject = newProject;

			var tpState =
				projController.CurrentProject.GetTranslationProviderConfiguration().Entries.FirstOrDefault(
					entry => entry.MainTranslationProvider.Uri.ToString().Contains("sdlmtcloud"))?.MainTranslationProvider.State;

			var currentLanguagePair = new LanguagePair(_currentProject.SourceLanguage.CultureInfo,
				_editorController?.ActiveDocument?.ActiveFile?.Language.CultureInfo);

			if (_editorController is null) return;

			LoadState(tpState);
			if (tpState != null && currentLanguagePair.TargetCulture != null)
			{
				GetMTCloudLanguagePair(currentLanguagePair);
			}
			ActivateRatingController();
		}

		private void Settings_TranslationProviderStatusChanged(TranslationProviderStatusChanged tpInfo)
		{
			if (!tpInfo.TpUri.ToString().Contains(PluginResources.SDLMTCloudUri)) return;
			SwitchRateTranslationsControllerVisibility(tpInfo.NewStatus ?? false);
		}

		private void SetTranslationServiceOnRateItControl()
		{
			_rateItController?.RateIt.SetTranslationService(TranslationService);
		}

		private void SwitchRateTranslationsControllerVisibility(bool onOffSwitch)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				if (!onOffSwitch)
				{
					_rateItController?.Hide();
				}
				if (onOffSwitch)
				{
					_rateItController?.Activate();
				}
			});
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
	}
}