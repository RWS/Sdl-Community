using System;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using System.Xml.Serialization;
using GoogleTranslatorProvider.Commands;
using GoogleTranslatorProvider.Interfaces;
using GoogleTranslatorProvider.Models;
using GoogleTranslatorProvider.Service;
using NLog;

namespace GoogleTranslatorProvider.ViewModels
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

		private string _preLookupFileName;
		private string _postLookupFileName;
		private string _errorMessage;

		private ICommand _clearCommand;
		private ICommand _browseCommand;

		public SettingsViewModel(ITranslationOptions options, bool isTellMeAction = false)
		{
			ViewModel = this;
			IsTellMeAction = isTellMeAction;
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

		public ICommand ClearCommand => _clearCommand ??= new RelayCommand(Clear);
		public ICommand BrowseCommand => _browseCommand ??= new RelayCommand(Browse);

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
			}
		}

		private void Browse(object parameter)
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
	}
}