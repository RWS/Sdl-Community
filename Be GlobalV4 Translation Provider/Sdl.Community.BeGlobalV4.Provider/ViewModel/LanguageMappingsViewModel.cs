using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Sdl.Community.MTCloud.Languages.Provider.Model;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.Community.MTCloud.Provider.Studio;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MTCloud.Provider.ViewModel
{
	public class LanguageMappingsViewModel : BaseViewModel
	{

		private readonly Languages.Provider.Languages _languages;	
		private readonly LanguageMappingsService _languageMappingService;		
		private readonly OptionsWindowModel _beGlobalWindowViewModel;
		private readonly LanguagePair[] _languagePairs;
		private readonly List<MTCloudDictionary> _mtCloudDictionaries;

		private bool _reSendChecked;
		private LanguageMappingModel _selectedLanguageMapping;
		private ObservableCollection<LanguageMappingModel> _languageMappings;
		private bool _isWaiting;

		private ICommand _resetLanguageMappingsCommand;
		
		public LanguageMappingsViewModel(SdlMTCloudTranslationOptions options, OptionsWindowModel beGlobalWindowViewModel, 
			LanguagePair[] languagePairs, Languages.Provider.Languages languages)
		{
			_mtCloudDictionaries = new List<MTCloudDictionary>();
			_languages = languages;
			_languagePairs = languagePairs;

			Options = options;			
			_languageMappings = new ObservableCollection<LanguageMappingModel>();
			_beGlobalWindowViewModel = beGlobalWindowViewModel;

			_languageMappingService = new LanguageMappingsService();

			if (Options != null)
			{
				ReSendChecked = options.ResendDrafts;				
			}

			BindingOperations.EnableCollectionSynchronization(LanguageMappings, _languageMappings);
		}

		public SdlMTCloudTranslationOptions Options { get; set; }
	
		public ObservableCollection<LanguageMappingModel> LanguageMappings
		{
			get => _languageMappings;
			set
			{
				_languageMappings = value;
				OnPropertyChanged(nameof(LanguageMappings));
			}
		}			

		public List<LangMappingMTCode> MTCodeSourceList = new List<LangMappingMTCode>();

		public List<LangMappingMTCode> MTCodeTargetList = new List<LangMappingMTCode>();

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

		public bool IsWaiting
		{
			get => _isWaiting;
			set
			{
				_isWaiting = value;
				OnPropertyChanged(nameof(IsWaiting));
			}
		}

		public ICommand ResetLanguageMappingsCommand => _resetLanguageMappingsCommand ?? (_resetLanguageMappingsCommand = new RelayCommand(ResetLanguageMappings));
		
		/// <summary>
		/// Load the language mapping settings from .sdlproj settings group
		/// </summary>
		public void LoadLanguageMappings()
		{
			var languageMappings = _languageMappingService.GetLanguageMappingSettings();

			if (languageMappings != null)
			{
				var currentSettings = languageMappings.LanguageMappings?.ToList();
				if (currentSettings.Any() && _languagePairs.Length.Equals(currentSettings.Count()))
				{
					var areLanguagesEquals = currentSettings.All(c => _languagePairs.ToList().Any(l => l.SourceCulture.Name.Equals(c.SourceTradosCode)
					  && (l.TargetCulture.Name.Equals(c.TargetTradosCode))));
					if (areLanguagesEquals)
					{
						// clear the current LanguageMappings list to avoid duplications inside the grid and load them from ProjectGroup configuration
						LanguageMappings.Clear();


						var mtCodes = _languages.GetLanguages();

						foreach (var languageMapping in currentSettings)
						{
							LanguageMappings.Add(languageMapping);

							var missingCodes = LoadNewCodes(languageMapping, mtCodes);

							// set the SelectedModelOption of the current LanguageMappings collection (otherwise it will not shown in the grid)
							var selectedLangModel = LanguageMappings.FirstOrDefault(l => l.ProjectLanguagePair.Equals(languageMapping.ProjectLanguagePair));
							var langMappingIndex = LanguageMappings.IndexOf(selectedLangModel);
							var selectedModelOption = LanguageMappings[langMappingIndex].Engines?.FirstOrDefault(e => e.DisplayName.Equals(selectedLangModel?.SelectedModelOption?.DisplayName));
							LanguageMappings[langMappingIndex].SelectedModelOption = selectedModelOption;

							var selectedDictionary = LanguageMappings[langMappingIndex].MTCloudDictionaries?.FirstOrDefault(e => e.Name.Equals(selectedLangModel?.SelectedMTCloudDictionary?.Name));
							LanguageMappings[langMappingIndex].SelectedMTCloudDictionary = selectedDictionary;
													   
							AddMissingCode(missingCodes.Keys.First(), LanguageMappings[langMappingIndex].MTCodesSource);
							AddMissingCode(missingCodes.Values.First(), LanguageMappings[langMappingIndex].MTCodesTarget);

							var selectedMTCodeSource = LanguageMappings[langMappingIndex].MTCodesSource?.FirstOrDefault(e => e.CodeName.Equals(selectedLangModel?.SelectedMTCodeSource?.CodeName));
							selectedMTCodeSource.Flag = SetLanguageFlag(new CultureInfo(languageMapping.SourceTradosCode));
							var selectedMTCodeTarget = LanguageMappings[langMappingIndex].MTCodesTarget?.FirstOrDefault(e => e.CodeName.Equals(selectedLangModel?.SelectedMTCodeTarget?.CodeName));
							selectedMTCodeTarget.Flag = SetLanguageFlag(new CultureInfo(languageMapping.TargetTradosCode));
							LanguageMappings[langMappingIndex].SelectedMTCodeSource = selectedMTCodeSource;
							LanguageMappings[langMappingIndex].SelectedMTCodeTarget = selectedMTCodeTarget;

							SetMTFlag(LanguageMappings[langMappingIndex]);
						}
					}
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
			if (savedSettings != null)
			{
				_languageMappingService.RemoveLanguageMappingSettings();
				savedSettings.LanguageMappings = LanguageMappings;
				_languageMappingService.SaveLanguageMappingSettings(savedSettings);
			}
		}

		/// <summary>
		/// Load MT Cloud dictionaries for the current user
		/// </summary>
		public void LoadDictionaries()
		{
			_mtCloudDictionaries.Clear();
			var beGlobalTranslator = new SdlMTCloudTranslator(Constants.MTCloudTranslateAPIUri, Options);
			var accountId = Options.AuthenticationMethod.Equals("ClientLogin") ? beGlobalTranslator.GetClientInformation() : beGlobalTranslator.GetUserInformation();
		
			var dictionaries = beGlobalTranslator.GetDictionaries(accountId);
			if (dictionaries != null)
			{
				foreach (var item in dictionaries?.Dictionaries)
				{
					var dictionary = new MTCloudDictionary
					{
						Description = item.Description,
						DictionaryId = item.DictionaryId,
						Name = item.Name,
						Source = item.Source,
						Target = item.Target
					};
					_mtCloudDictionaries.Add(dictionary);
				}
				foreach (var languageMapping in LanguageMappings)
				{

					var languageDictionaries = _mtCloudDictionaries
						.Where(d => languageMapping.MTCodesSource.Any(s => s.CodeName.Equals(d.Source)))
						.Where(d => languageMapping.MTCodesTarget.Any(t => t.CodeName.Equals(d.Target))).ToList();

					if (languageDictionaries.Count == 0)
					{
						var dictionary = new MTCloudDictionary
						{
							Name = Constants.NoAvailableDictionary,
							DictionaryId = string.Empty
						};
						languageMapping.MTCloudDictionaries = new ObservableCollection<MTCloudDictionary>();
						languageMapping.MTCloudDictionaries.Add(dictionary);
						languageMapping.SelectedMTCloudDictionary = dictionary;
					}
					else
					{
						languageMapping.MTCloudDictionaries = new ObservableCollection<MTCloudDictionary>(languageDictionaries);
						if (!languageMapping.MTCloudDictionaries.Any(d => d.Name.Equals(Constants.NoDictionary)))
						{
							languageMapping.MTCloudDictionaries.Insert(0, new MTCloudDictionary { Name = Constants.NoDictionary, DictionaryId = string.Empty });
						}
						languageMapping.SelectedMTCloudDictionary = languageDictionaries[0];
					}
				}
			}
		}

		/// <summary>
		/// Reset the language mappings to the default values
		/// </summary>
		/// <param name="parameter"></param>
		private async void ResetLanguageMappings(object parameter)
		{
			IsWaiting = true;
			if (LanguageMappings != null)
			{
				await Task.Run(async () =>
				{
					_languageMappingService.RemoveLanguageMappingSettings();

					var savedSettings = _languageMappingService.GetLanguageMappingSettings();
					_languageMappingService.SaveLanguageMappingSettings(savedSettings);

					LanguageMappings.Clear();
					LoadProjectLanguagePairs();
					_beGlobalWindowViewModel?.ValidateWindow(false);
				}).ConfigureAwait(true);
				IsWaiting = false;
			}	
		}
		
		/// <summary>
		/// Load to the existing language mappings settings, the new codes which were added using the MTCodes window
		/// </summary>
		/// <param name="languageMapping">language mapping</param>
		/// <param name="mtCodes">mtcodes collection</param>
		/// <returns>dictionary of lists with missing source and target codes</returns>
		private Dictionary<List<LangMappingMTCode>, List<LangMappingMTCode>> LoadNewCodes(
			LanguageMappingModel languageMapping, 
			IReadOnlyCollection<MTCloudLanguage> mtCodes)
		{
			var sourceModel = mtCodes.FirstOrDefault(s => s.TradosCode.Equals(languageMapping.SourceTradosCode));
			var targetModel = mtCodes.FirstOrDefault(s => s.TradosCode.Equals(languageMapping.TargetTradosCode));

			var sCodes = new List<string>();
			sCodes.Add(sourceModel?.MTCode);
			sCodes.Add(sourceModel?.MTCodeLocale);

			var tCodes = new List<string>();
			tCodes.Add(targetModel?.MTCode);
			tCodes.Add(targetModel?.MTCodeLocale);

			var missingSourceCodes = sCodes.Where(s => languageMapping.MTCodesSource.All(m => m.CodeName != s)).ToList();
			var langMappingMTSource = SetNewCodes(missingSourceCodes, languageMapping.SourceTradosCode);

			var missingTargetCodes = tCodes.Where(t => languageMapping.MTCodesTarget.All(m => m.CodeName != t)).ToList();
			var langMappingMTTarget = SetNewCodes(missingTargetCodes, languageMapping.TargetTradosCode);

			var result = new Dictionary<List<LangMappingMTCode>, List<LangMappingMTCode>>();
			result.Add(langMappingMTSource, langMappingMTTarget);
			return result;
		}

		/// <summary>
		/// Add missing code to existing LanguageMappings configurations
		/// </summary>
		/// <param name="missingCodes"></param>
		/// <param name="langMappingIndex"></param>
		private void AddMissingCode(List<LangMappingMTCode> missingCodes, ObservableCollection<LangMappingMTCode> languageMappingCodes)
		{
			foreach (var code in missingCodes)
			{
				if (!string.IsNullOrEmpty(code.CodeName))
				{
					languageMappingCodes.Add(code);
				}
			}
		}

		/// <summary>
		/// Set the missing codes which were new added withing the SDL MT Codes window.
		/// </summary>
		/// <param name="missingCodes">missing codes</param>
		/// <param name="tradosCode">corresponding tradosCode for source and target languages</param>
		/// <returns></returns>
		private List<LangMappingMTCode> SetNewCodes(List<string> missingCodes, string tradosCode )
		{
			var langMappingMT = new List<LangMappingMTCode>();
			foreach (var item in missingCodes)
			{
				var cultureInfo = new CultureInfo(tradosCode);
				langMappingMT.Add(new LangMappingMTCode { CodeName = item, Flag = SetLanguageFlag(cultureInfo) });
			}
			return langMappingMT;
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
				var mtCodes = _languages.GetLanguages();

				var sourceLanguage = _languagePairs?[0].SourceCulture;
				var mtCodeSource = mtCodes?.FirstOrDefault(s => s.TradosCode.Equals(sourceLanguage?.Name)
				|| s.TradosCode.Equals(sourceLanguage?.ThreeLetterISOLanguageName));
				if (mtCodeSource != null)
				{
					MTCodeSourceList.Clear();
					MTCodeSourceList.Add(new LangMappingMTCode { CodeName = mtCodeSource.MTCode, Flag = SetLanguageFlag(sourceLanguage) });
					if (!string.IsNullOrEmpty(mtCodeSource.MTCodeLocale))
					{
						MTCodeSourceList.Add(new LangMappingMTCode { CodeName = mtCodeSource.MTCodeLocale, Flag = SetLanguageFlag(sourceLanguage) });
					}
				}

				foreach (var langPair in _languagePairs)
				{
					MTCodeTargetList.Clear();
					var languagePair = $"{sourceLanguage.DisplayName} - {langPair.TargetCulture?.DisplayName}";

					var mtCodeTarget = mtCodes?.FirstOrDefault(s => s.TradosCode.Equals(langPair.TargetCulture?.Name)
					|| s.TradosCode.Equals(langPair?.TargetCulture.ThreeLetterISOLanguageName));
					if (mtCodeTarget != null)
					{
						MTCodeTargetList.Add(new LangMappingMTCode { CodeName = mtCodeTarget.MTCode, Flag = SetLanguageFlag(langPair.TargetCulture)});
						if (!string.IsNullOrEmpty(mtCodeTarget.MTCodeLocale))
						{
							MTCodeTargetList.Add(new LangMappingMTCode { CodeName = mtCodeTarget.MTCodeLocale, Flag = SetLanguageFlag(langPair.TargetCulture) });
						}

						var languageMappingModel = new LanguageMappingModel
						{
							ProjectLanguagePair = languagePair,
							MTCodesSource = new ObservableCollection<LangMappingMTCode>(MTCodeSourceList),
							SelectedMTCodeSource = MTCodeSourceList[0],
							MTCodesTarget = new ObservableCollection<LangMappingMTCode>(MTCodeTargetList),
							SelectedMTCodeTarget = MTCodeTargetList[0],
							Engines = new ObservableCollection<TranslationModel>(),
							TargetTradosCode = mtCodeTarget.TradosCode,
							SourceTradosCode = mtCodeSource.TradosCode,
							MTCloudDictionaries = new ObservableCollection<MTCloudDictionary>()
						};
						LanguageMappings.Add(languageMappingModel);
					}
				}
				LoadDictionaries();
			}
		}

		/// <summary>
		/// Set Language Flag for Source MTCode and Target MTCode
		/// </summary>
		/// <param name="cultureInfo">cultureInfo</param>
		/// <returns>image</returns>
		private Image SetLanguageFlag(CultureInfo cultureInfo)
		{
			return new Language(cultureInfo).GetFlagImage();
		}

		/// <summary>
		/// Set the flag for each Source MTCode and Target MTCode when the Language Mappings grid is showing
		/// </summary>
		/// <param name="languageMapping">languageMapping</param>
		private void SetMTFlag(LanguageMappingModel languageMapping)
		{
			foreach(var item in languageMapping.MTCodesSource)
			{
				item.Flag = SetLanguageFlag(new CultureInfo(languageMapping.SourceTradosCode));
			}

			foreach (var item in languageMapping.MTCodesTarget)
			{
				item.Flag = SetLanguageFlag(new CultureInfo(languageMapping.TargetTradosCode));
			}
		}
	}
}