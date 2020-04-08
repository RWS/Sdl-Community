using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.MTCloud.Languages.Provider.Model;
using Sdl.Community.MTCloud.Provider.Commands;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Studio;
using Sdl.Community.MTCloud.Provider.View;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Application = System.Windows.Forms.Application;
using Cursors = System.Windows.Input.Cursors;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Sdl.Community.MTCloud.Provider.ViewModel
{
	public class OptionsViewModel : BaseViewModel, IDisposable
	{		
		private readonly SdlMTCloudTranslationProvider _provider;
		private readonly LanguagePair[] _projectLanguagePairs;

		private ICommand _saveCommand;
		private ICommand _resetToDefaultsCommand;
		private ICommand _viewLanguageMappingsCommand;

		private bool _reSendChecked;
		private List<MTCloudDictionary> _mtCloudDictionaries;
		private LanguageMappingModel _selectedLanguageMapping;
		private ObservableCollection<LanguageMappingModel> _languageMappings;
		private bool _isWaiting;

		public OptionsViewModel(Window owner, SdlMTCloudTranslationProvider provider, LanguagePair[] projectLanguagePairs)
		{
			Owner = owner;

			_provider = provider;
			_projectLanguagePairs = projectLanguagePairs;

			ReSendChecked = provider.Options?.ResendDraft ?? true;
			LoadLanguageMappings();
		}

		public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(Save));

		public ICommand ResetToDefaultsCommand => _resetToDefaultsCommand
														?? (_resetToDefaultsCommand = new RelayCommand(ResetToDefaults));

		public ICommand ViewLanguageMappingsCommand => _viewLanguageMappingsCommand
														?? (_viewLanguageMappingsCommand = new RelayCommand(ViewLanguageMappings));

		public Window Owner { get; }	

		public List<MTCloudDictionary> MTCloudDictionaries
		{
			get
			{
				if (_mtCloudDictionaries == null)
				{
					var accountId = _provider.TranslationService.ConnectionService.Credential.AccountId;
					var result = Task.Run(async () => await _provider.TranslationService.GetDictionaries(accountId)).Result;
					_mtCloudDictionaries = result?.Dictionaries;
				}

				return _mtCloudDictionaries;
			}
		}

		public ObservableCollection<LanguageMappingModel> LanguageMappings
		{
			get => _languageMappings;
			set
			{
				if (_languageMappings != null)
				{
					foreach (var languageMappingModel in _languageMappings)
					{
						languageMappingModel.PropertyChanged -= LanguageMappingModel_PropertyChanged;
					}
				}

				_languageMappings = value;

				if (_languageMappings != null)
				{
					foreach (var languageMappingModel in _languageMappings)
					{
						languageMappingModel.PropertyChanged += LanguageMappingModel_PropertyChanged;
					}
				}

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

		public void SaveLanguageMappings()
		{
			var savedSettings = _provider.TranslationService.LanguageMappingsService.GetLanguageMappingSettings();

			if (savedSettings != null)
			{
				_provider.TranslationService.LanguageMappingsService.RemoveLanguageMappingSettings();
				savedSettings.LanguageMappings = LanguageMappings.ToList();
				_provider.TranslationService.LanguageMappingsService.SaveLanguageMappingSettings(savedSettings);
			}
		}	

		private List<LanguageMappingModel> GetSavedLanguageMappings()
		{
			try
			{
				return _provider.TranslationService.LanguageMappingsService.GetLanguageMappingSettings()?.LanguageMappings;
			}
			catch
			{
				return new List<LanguageMappingModel>();
			}
		}

		private void LoadLanguageMappings()
		{
			if (LanguageMappings != null && LanguageMappings.Any())
			{
				return;
			}

			var savedSettings = GetSavedLanguageMappings();

			var mtCodes = _provider.LanguagesProvider.GetLanguages();

			var sourceLanguage = _projectLanguagePairs?[0].SourceCulture;
			var sourceLanguageCode = mtCodes?.FirstOrDefault(s => s.TradosCode.Equals(sourceLanguage?.Name));

			var sourceLanguageMappings = GetLanguageMapping(sourceLanguageCode, sourceLanguage);
			var sourceLanguageMappingSelected = sourceLanguageMappings.FirstOrDefault(a => a.IsLocale) ?? sourceLanguageMappings[0];

			if (_projectLanguagePairs == null || sourceLanguageCode == null)
			{
				return;
			}

			var languageMappingModels = new List<LanguageMappingModel>();

			foreach (var languagePair in _projectLanguagePairs)
			{
				var targetLanguage = languagePair.TargetCulture;
				var targetLanguageCode = mtCodes.FirstOrDefault(s => s.TradosCode.Equals(languagePair.TargetCulture?.Name));

				if (targetLanguageCode != null)
				{
					var languagePairName = $"{sourceLanguage?.DisplayName} - {languagePair.TargetCulture?.DisplayName}";
					var savedLanguageMappingModel = savedSettings.FirstOrDefault(a => a.ProjectLanguagePair.Equals(languagePairName));

					var targetLanguageMappings = GetLanguageMapping(targetLanguageCode, targetLanguage);
					var targetLanguageMappingSelected = targetLanguageMappings.FirstOrDefault(a => a.IsLocale) ?? targetLanguageMappings[0];

					// assign the selected target langauge
					targetLanguageMappingSelected = targetLanguageMappings.FirstOrDefault(a =>
														a.CodeName.Equals(savedLanguageMappingModel?.SelectedMTCodeTarget?.CodeName))
													?? targetLanguageMappingSelected;

					var engineModels = GetEngineModels(sourceLanguageCode.TradosCode, targetLanguageCode.TradosCode, sourceLanguageMappingSelected, targetLanguageMappingSelected);

					// attempt to recover the language model from the secondary language code if it exists!
					if (engineModels.Count == 1 && engineModels[0].DisplayName == Constants.NoAvailableModel && targetLanguageMappings.Count > 1
							&& savedLanguageMappingModel?.SelectedModelOption.DisplayName != Constants.NoAvailableModel)
					{						
						var secondaryLanguageCode = targetLanguageMappings.FirstOrDefault(a => a.CodeName != targetLanguageMappingSelected.CodeName);

						var secondaryEngineModels = GetEngineModels(sourceLanguageCode.TradosCode, targetLanguageCode.TradosCode,
							sourceLanguageMappingSelected, secondaryLanguageCode);

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
							engineModels.FirstOrDefault(a => string.Compare(a.DisplayName,
									  savedLanguageMappingModel?.SelectedModelOption?.DisplayName, StringComparison.InvariantCultureIgnoreCase) == 0)
							?? engineModels.FirstOrDefault(a => string.Compare(a.Model, "generic", StringComparison.InvariantCultureIgnoreCase) == 0)
							?? engineModels[0];

						var dictionaries = GetDictionaries(sourceLanguageMappingSelected, targetLanguageMappingSelected);

						// assign the selected dictionary
						var selectedDictionary =
							dictionaries.FirstOrDefault(a => a.Name.Equals(savedLanguageMappingModel?.SelectedMTCloudDictionary?.Name))
							?? dictionaries[0];

						var languageMappingModel = new LanguageMappingModel
						{
							ProjectLanguagePair = languagePairName,
							MTCodesSource = sourceLanguageMappings,
							MTCodesTarget = targetLanguageMappings,
							SelectedMTCodeSource = sourceLanguageMappingSelected,
							SelectedMTCodeTarget = targetLanguageMappingSelected,
							SourceTradosCode = sourceLanguageCode.TradosCode,
							TargetTradosCode = targetLanguageCode.TradosCode,
							Engines = engineModels,
							SelectedModelOption = selectedModel,
							MTCloudDictionaries = dictionaries,
							SelectedMTCloudDictionary = selectedDictionary
						};

						languageMappingModels.Add(languageMappingModel);
					}
				}
			}

			LanguageMappings = new ObservableCollection<LanguageMappingModel>(languageMappingModels);
		}
		
		private List<TranslationModel> GetEngineModels(string source, string target, LangMappingMTCode sourceCode, LangMappingMTCode targetCode)
		{
			var translationModels = new List<TranslationModel>();

			var mtCloudlanguagePairs = _provider.SubscriptionInfo?.LanguagePairs;

			if (mtCloudlanguagePairs == null)
			{
				return null;
			}

			var models = mtCloudlanguagePairs.Where(a => string.Compare(sourceCode.CodeName, a.SourceLanguageId, StringComparison.InvariantCultureIgnoreCase) == 0
													 && string.Compare(targetCode.CodeName, a.TargetLanguageId, StringComparison.InvariantCultureIgnoreCase) == 0);

			foreach (var model in models)
			{
				translationModels.Add(new TranslationModel
				{
					Model = model.Model,
					MTCloudLanguagePair = model,
					DisplayName =
						$"{model.SourceLanguageId}-{model.TargetLanguageId} {model.DisplayName}",
					Source = source,
					Target = target
				});
			}

			if (translationModels.Count == 0)
			{
				translationModels.Add(new TranslationModel
				{
					Model = null,
					MTCloudLanguagePair = null,
					DisplayName = Constants.NoAvailableModel,
					Source = source,
					Target = target
				});
			}

			return translationModels;
		}

		private static List<LangMappingMTCode> GetLanguageMapping(MTCloudLanguage mtCloudLanguage, CultureInfo language)
		{
			var languageMappings = new List<LangMappingMTCode>();

			if (mtCloudLanguage != null)
			{
				languageMappings.Add(new LangMappingMTCode
				{
					CodeName = mtCloudLanguage.MTCode,
					IsLocale = false,
					Flag = SetLanguageFlag(language)
				});

				if (!string.IsNullOrEmpty(mtCloudLanguage.MTCodeLocale))
				{
					languageMappings.Add(new LangMappingMTCode
					{
						CodeName = mtCloudLanguage.MTCodeLocale,
						IsLocale = true,
						Flag = SetLanguageFlag(language)
					});
				}
			}

			return languageMappings;
		}

		private List<MTCloudDictionary> GetDictionaries(LangMappingMTCode sourceCode, LangMappingMTCode targetCode)
		{
			var cloudDictionaries = new List<MTCloudDictionary>();

			if (MTCloudDictionaries != null)
			{
				var dictionaries = MTCloudDictionaries.Where(a =>
					string.Compare(a.Source, sourceCode.CodeName, StringComparison.InvariantCultureIgnoreCase) == 0
					&& string.Compare(a.Target, targetCode.CodeName, StringComparison.InvariantCultureIgnoreCase) == 0).ToList();

				if (dictionaries.Any())
				{
					cloudDictionaries.AddRange(dictionaries);
				}

				if (cloudDictionaries.Count == 0)
				{
					cloudDictionaries.Add(new MTCloudDictionary { Name = Constants.NoAvailableDictionary, DictionaryId = string.Empty });
				}
				else if (!cloudDictionaries.Exists(a => a.Name == Constants.NoDictionary))
				{
					cloudDictionaries.Insert(0, new MTCloudDictionary { Name = Constants.NoDictionary, DictionaryId = string.Empty });
				}
			}

			return cloudDictionaries;
		}

		private void ResetToDefaults(object parameter)
		{
			try
			{
				ReSendChecked = true;

				var savedSettings = _provider.TranslationService.LanguageMappingsService.GetLanguageMappingSettings();
				if (savedSettings != null)
				{
					_provider.TranslationService.LanguageMappingsService.RemoveLanguageMappingSettings();
					savedSettings.LanguageMappings = new List<LanguageMappingModel>();
					_provider.TranslationService.LanguageMappingsService.SaveLanguageMappingSettings(savedSettings);
				}

				IsWaiting = true;
				if (Owner != null)
				{
					Mouse.OverrideCursor = Cursors.Wait;
				}

				if (LanguageMappings != null)
				{
					LanguageMappings.Clear();
					LoadLanguageMappings();

					if (Owner != null)
					{
						System.Windows.MessageBox.Show(PluginResources.Message_Successfully_reset_to_defaults,
							Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
					}
				}
			}
			catch (Exception ex)
			{
				IsWaiting = false;
				Log.Logger.Error($"{Constants.IsWindowValid} {ex.Message}\n {ex.StackTrace}");

				if (Owner != null)
				{
					Mouse.OverrideCursor = Cursors.Arrow;
					MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}				
			}
			finally
			{
				IsWaiting = false;
				if (Owner != null)
				{
					Mouse.OverrideCursor = Cursors.Arrow;
				}
			}
		}

		private void Reload()
		{
			try
			{
				IsWaiting = true;
				if (Owner != null)
				{
					Mouse.OverrideCursor = Cursors.Wait;
				}

				if (LanguageMappings != null)
				{
					LanguageMappings.Clear();
					LoadLanguageMappings();					
				}
			}
			catch (Exception ex)
			{
				IsWaiting = false;
				Log.Logger.Error($"{Constants.IsWindowValid} {ex.Message}\n {ex.StackTrace}");

				if (Owner != null)
				{
					Mouse.OverrideCursor = Cursors.Arrow;
					MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			finally
			{
				IsWaiting = false;
				if (Owner != null)
				{
					Mouse.OverrideCursor = Cursors.Arrow;
				}
			}
		}
		
		private void Save(object parameter)
		{
			try
			{
				if (Owner != null)
				{
					Mouse.OverrideCursor = Cursors.Wait;
				}

				var canSave = true;
				var invalidModel = LanguageMappings.FirstOrDefault(a => a.SelectedModelOption.DisplayName == Constants.NoAvailableModel);
				if (invalidModel != null)
				{
					canSave = false;

					if (Owner != null)
					{
						var message = string.Format(PluginResources.Message_SelectLanguageDirectionForMTModel,
							invalidModel.SelectedMTCodeSource.CodeName, invalidModel.SelectedMTCodeTarget.CodeName);
						var question = PluginResources.Message_DoYouWantToProceed;

						var response = MessageBox.Show(message + Environment.NewLine + Environment.NewLine + question,
							Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
						if (response == DialogResult.Yes)
						{
							canSave = true;
						}
					}
				}

				if (canSave)
				{
					_provider.Options.ResendDraft = ReSendChecked;

					SaveLanguageMappings();
					Dispose();

					if (Owner != null)
					{
						WindowCloser.SetDialogResult(Owner, true);
						Owner.Close();
					}
				}
			}
			finally
			{
				if (Owner != null)
				{
					Mouse.OverrideCursor = Cursors.Arrow;
				}
			}
		}
	
		private void ViewLanguageMappings(object obj)
		{
			if (Owner == null)
			{
				return;
			}

			var window = new MTCodesWindow
			{
				Owner = Owner.Owner
			};

			var languages = new Languages.Provider.Languages();
			var viewModel = new MTCodesViewModel(window, languages);
			window.DataContext = viewModel;

			var result = window.ShowDialog();
			if (result.HasValue && result.Value)
			{
				Reload();
			}
		}

		private static Image SetLanguageFlag(CultureInfo cultureInfo)
		{
			return new Language(cultureInfo).GetFlagImage();
		}

		private void LanguageMappingModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (sender is LanguageMappingModel languageModel)
			{
				if (e.PropertyName == nameof(LanguageMappingModel.SelectedMTCodeSource) ||
					e.PropertyName == nameof(LanguageMappingModel.SelectedMTCodeTarget))
				{
					var engineModels = GetEngineModels(languageModel.SourceTradosCode, languageModel.TargetTradosCode,
						languageModel.SelectedMTCodeSource, languageModel.SelectedMTCodeTarget);

					if (engineModels.Any())
					{
						// assign the selected model
						var selectedModel =
							engineModels.FirstOrDefault(a => string.Compare(a.DisplayName,
										 languageModel.SelectedModelOption?.DisplayName, StringComparison.InvariantCultureIgnoreCase) == 0)
							?? engineModels.FirstOrDefault(a => string.Compare(a.Model, "generic", StringComparison.InvariantCultureIgnoreCase) == 0)
							?? engineModels[0];

						languageModel.Engines = engineModels;
						languageModel.SelectedModelOption = selectedModel;
					}

					var dictionaries = GetDictionaries(languageModel.SelectedMTCodeSource, languageModel.SelectedMTCodeTarget);

					// assign the selected dictionary
					var selectedDictionary =
						dictionaries.FirstOrDefault(a => a.Name.Equals(languageModel.SelectedMTCloudDictionary?.Name))
						?? dictionaries[0];

					languageModel.MTCloudDictionaries = dictionaries;
					languageModel.SelectedMTCloudDictionary = selectedDictionary;
				}
			}
		}

		public void Dispose()
		{
			if (_languageMappings != null)
			{
				foreach (var languageMappingModel in _languageMappings)
				{
					languageMappingModel.PropertyChanged -= LanguageMappingModel_PropertyChanged;
				}
			}
		}
	}
}