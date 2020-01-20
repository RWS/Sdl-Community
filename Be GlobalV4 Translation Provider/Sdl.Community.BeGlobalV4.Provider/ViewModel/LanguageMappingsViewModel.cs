using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class LanguageMappingsViewModel : BaseViewModel
	{
		private bool _reSendChecked;
		private ProjectsController _projectController;
		private LanguageMappingModel _selectedLanguageMapping;
		private LanguageMappingsService _languageMappingService;
		private ObservableCollection<LanguageMappingModel> _languageMappings;

		public LanguageMappingsViewModel(BeGlobalTranslationOptions options)
		{
			Options = options;
			_languageMappings = new ObservableCollection<LanguageMappingModel>();
			_languageMappingService = new LanguageMappingsService();

			if (Options != null)
			{
				ReSendChecked = options.ResendDrafts;				
			}
		}
		
		public BeGlobalTranslationOptions Options { get; set; }
	
		public ObservableCollection<LanguageMappingModel> LanguageMappings
		{
			get => _languageMappings;
			set
			{
				_languageMappings = value;
				OnPropertyChanged(nameof(LanguageMappings));
			}
		}

		public List<string> MTCodeSourceList = new List<string>();
		public List<string> MTCodeTargetList = new List<string>();

		public LanguageMappingModel SelectedLanguageMapping
		{
			get => _selectedLanguageMapping;
			set
			{
				_selectedLanguageMapping = value;
				OnPropertyChanged(nameof(SelectedLanguageMapping));
			}
		}

		public bool ReSendChecked
		{
			get => _reSendChecked;
			set
			{
				_reSendChecked = value;
				if (LanguageMappings.Any())
				{
					Options.ResendDrafts = value;
				}
				OnPropertyChanged(nameof(ReSendChecked));
			}
		}

		/// <summary>
		/// Load the language mapping settings from .sdlproj settings group
		/// </summary>
		public void LoadLanguageMappings()
		{
			_projectController = AppInitializer.GetProjectController();
			var currentSettings = _languageMappingService.GetLanguageMappingSettings();
			if (currentSettings != null && currentSettings.LanguageMappings.Any())
			{
				// clear current LanguageMappings list to avoid duplications inside the grid
				LanguageMappings.Clear();

				foreach (var languageMapping in currentSettings.LanguageMappings)
				{
					LanguageMappings.Add(languageMapping);

					// set the SelectedModelOption of the current LanguageMappings collection (otherwise it will not shown in the grid)
					var selectedLangModel = LanguageMappings.FirstOrDefault(l => l.ProjectLanguagePair.Equals(languageMapping.ProjectLanguagePair));
					var langMappingIndex = LanguageMappings.IndexOf(selectedLangModel);
					var selectedModelOption = LanguageMappings[langMappingIndex].Engines.FirstOrDefault(e => e.DisplayName.Equals(selectedLangModel.SelectedModelOption.DisplayName));
					LanguageMappings[langMappingIndex].SelectedModelOption = selectedModelOption;
				}
			}
			LoadProjectLanguagePairs();
		}

		/// <summary>
		/// Save the language mapping settings inside the .sdlproj settings group
		/// </summary>
		public void SaveLanguageMappingSettings()
		{
			var savedSettings = _languageMappingService.GetLanguageMappingSettings();
			_languageMappingService.RemoveLanguageMappingSettings();
			savedSettings.LanguageMappings = LanguageMappings;
			_languageMappingService.SaveLanguageMappingSettings(savedSettings);
		}

		/// <summary>
		/// Load project language pairs and the default details
		/// (the information is loaded only when no other LanguageMappings settings were already made and saved inside the .sdlproj settings group)
		/// </summary>
		private void LoadProjectLanguagePairs()
		{
			if (LanguageMappings == null || !LanguageMappings.Any())
			{
				// load the MTCode (the load is needed, because user might add/remove codes from MTCodes grid
				var mtCodes = AppInitializer.GetMTCodes();

				var currentProjectInfo = _projectController?.CurrentProject?.GetProjectInfo();
				var sourceLanguage = currentProjectInfo?.SourceLanguage;
				var targetLanguages = currentProjectInfo?.TargetLanguages;
				var mtCodeSource = mtCodes?.FirstOrDefault(s => s.TradosCode.Equals(sourceLanguage?.CultureInfo?.Name)
				|| s.TradosCode.Equals(sourceLanguage?.IsoAbbreviation));
				if (mtCodeSource != null)
				{
					MTCodeSourceList.Add(mtCodeSource.MTCodeMain);
					if (!string.IsNullOrEmpty(mtCodeSource.MTCodeLocale))
					{
						MTCodeSourceList.Add(mtCodeSource.MTCodeLocale);
					}
				}

				foreach (var targetLanguage in targetLanguages)
				{
					var languagePair = $"{sourceLanguage.DisplayName} - {targetLanguage.DisplayName}";

					var mtCodeTarget = mtCodes?.FirstOrDefault(s => s.TradosCode.Equals(targetLanguage?.CultureInfo?.Name)
					|| s.TradosCode.Equals(targetLanguage?.IsoAbbreviation));
					if (mtCodeTarget != null)
					{
						MTCodeTargetList.Add(mtCodeTarget.MTCodeMain);
						if (!string.IsNullOrEmpty(mtCodeTarget.MTCodeLocale))
						{
							MTCodeTargetList.Add(mtCodeTarget.MTCodeLocale);
						}

						var languageMappingModel = new LanguageMappingModel
						{
							ProjectLanguagePair = languagePair,
							MTCodesSource = new ObservableCollection<string>(MTCodeSourceList),
							SelectedMTCodeSource = MTCodeSourceList[0],
							MTCodesTarget = new ObservableCollection<string>(MTCodeTargetList),
							SelectedMTCodeTarget = MTCodeTargetList[0],
							Engines = new ObservableCollection<TranslationModel>()
						};
						LanguageMappings.Add(languageMappingModel);						
						MTCodeTargetList.Clear();
					}
				}
			}
		}
	}
}