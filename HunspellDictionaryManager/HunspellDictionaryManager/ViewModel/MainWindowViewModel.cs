using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Sdl.Community.HunspellDictionaryManager.Commands;
using Sdl.Community.HunspellDictionaryManager.Helpers;
using Sdl.Community.HunspellDictionaryManager.Model;
using Sdl.Community.HunspellDictionaryManager.Ui;
using Sdl.Core.Globalization;

namespace Sdl.Community.HunspellDictionaryManager.ViewModel
{
	public class MainWindowViewModel : ViewModelBase
	{
		#region Private Fields
		private MainWindow _mainWindow;
		private ObservableCollection<HunspellLangDictionaryModel> _dictionaryLanguages = new ObservableCollection<HunspellLangDictionaryModel>();
		private ObservableCollection<LanguageModel> _languages = new ObservableCollection<LanguageModel>();
		private HunspellLangDictionaryModel _selectedDictionaryLanguage;
		private HunspellLangDictionaryModel _deletedDictionaryLanguage;
		private Language[] _studioLanguages = Language.GetAllLanguages();
		private string _hunspellDictionariesFolderPath;
		private LanguageModel _newDictionaryLanguage;
		private string _resultMessageColor;
		private string _labelVisibility = Constants.Hidden;
		private string _resultMessage;
		private ICommand _createHunspellDictionaryCommand;
		private ICommand _cancelCommand;
		private ICommand _deleteCommand;
		private ICommand _refreshCommand;
		#endregion

		#region Constructors
		public MainWindowViewModel(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
			LoadDictionariesLanguages();
			LoadStudioLanguages();
		}
		#endregion

		#region Public Properties
		public HunspellLangDictionaryModel SelectedDictionaryLanguage
		{
			get => _selectedDictionaryLanguage;
			set
			{
				_selectedDictionaryLanguage = value;
				OnPropertyChanged();
			}
		}

