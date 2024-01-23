using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Model.Options;
using LanguageWeaverProvider.Services;
using Newtonsoft.Json;
using Sdl.Core.Globalization.LanguageRegistry;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace LanguageWeaverProvider.ViewModel
{
	public class CreateDictionaryTermViewModel : BaseViewModel
	{
		readonly EditorController _editController;
		readonly FileBasedProject _projectController;

		List<ITranslationOptions> _providers;
		ITranslationOptions _currentProvider;

		ObservableCollection<PairDictionary> _dictionaries;
		PairDictionary _selectedDictionary;

		bool _isNotificationVisible;
		string _notificationMessage;

		string _sourceLanguage;
		string _targetLanguage;
		Image _sourceImage;
		Image _targetImage;

		string _sourceTerm;
		string _targetTerm;
		string _comment;

		public CreateDictionaryTermViewModel()
		{
			_editController = SdlTradosStudio.Application.GetController<EditorController>();
			_projectController = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
			Initialize();
		}

		public ObservableCollection<PairDictionary> Dictionaries
		{
			get => _dictionaries;
			set
			{
				_dictionaries = value;
				OnPropertyChanged();
			}
		}

		public PairDictionary SelectedDictionary
		{
			get => _selectedDictionary;
			set
			{
				_selectedDictionary = value;
				OnPropertyChanged();

				SetLanguagesInfo();
				SetCurrentProvider();
			}
		}

		public bool IsNotificationVisible
		{
			get { return _isNotificationVisible; }
			set
			{
				if (_isNotificationVisible != value)
				{
					_isNotificationVisible = value;
					OnPropertyChanged();
				}
			}
		}

		public string NotificationMessage
		{
			get => _notificationMessage;
			set
			{
				_notificationMessage = value;
				OnPropertyChanged();
			}
		}

		public string SourceLanguage
		{
			get => _sourceLanguage;
			set
			{
				_sourceLanguage = value;
				OnPropertyChanged();
			}
		}

		public string TargetLanguage
		{
			get => _targetLanguage;
			set
			{
				_targetLanguage = value;
				OnPropertyChanged();
			}
		}

		public Image SourceImage
		{
			get => _sourceImage;
			set
			{
				_sourceImage = value;
				OnPropertyChanged();
			}
		}

		public Image TargetImage
		{
			get => _targetImage;
			set
			{
				_targetImage = value;
				OnPropertyChanged();
			}
		}

		public string SourceTerm
		{
			get => _sourceTerm;
			set
			{
				_sourceTerm = value;
				OnPropertyChanged();
			}
		}

		public string TargetTerm
		{
			get => _targetTerm;
			set
			{
				_targetTerm = value;
				OnPropertyChanged();
			}
		}

		public string Comment
		{
			get => _comment;
			set
			{
				_comment = value;
				OnPropertyChanged();
			}
		}

		public ICommand CreateNewTermCommand { get; set; }

		public ICommand ClearCommand { get; set; }

		public ICommand CloseDialogCommand { get; set; }

		public delegate void CloseWindowEventRaiser();

		public event CloseWindowEventRaiser CloseEventRaised;

		private void Initialize()
		{
			ClearCommand = new RelayCommand(Clear);
			CreateNewTermCommand = new RelayCommand(CreateNewTerm);
			CloseDialogCommand = new RelayCommand((action) => CloseEventRaised?.Invoke());

			_providers = _projectController
				.GetTranslationProviderConfiguration()
				.Entries
				.Where(entry => entry.MainTranslationProvider.Uri.AbsoluteUri.StartsWith(Constants.BaseTranslationScheme))
				.Select(entry => JsonConvert.DeserializeObject<TranslationOptions>(entry.MainTranslationProvider.State) as ITranslationOptions)
				.ToList();

			Dictionaries = new(_providers
				.SelectMany(provider => provider.PairMappings.SelectMany(pair => pair.Dictionaries))
				.OrderBy(x=> x.IsSelected == false));
			if (!Dictionaries.Any())
			{
				return;
			}

			SelectedDictionary = Dictionaries.FirstOrDefault();

			var currentSelection = _editController.ActiveDocument.Selection;
			SourceTerm = currentSelection.Source.ToString();
			TargetTerm = currentSelection.Target.ToString();
		}

		private void SetLanguagesInfo()
		{
			var sourceCultureInfo = new CultureInfo(SelectedDictionary.LanguagePair.SourceCultureName);
			var targetCultureInfo = new CultureInfo(SelectedDictionary.LanguagePair.TargetCultureName);

			SourceLanguage = sourceCultureInfo.DisplayName.Split('(')[0].Trim();
			TargetLanguage = targetCultureInfo.DisplayName.Split('(')[0].Trim();

			SourceImage = LanguageRegistryApi.Instance.GetLanguage(SelectedDictionary.LanguagePair.SourceCulture).GetFlagImage();
			TargetImage = LanguageRegistryApi.Instance.GetLanguage(SelectedDictionary.LanguagePair.TargetCulture).GetFlagImage();
		}

		private void SetCurrentProvider()
		{
			_currentProvider = _providers
				.SelectMany(p => p.PairMappings, (p, pair) => new { Provider = p, Pair = pair })
				.FirstOrDefault(x => x.Pair.Dictionaries.Contains(SelectedDictionary))?
				.Provider;
			CredentialManager.GetCredentials(_currentProvider, true);
		}

		private void Clear(object parameter)
		{
			switch (parameter as string)
			{
				case nameof(SourceTerm):
					SourceTerm = string.Empty;
					break;

				case nameof(TargetTerm):
					TargetTerm = string.Empty;
					break;

				case nameof(Comment):
					Comment = string.Empty;
					break;
			}
		}

		private async void ToggleSuccesfullNotification()
		{
			IsNotificationVisible = true;
			NotificationMessage = PluginResources.Dictionary_NewTerm_Succesfully;
			await Task.Delay(3000);
			IsNotificationVisible = false;
			await Task.Delay(1000);
			NotificationMessage = null;
		}

		private async void CreateNewTerm(object parameter)
		{
			var newDictionaryTerm = new DictionaryTerm()
			{
				Source = SourceTerm,
				Target = TargetTerm,
				Comment = Comment
			};

			var termWasAdded = _currentProvider.PluginVersion switch
			{
				PluginVersion.LanguageWeaverCloud => await CloudService.CreateDictionaryTerm(_currentProvider.AccessToken, SelectedDictionary, newDictionaryTerm),
				PluginVersion.LanguageWeaverEdge => await EdgeService.CreateDictionaryTerm(_currentProvider.AccessToken, SelectedDictionary, newDictionaryTerm),
				_ => false
			};

			if (termWasAdded)
			{
				ToggleSuccesfullNotification();
			}
		}
	}
}