using System;
using System.Reflection;
using System.Windows.Input;
using System.Xml.Serialization;
using NLog;
using MicrosoftTranslatorProvider.Commands;
using MicrosoftTranslatorProvider.Model;
using MicrosoftTranslatorProvider.Interfaces;
using System.Windows;
using System.Windows.Forms;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace MicrosoftTranslatorProvider.ViewModel
{
	public class SettingsControlViewModel: BaseModel, ISettingsControlViewModel
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly ITranslationOptions _options;
		private readonly IOpenFileDialogService _openFileDialogService;
		private readonly ITranslationProviderCredentialStore _credentialStore;

		private bool _reSendDraft;
		private bool _sendPlainText;
		private bool _doPreLookup;
		private bool _doPostLookup;
		private bool _isTellMeAction;
		private string _preLookupFileName;
		private string _postLookupFileName;
		private string _errorMessage;

		private bool _useCustomProviderName;
		private string _customProviderName;

		private ICommand _clearCommand;

		public SettingsControlViewModel(ITranslationOptions options, ITranslationProviderCredentialStore credentialStore, IOpenFileDialogService openFileDialogService, bool isTellMeAction)
		{
			ViewModel = this;
			_options = options;
			IsTellMeAction = isTellMeAction;
			BrowseCommand = new RelayCommand(Browse);
			_openFileDialogService = openFileDialogService;
			_credentialStore = credentialStore;

			SetSavedSettings();
		}
		
		public BaseModel ViewModel { get; set; }
		public ICommand ShowMainWindowCommand { get; set; }
		public ICommand BrowseCommand { get; set; }
		public ICommand ShowSettingsCommand { get; set; }

		public bool ReSendDraft
		{
			get => _reSendDraft;
			set
			{
				if (_reSendDraft == value) return;
				_reSendDraft = value;
				ErrorMessage = string.Empty;
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
				ErrorMessage = string.Empty;
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
					PreLookupFileName = string.Empty;
				}
				ErrorMessage = string.Empty;
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
					PostLookupFileName = string.Empty;
				}
				OnPropertyChanged(nameof(DoPostLookup));
			}
		}

		public string PreLookupFileName
		{
			get => _preLookupFileName;
			set
			{
				if (_preLookupFileName == value) return;
				_preLookupFileName = value;
				ErrorMessage = string.Empty;

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
				ErrorMessage = string.Empty;

				OnPropertyChanged(nameof(PostLookupFileName));
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

		public ICommand ClearCommand => _clearCommand ??= new RelayCommand(Clear);
		
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
		}

		private void Clear(object parameter)
		{
			if (parameter is not string fieldName)
			{
				return;
			}

			switch (fieldName)
			{
				case "PreLookupFileName":
					PreLookupFileName = string.Empty;
					break;
				case "PostLookupFileName":
					PostLookupFileName = string.Empty;
					break;
				case "CustomProviderName":
					CustomProviderName = string.Empty;
					break;
			}
		}

		private void Browse(object commandParameter)
		{
			ErrorMessage = string.Empty;
			if (string.IsNullOrEmpty(commandParameter.ToString()))
			{
				return;
			}

			var selectedFile = _openFileDialogService.ShowDialog("XML Files(*.xml) | *.xml");
			if (string.IsNullOrEmpty(selectedFile))
			{
				return;
			}

			if (commandParameter.Equals(PluginResources.PreLookBrowse))
			{
				PreLookupFileName = selectedFile;
				CheckIfIsValidLookupFile(PreLookupFileName);
			}
			else if (commandParameter.Equals(PluginResources.PostLookupBrowse))
			{
				PostLookupFileName = selectedFile;
				CheckIfIsValidLookupFile(PostLookupFileName);
			}
		}

		private void CheckIfIsValidLookupFile(string filePath)
		{
			try
			{
				using var reader = new System.IO.StreamReader(filePath);
				var serializer = new XmlSerializer(typeof(EditCollection));
				var edcoll = (EditCollection)serializer.Deserialize(reader);
			}
			catch (InvalidOperationException) //invalid operation is what happens when the xml can't be parsed into the objects correctly
			{
				var fileName = System.IO.Path.GetFileName(filePath);
				ErrorMessage = $"{PluginResources.lookupFileStructureCheckErrorCaption} {fileName}";
			}
			catch (Exception exp) //catch-all for any other kind of error...passes up a general message with the error description
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name}: {exp}");

				ErrorMessage = $"{PluginResources.lookupFileStructureCheckGenericErrorMessage} {exp.Message}";
			}
		}
	}
}