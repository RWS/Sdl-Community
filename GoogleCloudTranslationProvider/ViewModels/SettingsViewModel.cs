using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Serialization;
using GoogleCloudTranslationProvider.Commands;
using GoogleCloudTranslationProvider.Interfaces;
using GoogleCloudTranslationProvider.Models;
using GoogleCloudTranslationProvider.Service;
using NLog;

namespace GoogleCloudTranslationProvider.ViewModels
{
	public class SettingsViewModel : BaseModel, ISettingsControlViewModel
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

		private bool _showAdvancedSettingsButton;
		public bool ShowAdvancedSettingsButton
		{
			get => _showAdvancedSettingsButton;
			set
			{
				if (_showAdvancedSettingsButton == value) return;
				_showAdvancedSettingsButton = value;
				OnPropertyChanged(nameof(ShowAdvancedSettingsButton));
			}
		}
		private bool _showAdvancedSettings;
		private bool _persistV3Project;
		private string _v3Path;
		private string _downloadPath;
		private string _downloadFileName;
		private bool _alwaysResendDrafts;
		private bool _alwaysSendPlainText;


		private string _preLookupFileName;
		private string _postLookupFileName;
		private string _customProviderName;
		private string _errorMessage;

		private ICommand _clearCommand;
		private ICommand _browseFileCommand;
		private ICommand _browseFolderCommand;
		private ICommand _showAdvancedSettingsCommand;
		private ICommand _saveAdvancedSettingsCommand;
		private ICommand _resetAdvancedSettingsCommand;

		public SettingsViewModel(ITranslationOptions options, bool hideAdvancedSettings = false)
		{
			ViewModel = this;
			ShowAdvancedSettingsButton = hideAdvancedSettings;
			_options = options;
			_openFileDialogService = new OpenFileDialogService();
			SetSavedSettings();
		}

		public BaseModel ViewModel { get; set; }

		public bool ReSendDraft
		{
			get => _reSendDraft;
			set
			{
				if (_reSendDraft == value) return;
				_reSendDraft = value;
				OnPropertyChanged(nameof(ReSendDraft));
				ErrorMessage = string.Empty;
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
				ErrorMessage = string.Empty;
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
				ErrorMessage = string.Empty;
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
				ErrorMessage = string.Empty;
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

		public bool ShowAdvancedSettings
		{
			get => _showAdvancedSettings;
			set
			{
				if (_showAdvancedSettings == value) return;
				_showAdvancedSettings = value;
				OnPropertyChanged(nameof(ShowAdvancedSettings));
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
				ErrorMessage = string.Empty;
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
				ErrorMessage = string.Empty;
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

		public string ErrorMessage
		{
			get => _errorMessage;
			set
			{
				if (_errorMessage == value) return;
				_errorMessage = value;
				OnPropertyChanged(nameof(ErrorMessage));
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
				_options.AdvancedSettings.PersistV3Project = value;
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
				_options.AdvancedSettings.V3Path = value;
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
				_options.AdvancedSettings.DownloadPath = value;
				OnPropertyChanged(nameof(DownloadPath));
			}
		}

		public string DownloadFileName
		{
			get => _downloadFileName;
			set
			{
				if (_downloadFileName == value) return;
				_downloadFileName = value;
				_options.AdvancedSettings.DownloadFileName = value;
				OnPropertyChanged(nameof(DownloadFileName));
			}
		}

		public bool AlwaysResendDrafts
		{
			get => _alwaysResendDrafts;
			set
            {
                if (_alwaysResendDrafts == value) return;
                _alwaysResendDrafts = value;
				_options.AdvancedSettings.AlwaysResendDrafts = value;
				OnPropertyChanged(nameof(AlwaysResendDrafts));
            }
		}

		public bool AlwaysSendPlainText
		{
			get => _alwaysSendPlainText;
			set
            {
                if (_alwaysSendPlainText == value) return;
                _alwaysSendPlainText = value;
				_options.AdvancedSettings.AlwaysSendPlainText = value;
				OnPropertyChanged(nameof(AlwaysSendPlainText));
            }
		}

		public ICommand ClearCommand => _clearCommand ??= new RelayCommand(Clear);
		public ICommand BrowseFileCommand => _browseFileCommand ??= new RelayCommand(BrowseFile);
		public ICommand BrowseFolderCommand => _browseFolderCommand ??= new RelayCommand(BrowseFolder);
		public ICommand ShowAdvancedSettingsCommand => _showAdvancedSettingsCommand ??= new RelayCommand(ChangeAdvancedSettingsVisibility);
		public ICommand SaveAdvancedSettingsCommand => _saveAdvancedSettingsCommand ??= new RelayCommand(SaveAdvancedSettings);
		public ICommand ResetAdvancedSettingsCommand => _resetAdvancedSettingsCommand ??= new RelayCommand(ResetAdvancedSettings);

		public bool SettingsAreValid()
		{
			if (DoPreLookup && string.IsNullOrEmpty(PreLookupFileName))
			{
				ErrorMessage = PluginResources.PreLookupEmptyMessage;
				return false;
			}

			if (DoPreLookup && !File.Exists(PreLookupFileName))
			{
				ErrorMessage = PluginResources.PreLookupWrongPathMessage;
				return false;
			}

			if (DoPostLookup && string.IsNullOrEmpty(PostLookupFileName))
			{
				ErrorMessage = PluginResources.PostLookupEmptyMessage;
				return false;
			}

			if (DoPostLookup && !File.Exists(PostLookupFileName))
			{
				ErrorMessage = PluginResources.PostLookupWrongPathMessage;
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

			_options.AdvancedSettings ??= new();
			PersistV3Project = _options.AdvancedSettings.PersistV3Project;
			V3Path = _options.AdvancedSettings.V3Path;
			DownloadPath = _options.AdvancedSettings.DownloadPath;
			DownloadFileName = _options.AdvancedSettings.DownloadFileName;
			AlwaysResendDrafts = _options.AdvancedSettings.AlwaysResendDrafts;
			AlwaysSendPlainText = _options.AdvancedSettings.AlwaysSendPlainText;
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
				case nameof(DownloadFileName):
					DownloadFileName = "downloadedProject";
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

			ErrorMessage = string.Empty;
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

		private void ChangeAdvancedSettingsVisibility(object parameter)
		{
			ShowAdvancedSettings = !ShowAdvancedSettings;
		}

		private void SaveAdvancedSettings(object parameter)
		{
			_options.AdvancedSettings.SaveState();
		}

		private void ResetAdvancedSettings(object parameter)
		{
			_options.AdvancedSettings.Clear();
			DownloadPath = _options.AdvancedSettings.DownloadPath;
			DownloadFileName = _options.AdvancedSettings.DownloadFileName;
			AlwaysResendDrafts = _options.AdvancedSettings.AlwaysResendDrafts;
			AlwaysSendPlainText = _options.AdvancedSettings.AlwaysSendPlainText;
		}
	}
}