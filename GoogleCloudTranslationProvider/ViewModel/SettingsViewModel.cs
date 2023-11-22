using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Serialization;
using GoogleCloudTranslationProvider.Commands;
using GoogleCloudTranslationProvider.Helpers;
using GoogleCloudTranslationProvider.Interfaces;
using GoogleCloudTranslationProvider.Models;
using GoogleCloudTranslationProvider.ViewModel;
using NLog;

namespace GoogleCloudTranslationProvider.ViewModels
{
	public class SettingsViewModel : BaseViewModel, ISettingsControlViewModel
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly IOpenFileDialogService _openFileDialogService;
		private readonly ITranslationOptions _options;

		private bool _isTellMeAction;
		private bool _sendPlainText;
		private bool _doPostLookup;
		private bool _doPreLookup;
		private bool _reSendDraft;
		private bool _useCustomProviderName;
		private bool _useLanguageMappingProvider;

		private bool _persistV3Project;
		private string _v3Path;
		private string _downloadPath;

		private string _preLookupFileName;
		private string _postLookupFileName;
		private string _customProviderName;

		private ICommand _clearCommand;
		private ICommand _browseFileCommand;
		private ICommand _browseFolderCommand;

		public SettingsViewModel(ITranslationOptions options)
		{
			ViewModel = this;
			_options = options;
			_openFileDialogService = new OpenFileDialogService();
			UseLanguageMappingProvider = true;
			SetSavedSettings();
		}

		public BaseViewModel ViewModel { get; set; }

		public bool ReSendDraft
		{
			get => _reSendDraft;
			set
			{
				if (_reSendDraft == value) return;
				_reSendDraft = value;
				OnPropertyChanged(nameof(ReSendDraft));
			}
		}

		public bool SendPlainText
		{
			get => _sendPlainText;
			set
			{
				if (_sendPlainText == value) return;
				_sendPlainText = value;
				OnPropertyChanged(nameof(SendPlainText));
			}
		}

		public bool DoPreLookup
		{
			get => _doPreLookup;
			set
			{
				if (_doPreLookup == value) return;
				_doPreLookup = value;
				if (!_doPreLookup)
				{
					Clear(nameof(PreLookupFileName));
				}

				OnPropertyChanged(nameof(DoPreLookup));
			}
		}

		public bool DoPostLookup
		{
			get => _doPostLookup;
			set
			{
				if (_doPostLookup == value) return;
				_doPostLookup = value;
				if (!_doPostLookup)
				{
					Clear(nameof(PostLookupFileName));
				}

				OnPropertyChanged(nameof(DoPostLookup));
			}
		}

		public bool UseCustomProviderName
		{
			get => _useCustomProviderName;
			set
			{
				if (_useCustomProviderName == value) return;
				_useCustomProviderName = value;
				OnPropertyChanged(nameof(UseCustomProviderName));
			}
		}

		public string PreLookupFileName
		{
			get => _preLookupFileName;
			set
			{
				if (_preLookupFileName == value) return;
				_preLookupFileName = value;
				OnPropertyChanged(nameof(PreLookupFileName));
			}
		}

		public string PostLookupFileName
		{
			get => _postLookupFileName;
			set
			{
				if (_postLookupFileName == value) return;
				_postLookupFileName = value;
				OnPropertyChanged(nameof(PostLookupFileName));
			}
		}

		public string CustomProviderName
		{
			get => _customProviderName;
			set
			{
				if (_customProviderName == value) return;
				_customProviderName = value;
				OnPropertyChanged(nameof(CustomProviderName));
			}
		}


		public bool IsTellMeAction
		{
			get => _isTellMeAction;
			set
			{
				if (_isTellMeAction == value) return;
				_isTellMeAction = value;
				OnPropertyChanged(nameof(IsTellMeAction));
			}
		}

		public bool PersistV3Project
		{
			get => _persistV3Project;
			set
			{
				if (_persistV3Project == value) return;
				_persistV3Project = value;
				OnPropertyChanged(nameof(PersistV3Project));
			}
		}

		public string V3Path
		{
			get => _v3Path;
			set
			{
				if (_v3Path == value) return;
				_v3Path = value;
				OnPropertyChanged(nameof(V3Path));
			}
		}

		public string DownloadPath
		{
			get => _downloadPath;
			set
			{
				if (_downloadPath == value) return;
				_downloadPath = value;
				_options.DownloadPath = value;
				OnPropertyChanged(nameof(DownloadPath));
			}
		}