		public HunspellLangDictionaryModel DeletedDictionaryLanguage
		{
			get => _deletedDictionaryLanguage;
			set
			{
				_deletedDictionaryLanguage = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<HunspellLangDictionaryModel> DictionaryLanguages
		{
			get => _dictionaryLanguages;
			set
			{
				_dictionaryLanguages = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<LanguageModel> Languages
		{
			get => _languages;
			set
			{
				_languages = value;
				OnPropertyChanged();
			}
		}

		public LanguageModel NewDictionaryLanguage
		{
			get => _newDictionaryLanguage;
			set
			{
				_newDictionaryLanguage = value;
				OnPropertyChanged();
			}
		}

		public string LabelVisibility
		{
			get => _labelVisibility;
			set
			{
				_labelVisibility = value;
				OnPropertyChanged();
			}
		}

		public string ResultMessage
		{
			get => _resultMessage;
			set
			{
				_resultMessage = value;
				OnPropertyChanged();
			}
		}

		public string ResultMessageColor
		{
			get => _resultMessageColor;
			set
			{
				_resultMessageColor = value;
				OnPropertyChanged();
			}
		}
		#endregion

		#region Commands
		public ICommand CreateHunspellDictionaryCommand => _createHunspellDictionaryCommand ?? (_createHunspellDictionaryCommand = new CommandHandler(CreateHunspellDictionaryAction, true));
		public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new CommandHandler(CancelAction, true));
		public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new CommandHandler(DeleteAction, true));
		public ICommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new CommandHandler(RefreshAction, true));

		#endregion

		#region Actions
		private void CreateHunspellDictionaryAction()
		{
			LabelVisibility = Constants.Hidden;
			if (NewDictionaryLanguage != null)
			{
				CopyFiles();
			}
		}

		private void CancelAction()
		{
			if (_mainWindow.IsLoaded)
			{
				_mainWindow.Close();
			}
		}

		private void DeleteAction()
		{
			LabelVisibility = Constants.Hidden;
			if (DeletedDictionaryLanguage != null)
			{
				DeleteSelectedFies();
				RemoveConfigLanguageNode();
			}
		}

		private void RefreshAction()
		{
			NewDictionaryLanguage = new LanguageModel();
			DeletedDictionaryLanguage = new HunspellLangDictionaryModel();
			SelectedDictionaryLanguage = new HunspellLangDictionaryModel();
			DictionaryLanguages = new ObservableCollection<HunspellLangDictionaryModel>();
			Languages = new ObservableCollection<LanguageModel>();

			LoadDictionariesLanguages();
			LoadStudioLanguages();
			LabelVisibility = Constants.Hidden;
		}
		#endregion

		#region Private Methods

		/// <summary>
		/// Load .dic and .aff files from the installed Studio location -> HunspellDictionaries folder
		/// and display the dictionary language name which is compatible with Studio 
		/// </summary>
		private void LoadDictionariesLanguages()
		{
			var studioPath = Utils.GetInstalledStudioPath();
			if (!string.IsNullOrEmpty(studioPath))
			{
				_hunspellDictionariesFolderPath = Path.Combine(Path.GetDirectoryName(studioPath), Constants.HunspellDictionaries);

				// get .dic files from Studio HunspellDictionaries folder
				var dictionaryFiles = Directory.GetFiles(_hunspellDictionariesFolderPath, "*.dic").ToList();
				foreach (var hunspellDictionary in dictionaryFiles)
				{
					var hunspellLangDictionaryModel = new HunspellLangDictionaryModel()
					{
						DictionaryFile = hunspellDictionary,
						ShortLanguageName = Path.GetFileNameWithoutExtension(hunspellDictionary),
						DisplayLanguageName = SetDisplayLanguageName(Path.GetFileNameWithoutExtension(hunspellDictionary))
					};
					// add to dropdown list only dictionaries that has language correspondence in Studio
					if (!string.IsNullOrEmpty(hunspellLangDictionaryModel.DisplayLanguageName))
					{
						DictionaryLanguages.Add(hunspellLangDictionaryModel);
					}
				}

				// get .aff files from Studio HunspellDictionaries folder
				var affFiles = Directory.GetFiles(_hunspellDictionariesFolderPath, "*.aff").ToList();
				foreach (var dicFile in DictionaryLanguages)
				{
					var affFile = affFiles.Where(d => d.Contains($"{dicFile.ShortLanguageName}.aff")).FirstOrDefault();
					if (affFile != null)
					{
						dicFile.AffFile = affFile;
					}
				}
			}
			else
			{
				_mainWindow.Close();
			}
		}

		/// <summary>
		/// Copy selected (.dic and .aff) files and rename them using the specified dictionary language
		/// If dictionary files already exists in folder, allow user the possibility to override or not the file
		/// </summary>
		private void CopyFiles()
		{
			var newDictionaryFilePath = Path.Combine(_hunspellDictionariesFolderPath, $"{NewDictionaryLanguage.IsoCode.Replace('-', '_')}.dic");
			var newAffFilePath = Path.Combine(_hunspellDictionariesFolderPath, $"{NewDictionaryLanguage.IsoCode.Replace('-', '_')}.aff");

			if (DictionaryLanguages.Any(d => d.DictionaryFile.Equals(newDictionaryFilePath)))
			{
				var result = MessageBox.Show(Constants.DictionaryAlreadyExists, Constants.InformativeMessage, MessageBoxButton.YesNo, MessageBoxImage.Warning);
				if (result.Equals(MessageBoxResult.Yes))
				{
					CreateLanguageDictionary(newDictionaryFilePath, newAffFilePath);
				}
			}
			else
			{
				CreateLanguageDictionary(newDictionaryFilePath, newAffFilePath);
			}
		}

		/// <summary>
		/// Update spellcheckmanager_config.xml file by adding a new node which contains the new dictionary language
		/// </summary>
		private void UpdateConfigFile()
		{
			// load xml config file
			var configFilePath = Path.Combine(_hunspellDictionariesFolderPath, Constants.ConfigFileName);
			var xmlDoc = XDocument.Load(configFilePath);

			// add new language dictionary if doesn't already exists in the config file
			var languageElem = (string)xmlDoc.Root.Elements("language").FirstOrDefault(x => (string)x.Element("isoCode") == NewDictionaryLanguage.IsoCode);
			if (string.IsNullOrEmpty(languageElem))
			{
				var node = new XElement("language",
					new XElement("isoCode", NewDictionaryLanguage.IsoCode), new XElement("dict", NewDictionaryLanguage.IsoCode.Replace('-', '_')));

				xmlDoc.Element("config").Add(node);
				xmlDoc.Save(configFilePath);

				LoadDictionariesLanguages();
			}
			LabelVisibility = Constants.Visible;
			SetGridSettings(Constants.SuccessfullCreateMessage, Constants.GreenColor);
		}

		/// <summary>
		/// Delete from HunspellDictionaries folder the .aff and .dic files based on user selection
		/// </summary>
		private void DeleteSelectedFies()
		{
			if (File.Exists(DeletedDictionaryLanguage.DictionaryFile))
			{
				File.Delete(DeletedDictionaryLanguage.DictionaryFile);
			}
			if (File.Exists(DeletedDictionaryLanguage.AffFile))
			{
				File.Delete(DeletedDictionaryLanguage.AffFile);
			}
		}

		/// <summary>
		/// Remove corresponding nodes from the xml config file
		/// </summary>
		private void RemoveConfigLanguageNode()
		{
			// load xml config file
			var configFilePath = Path.Combine(_hunspellDictionariesFolderPath, Constants.ConfigFileName);
			var xmlDoc = XDocument.Load(configFilePath);
			var dictionaryLanguage = Path.GetFileNameWithoutExtension(DeletedDictionaryLanguage.DictionaryFile);

			// remove the language dictionary from the config
			var languageElem = xmlDoc.Root.Elements("language").FirstOrDefault(x => (string)x.Element("dict") == dictionaryLanguage);

			if (languageElem != null)
			{
				languageElem.Remove();
				xmlDoc.Save(configFilePath);
				SetGridSettings(Constants.SuccessfullDeleteMessage, Constants.GreenColor);
			}
			else
			{
				SetGridSettings(Constants.NoLanguageDictionaryFound, Constants.RedColor);
			}
			RefreshAction();
			LabelVisibility = Constants.Visible;
		}

		private void SetGridSettings(string resultMessage, string resultMessageColor)
		{
			ResultMessage = resultMessage;
			ResultMessageColor = resultMessageColor;
		}

		/// <summary>
		/// Set dictionary language display name based on the studio languages
		/// </summary>
		/// <param name="hunspellDictionaryName">hunspell dictionary name</param>
		/// <param name="studioLanguages">all Studio languages</param>
		/// <returns>Dictionary display language name</returns>
		private string SetDisplayLanguageName(string hunspellDictionaryName)
		{
			hunspellDictionaryName = hunspellDictionaryName.Replace('_', '-');
			//search for Language based on IsoAbbreviation
			var displayLanguageName = _studioLanguages.Where(a => a.IsoAbbreviation.Equals(hunspellDictionaryName)).FirstOrDefault();
			if (displayLanguageName != null)
			{
				return displayLanguageName.DisplayName;
			}
			else
			{
				// search for Language based on TwoLetterISOLanguageName
				displayLanguageName = _studioLanguages.Where(a => a.CultureInfo.TwoLetterISOLanguageName.Equals(hunspellDictionaryName)).FirstOrDefault();
				if (displayLanguageName != null)
				{
					return displayLanguageName.DisplayName;
				}
			}
			return string.Empty;
		}

		/// <summary>
		/// Load languages which are compatible in Studio
		/// </summary>
		private void LoadStudioLanguages()
		{
			foreach (var language in _studioLanguages)
			{
				var languageModel = new LanguageModel
				{
					LanguageName = language.DisplayName,
					IsoCode = language.IsoAbbreviation,
					TwoLetterISOLanguageName = language.CultureInfo.TwoLetterISOLanguageName
				};
				Languages.Add(languageModel);
			}
		}

		private void CreateLanguageDictionary(string newDictionaryFilePath, string newAffFilePath)
		{
			File.Copy(SelectedDictionaryLanguage.DictionaryFile, newDictionaryFilePath, true);
			File.Copy(SelectedDictionaryLanguage.AffFile, newAffFilePath, true);
			UpdateConfigFile();
		}
		#endregion
	}
}