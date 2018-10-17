using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;
using Sdl.Community.HunspellDictionaryManager.Commands;
using Sdl.Community.HunspellDictionaryManager.Helpers;
using Sdl.Community.HunspellDictionaryManager.Model;
using Sdl.Community.HunspellDictionaryManager.Ui;

namespace Sdl.Community.HunspellDictionaryManager.ViewModel
{
	public class MainWindowViewModel : ViewModelBase
	{
		#region Private Fields
		private MainWindow _mainWindow;
		private ObservableCollection<HunspellLangDictionaryModel> _dictionaryLanguages = new ObservableCollection<HunspellLangDictionaryModel>();
		private HunspellLangDictionaryModel _selectedDictionaryLanguage;
		private HunspellLangDictionaryModel _deletedDictionaryLanguage;
		private string _hunspellDictionariesFolderPath;
		private string _newDictionaryLanguage;
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
			LoadStudioLanguageDictionaries();
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

		public string NewDictionaryLanguage
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
			if (!string.IsNullOrEmpty(NewDictionaryLanguage))
			{
				CopyFiles();
				UpdateConfigFile();
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
			NewDictionaryLanguage = string.Empty;
			DeletedDictionaryLanguage = new HunspellLangDictionaryModel();
			SelectedDictionaryLanguage = new HunspellLangDictionaryModel();
			DictionaryLanguages = new ObservableCollection<HunspellLangDictionaryModel>();

			LoadStudioLanguageDictionaries();
			LabelVisibility = Constants.Hidden;
		}
		#endregion

		#region Private Methods

		/// <summary>
		/// Load .dic and .aff files from the installed Studio location -> HunspellDictionaries folder
		/// </summary>
		private void LoadStudioLanguageDictionaries()
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
						DisplayName = Path.GetFileNameWithoutExtension(hunspellDictionary)
					};

					DictionaryLanguages.Add(hunspellLangDictionaryModel);
				}

				// get .aff files from Studio HunspellDictionaries folder
				var affFiles = Directory.GetFiles(_hunspellDictionariesFolderPath, "*.aff").ToList();
				foreach (var affFile in affFiles)
				{
					var dictLang = DictionaryLanguages
						.Where(d => Path.GetFileNameWithoutExtension(d.DictionaryFile).Equals(Path.GetFileNameWithoutExtension(affFile)))
						.FirstOrDefault();

					if (dictLang != null)
					{
						dictLang.AffFile = affFile;
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
		/// </summary>
		private void CopyFiles()
		{
			var newDictionaryFilePath = Path.Combine(_hunspellDictionariesFolderPath, $"{NewDictionaryLanguage}.dic");
			var newAffFilePath = Path.Combine(_hunspellDictionariesFolderPath, $"{NewDictionaryLanguage}.aff");

			File.Copy(SelectedDictionaryLanguage.DictionaryFile, newDictionaryFilePath, true);
			File.Copy(SelectedDictionaryLanguage.DictionaryFile, newAffFilePath, true);
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
			var languageElem = (string)xmlDoc.Root.Elements("language").FirstOrDefault(x => (string)x.Element("isoCode") == NewDictionaryLanguage);

			if (string.IsNullOrEmpty(languageElem))
			{
				var node = new XElement("language",
					new XElement("isoCode", NewDictionaryLanguage), new XElement("dict", NewDictionaryLanguage));

				xmlDoc.Element("config").Add(node);
				xmlDoc.Save(configFilePath);

				LoadStudioLanguageDictionaries();
				SetGridSettings(Constants.SuccessfullCreateMessage, Constants.GreenColor);
			}
			else
			{
				SetGridSettings(Constants.LanguageAlreadyExists, Constants.RedColor);
			}
			LabelVisibility = Constants.Visible;

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

			// add new language dictionary if doesn't already exists in the config file
			var languageElem = xmlDoc.Root.Elements("language").FirstOrDefault(x => (string)x.Element("isoCode") == dictionaryLanguage);

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
		#endregion
	}
}