		public bool UseLanguageMappingProvider
		{
			get => _useLanguageMappingProvider;
			set
			{
				if (_useLanguageMappingProvider == value) return;
				_useLanguageMappingProvider = value;
				OnPropertyChanged(nameof(UseLanguageMappingProvider));
			}
		}

		public ICommand ClearCommand => _clearCommand ??= new RelayCommand(Clear);

		public ICommand BrowseFileCommand => _browseFileCommand ??= new RelayCommand(BrowseFile);

		public ICommand BrowseFolderCommand => _browseFolderCommand ??= new RelayCommand(BrowseFolder);

		public bool SettingsAreValid()
		{
			if (DoPreLookup && string.IsNullOrEmpty(PreLookupFileName))
			{
				ErrorHandler.HandleError(PluginResources.PreLookupEmptyMessage, nameof(PreLookupFileName));
				return false;
			}

			if (DoPreLookup && !File.Exists(PreLookupFileName))
			{
				ErrorHandler.HandleError(PluginResources.PreLookupWrongPathMessage, nameof(PreLookupFileName));
				return false;
			}

			if (DoPostLookup && string.IsNullOrEmpty(PostLookupFileName))
			{
				ErrorHandler.HandleError(PluginResources.PostLookupEmptyMessage, nameof(PostLookupFileName));
				return false;
			}

			if (DoPostLookup && !File.Exists(PostLookupFileName))
			{
				ErrorHandler.HandleError(PluginResources.PostLookupWrongPathMessage, nameof(PostLookupFileName));
				return false;
			}

			return true;
		}

		private void SetSavedSettings()
		{
			ReSendDraft = _options.ResendDrafts;
			SendPlainText = _options.SendPlainTextOnly;
			DoPreLookup = _options.UsePreEdit;
			PreLookupFileName = _options.PreLookupFilename;
			DoPostLookup = _options.UsePostEdit;
			PostLookupFileName = _options.PostLookupFilename;
			CustomProviderName = _options.CustomProviderName;
			UseCustomProviderName = _options.UseCustomProviderName;
			DownloadPath ??= _options.DownloadPath ?? Constants.AppDataFolder;
		}

		private void Clear(object parameter)
		{
			switch (parameter as string)
			{
				case nameof(PreLookupFileName):
					PreLookupFileName = string.Empty;
					break;
				case nameof(PostLookupFileName):
					PostLookupFileName = string.Empty;
					break;
				case nameof(CustomProviderName):
					CustomProviderName = string.Empty;
					break;
				case nameof(DownloadPath):
					DownloadPath = Constants.AppDataFolder;
					break;
			}
		}

		private void BrowseFolder(object parameter)
		{
			using var folderBrowserDialog = new FolderBrowserDialog();
			if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}

			DownloadPath = folderBrowserDialog.SelectedPath;
		}

		private void BrowseFile(object parameter)
		{
			const string Browse_XmlFiles = "XML Files|*.xml";

			var selectedFile = _openFileDialogService.ShowDialog(Browse_XmlFiles);
			if (string.IsNullOrEmpty(selectedFile))
			{
				return;
			}

			if (parameter.Equals(nameof(PreLookupFileName)))
			{
				PreLookupFileName = selectedFile;
				CheckIfIsValidLookupFile(PreLookupFileName);
			}
			else if (parameter.Equals(nameof(PostLookupFileName)))
			{
				PostLookupFileName = selectedFile;
				CheckIfIsValidLookupFile(PostLookupFileName);
			}
		}

		private void CheckIfIsValidLookupFile(string filePath)
		{
			try
			{
				using var reader = new StreamReader(filePath);
				var serializer = new XmlSerializer(typeof(EditCollection));
				var edcoll = (EditCollection)serializer.Deserialize(reader);
			}
			catch (InvalidOperationException)
			{ //invalid operation is what happens when the xml can't be parsed into the objects correctly
				var fileName = Path.GetFileName(filePath);
				//ErrorMessage = $"{MtProviderConfDialogResources.lookupFileStructureCheckErrorCaption} {fileName}";
			}
			catch (Exception e)
			{ //catch-all for any other kind of error...passes up a general message with the error description
				_logger.Error($"{MethodBase.GetCurrentMethod().Name}: {e}");
				//ErrorMessage = $"{MtProviderConfDialogResources.lookupFileStructureCheckGenericErrorMessage} {exp.Message}";
			}
		}
	}
}