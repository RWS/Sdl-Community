using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Sdl.Community.MTCloud.Languages.Provider.Model;
using Sdl.Community.MTCloud.Provider.Commands;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Studio;
using Sdl.Community.MTCloud.Provider.View;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.ViewModel
{
	public class OptionsViewModel : BaseViewModel, IDisposable
	{
		public static readonly Log Log = Log.Instance;

		private readonly SdlMTCloudTranslationProvider _provider;
		private readonly ProjectsController _projectsController;
		private readonly LanguagePair[] _languagePairs;

		private ICommand _saveCommand;
		private ICommand _resetLanguageMappingsCommand;

		private string _message;
		private bool _reSendChecked;
		private LanguageMappingModel _selectedLanguageMapping;
		private ObservableCollection<LanguageMappingModel> _languageMappings;
		private bool _isWaiting;

		public OptionsViewModel(OptionsWindow window, SdlMTCloudTranslationProvider provider, LanguagePair[] languagePairs, Languages.Provider.Languages languages)
		{
			_provider = provider;

			Window = window;
			Languages = languages;

			_languagePairs = languagePairs;

			_languageMappings = new ObservableCollection<LanguageMappingModel>();

			ReSendChecked = provider.Options?.ResendDraft ?? false;

			BindingOperations.EnableCollectionSynchronization(LanguageMappings, _languageMappings);

			LoadLanguageMappings();

			_projectsController = AppInitializer.GetProjectController();
			_projectsController.ProjectsChanged += ProjectController_ProjectsChanged;
		}

		public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(Save));

		public OptionsWindow Window { get; }

		public Languages.Provider.Languages Languages { get; }

		public string Message
		{
			get => _message;
			set
			{
				if (_message == value)
				{
					return;
				}

				_message = value;
				OnPropertyChanged(nameof(Message));
			}
		}

		public bool IsValidData(bool savePressed)
		{
			Message = string.Empty;
			_provider.Options.ResendDraft = ReSendChecked;

			try
			{
				if (!LanguageMappings.Any())
				{
					Message = Constants.EnginesSelectionMessage;
					return false;
				}


				return IsEngineValid(savePressed);
			}
			catch (Exception ex)
			{
				Message = ex.Message.Contains(Constants.TokenFailed) || ex.Message.Contains(Constants.NullValue)
					? Constants.CredentialsNotValid
					: ex.Message;

				Log.Logger.Error($"{Constants.IsWindowValid} {ex.Message}\n {ex.StackTrace}");
			}

			return false;
		}

		private void Save(object parameter)
		{
			try
			{
				Mouse.OverrideCursor = Cursors.Wait;

				SaveLanguageMappingSettings();

				Dispose();

				WindowCloser.SetDialogResult(Window, true);
				Window.Close();
			}
			finally
			{
				Mouse.OverrideCursor = Cursors.Arrow;
			}
		}

		private bool IsEngineValid(bool savePressed)
		{
			if (savePressed || LanguageMappings.Any())
			{
				var isEngineSetup = ValidateEnginesSetup();
				if (!isEngineSetup)
				{
					Message = Constants.CredentialsAndInternetValidation;
					return false;
				}

				if (!LanguageMappings.Any(l => l.Engines.Any()))
				{
					Message = Constants.NoEnginesLoaded;
					return false;
				}

				return true;
			}

			return true;
		}

		private void ProjectController_ProjectsChanged(object sender, EventArgs e)
		{
			var projectsController = (ProjectsController)sender;
			var lastCreatedProj = projectsController?.GetAllProjects()?.LastOrDefault();
			var currentProj = projectsController?.CurrentProject;

			// Save language mappings configuration for the last created project
			// (used when user adds the provider through Project creation wizard)
			if (currentProj != null && currentProj.FilePath.Equals(lastCreatedProj?.FilePath))
			{
				SaveLanguageMappingSettings();
			}
		}

		public void Dispose()
		{
			if (_projectsController != null)
			{
				_projectsController.ProjectsChanged -= ProjectController_ProjectsChanged;
			}
		}


		public ObservableCollection<LanguageMappingModel> LanguageMappings
		{
			get => _languageMappings;
			set
			{
				_languageMappings = value;
				OnPropertyChanged(nameof(LanguageMappings));
			}
		}

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
				if (_reSendChecked == value)
				{
					return;
				}

				_reSendChecked = value;
				_provider.Options.ResendDraft = value;

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

		public ICommand ResetLanguageMappingsCommand => _resetLanguageMappingsCommand
														?? (_resetLanguageMappingsCommand = new RelayCommand(ResetLanguageMappings));

		public bool ValidateEnginesSetup()
		{
			//Utils.LogServerIPAddresses();

			//TODO fix!
			var isEngineSet = GetEngineModels(_provider.SubscriptionInfo?.LanguagePairs);

			return isEngineSet;
		}

		private bool GetEngineModels(IReadOnlyCollection<MTCloudLanguagePair> mtCloudlanguagePairs)
		{
			if (mtCloudlanguagePairs != null && LanguageMappings != null)
			{
				foreach (var languageMapping in LanguageMappings)
				{
					//get beGlobalLanguagePairs for the specific source language MTSourceCodes
					var sourcePairs = mtCloudlanguagePairs.Where(b =>
						languageMapping.MTCodesSource.Any(l => b.SourceLanguageId.Equals(l.CodeName)));

					//get beGlobalLanguagePairs for the specific target MTTargetCodes and exiting sourcePairs
					var serviceLanguagePairs =
						sourcePairs.Where(s => languageMapping.MTCodesTarget.Any(l => s.TargetLanguageId.Equals(l.CodeName)));

					var splittedLangPair = Utils.SplitLanguagePair(languageMapping.ProjectLanguagePair);


					var sourceCultureName = _languagePairs
						?.FirstOrDefault(n => n.SourceCulture.DisplayName.Equals(splittedLangPair[0]))?.SourceCulture.Name;

					var targetCultureName = _languagePairs
						?.FirstOrDefault(n => n.TargetCulture.DisplayName.Equals(splittedLangPair[1]))?.TargetCulture.Name;

					if (string.IsNullOrEmpty(sourceCultureName) || string.IsNullOrEmpty(targetCultureName))
					{
						return false;
					}

					foreach (var serviceLanguagePair in serviceLanguagePairs)
					{
						var existingTranslationModel = languageMapping.Engines.FirstOrDefault(e =>
							e.Model.Equals(serviceLanguagePair.Model)
							&& e.DisplayName.Contains(serviceLanguagePair.TargetLanguageId));

						TranslationModel newTranslationModel;
						if (existingTranslationModel == null)
						{
							newTranslationModel = new TranslationModel
							{
								Model = serviceLanguagePair.Model,
								DisplayName =
									$"{serviceLanguagePair.SourceLanguageId}-{serviceLanguagePair.TargetLanguageId} {serviceLanguagePair.DisplayName}"
							};

							newTranslationModel.LanguagesSupported.Add(sourceCultureName, targetCultureName);

							if (!languageMapping.Engines.Any(e => e.DisplayName.Equals(newTranslationModel.DisplayName)))
							{
								// the initialization is needed to display the progress ring while loading the engines
								// after user presses on the "Reset Langauge Mappings to default" button
								var currentEnginesCollection = languageMapping.Engines;
								languageMapping.Engines = new ObservableCollection<TranslationModel>(currentEnginesCollection)
								{
									newTranslationModel

								};

								languageMapping.SelectedModelOption = languageMapping.SelectedModelOption ?? languageMapping.Engines?[0];
							}
						}
					}
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// Load the language mapping settings from .sdlproj settings group
		/// </summary>
		public void LoadLanguageMappings()
		{
			var languageMappings = _provider.TranslationService.LanguageMappingsService.GetLanguageMappingSettings();
			var currentSettings = languageMappings?.LanguageMappings?.ToList();

			if (currentSettings != null && currentSettings.Any() && _languagePairs.Length.Equals(currentSettings.Count))
			{
				var areLanguagesEquals = currentSettings.All(
					c => _languagePairs.ToList().Any(l => l.SourceCulture.Name.Equals(c.SourceTradosCode)
														&& (l.TargetCulture.Name.Equals(c.TargetTradosCode))));

				if (areLanguagesEquals)
				{
					// clear the current LanguageMappings list to avoid duplications inside the grid and load them from ProjectGroup configuration
					LanguageMappings.Clear();

					var mtCodes = Languages.GetLanguages();

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

			LoadProjectLanguagePairs();
			LoadDictionaries();
		}

		/// <summary>
		/// Save the language mapping settings inside the .sdlproj settings group
		/// </summary>
		public void SaveLanguageMappingSettings()
		{
			var savedSettings = _provider.TranslationService.LanguageMappingsService.GetLanguageMappingSettings();
			if (savedSettings != null)
			{
				_provider.TranslationService.LanguageMappingsService.RemoveLanguageMappingSettings();
				savedSettings.LanguageMappings = LanguageMappings;
				_provider.TranslationService.LanguageMappingsService.SaveLanguageMappingSettings(savedSettings);
			}
		}

		/// <summary>
		/// Load MT Cloud dictionaries for the current user
		/// </summary>
		public void LoadDictionaries()
		{
			var cloudDictionaries = new List<MTCloudDictionary>();

			var dictionariesResult = Task.Run(async () => await _provider.TranslationService.GetDictionaries(_provider.ConnectionService.Credential.AccountId)).Result;
			if (dictionariesResult != null)
			{
				foreach (var item in dictionariesResult.Dictionaries)
				{
					var dictionary = new MTCloudDictionary
					{
						Description = item.Description,
						DictionaryId = item.DictionaryId,
						Name = item.Name,
						Source = item.Source,
						Target = item.Target
					};

					cloudDictionaries.Add(dictionary);
				}

				foreach (var languageMapping in LanguageMappings)
				{
					var languageDictionaries = cloudDictionaries
						.Where(d => languageMapping.MTCodesSource.Any(s => s.CodeName.Equals(d.Source)))
						.Where(d => languageMapping.MTCodesTarget.Any(t => t.CodeName.Equals(d.Target))).ToList();

					if (languageDictionaries.Count == 0)
					{
						var dictionary = new MTCloudDictionary
						{
							Name = Constants.NoAvailableDictionary,
							DictionaryId = string.Empty
						};

						languageMapping.MTCloudDictionaries = new ObservableCollection<MTCloudDictionary> { dictionary };
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
				await Task.Run(() =>
				{
					_provider.TranslationService.LanguageMappingsService.RemoveLanguageMappingSettings();

					var savedSettings = _provider.TranslationService.LanguageMappingsService.GetLanguageMappingSettings();
					_provider.TranslationService.LanguageMappingsService.SaveLanguageMappingSettings(savedSettings);

					LanguageMappings.Clear();
					LoadProjectLanguagePairs();
					LoadDictionaries();

					IsValidData(false);

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

			var sCodes = new List<string>
			{
				sourceModel?.MTCode,
				sourceModel?.MTCodeLocale
			};

			var tCodes = new List<string>
			{
				targetModel?.MTCode,
				targetModel?.MTCodeLocale
			};

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
		/// <param name="languageMappingCodes"></param>
		private void AddMissingCode(IEnumerable<LangMappingMTCode> missingCodes, ICollection<LangMappingMTCode> languageMappingCodes)
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
		private List<LangMappingMTCode> SetNewCodes(IEnumerable<string> missingCodes, string tradosCode)
		{
			var mtCodes = new List<LangMappingMTCode>();
			foreach (var item in missingCodes)
			{
				var cultureInfo = new CultureInfo(tradosCode);
				mtCodes.Add(new LangMappingMTCode { CodeName = item, Flag = SetLanguageFlag(cultureInfo) });
			}

			return mtCodes;
		}

		/// <summary>
		/// Load project language pairs and the default details
		/// (the information is loaded only when no other LanguageMappings settings were already made and saved inside the .sdlproj settings group)
		/// </summary>
		private void LoadProjectLanguagePairs()
		{
			if (LanguageMappings != null && LanguageMappings.Any())
			{
				return;
			}

			// load the MTCode (the load is needed, because user might add/remove codes from MTCodes grid
			var mtCodes = Languages.GetLanguages();

			var sourceLanguage = _languagePairs?[0].SourceCulture;
			var mtCodeSource = mtCodes?.FirstOrDefault(s => s.TradosCode.Equals(sourceLanguage?.Name));

			var mtCodeSourceList = new List<LangMappingMTCode>();
			if (mtCodeSource != null)
			{
				mtCodeSourceList.Add(new LangMappingMTCode
				{
					CodeName = mtCodeSource.MTCode,
					Flag = SetLanguageFlag(sourceLanguage)
				});

				if (!string.IsNullOrEmpty(mtCodeSource.MTCodeLocale))
				{
					mtCodeSourceList.Add(new LangMappingMTCode
					{
						CodeName = mtCodeSource.MTCodeLocale,
						Flag = SetLanguageFlag(sourceLanguage)
					});
				}
			}

			if (_languagePairs == null || mtCodeSource == null)
			{
				return;
			}

			foreach (var langPair in _languagePairs)
			{
				var mtCodeTargetList = new List<LangMappingMTCode>();

				var languagePair = $"{sourceLanguage?.DisplayName} - {langPair.TargetCulture?.DisplayName}";

				var mtCodeTarget = mtCodes?.FirstOrDefault(s => s.TradosCode.Equals(langPair.TargetCulture?.Name));
				if (mtCodeTarget != null)
				{
					mtCodeTargetList.Add(new LangMappingMTCode
					{
						CodeName = mtCodeTarget.MTCode,
						Flag = SetLanguageFlag(langPair.TargetCulture)
					});

					if (!string.IsNullOrEmpty(mtCodeTarget.MTCodeLocale))
					{
						mtCodeTargetList.Add(new LangMappingMTCode
						{
							CodeName = mtCodeTarget.MTCodeLocale,
							Flag = SetLanguageFlag(langPair.TargetCulture)
						});
					}

					var languageMappingModel = new LanguageMappingModel
					{
						ProjectLanguagePair = languagePair,
						MTCodesSource = new ObservableCollection<LangMappingMTCode>(mtCodeSourceList),
						SelectedMTCodeSource = mtCodeSourceList[0],
						MTCodesTarget = new ObservableCollection<LangMappingMTCode>(mtCodeTargetList),
						SelectedMTCodeTarget = mtCodeTargetList[0],
						Engines = new ObservableCollection<TranslationModel>(),
						TargetTradosCode = mtCodeTarget.TradosCode,
						SourceTradosCode = mtCodeSource.TradosCode,
						MTCloudDictionaries = new ObservableCollection<MTCloudDictionary>()
					};

					LanguageMappings.Add(languageMappingModel);
				}
			}
		}

		/// <summary>
		/// Set Language Flag for Source MTCode and Target MTCode
		/// </summary>
		/// <param name="cultureInfo">cultureInfo</param>
		/// <returns>image</returns>
		private static Image SetLanguageFlag(CultureInfo cultureInfo)
		{
			return new Language(cultureInfo).GetFlagImage();
		}

		/// <summary>
		/// Set the flag for each Source MTCode and Target MTCode when the Language Mappings grid is showing
		/// </summary>
		/// <param name="languageMapping">languageMapping</param>
		private static void SetMTFlag(LanguageMappingModel languageMapping)
		{
			foreach (var item in languageMapping.MTCodesSource)
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