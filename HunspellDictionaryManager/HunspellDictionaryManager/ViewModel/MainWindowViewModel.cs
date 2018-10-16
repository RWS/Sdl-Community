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

namespace Sdl.Community.HunspellDictionaryManager.ViewModel
{
	public class MainWindowViewModel : ViewModelBase
	{
		#region Private Fields
		private ObservableCollection<HunspellLangDictionaryModel> _dictionaryLanguages = new ObservableCollection<HunspellLangDictionaryModel>();
		private HunspellLangDictionaryModel _selectedDictionaryLanguage;
		private string _newDictionaryLanguage;
		private string _labelVisibility = Constants.Hidden;
		private ICommand _createHunspellDictionaryCommand;
		private ICommand _cancelCommand;
		private MainWindow _mainWindow;
		private string _hunspellDictionariesFolderPath;
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
		#endregion

		#region Commands
		public ICommand CreateHunspellDictionaryCommand => _createHunspellDictionaryCommand ?? (_createHunspellDictionaryCommand = new CommandHandler(CreateHunspellDictionaryAction, true));
		public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new CommandHandler(CancelAction, true));
		#endregion

		#region Actions
		private void CreateHunspellDictionaryAction()
		{
			CopyFiles();
			UpdateConfigFile();

			LabelVisibility = Constants.Visible;
		}

		private void CancelAction()
		{
			if (_mainWindow.IsLoaded)
			{
				_mainWindow.Close();
			}
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

					_dictionaryLanguages.Add(hunspellLangDictionaryModel);
				}

				// get .aff files from Studio HunspellDictionaries folder
				var affFiles = Directory.GetFiles(_hunspellDictionariesFolderPath, "*.aff").ToList();
				foreach (var affFile in affFiles)
				{
					var dictLang = _dictionaryLanguages
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
			var languageElem = (string)xmlDoc.Root
									  .Elements("language")
									  .FirstOrDefault(x => (string)x.Element("isoCode") == NewDictionaryLanguage);

			if(string.IsNullOrEmpty(languageElem))
			{
				var node = new XElement("language",
					new XElement("isoCode", NewDictionaryLanguage), new XElement("dict", NewDictionaryLanguage));

				xmlDoc.Element("config").Add(node);
				xmlDoc.Save(configFilePath);
			}
			else
			{
				MessageBox.Show(Constants.LanguageAlreadyExists, Constants.InformativeMessage, MessageBoxButton.OK, MessageBoxImage.Information);
			}									  
		}
		#endregion
	}
}