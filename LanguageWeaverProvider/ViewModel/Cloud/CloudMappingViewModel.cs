using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using LanguageMappingProvider.Database;
using LanguageMappingProvider.Database.Interface;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.LanguageMappingProvider;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.ViewModel.Interface;
using Sdl.LanguagePlatform.Core;

namespace LanguageWeaverProvider.ViewModel.Cloud
{
	public class CloudMappingViewModel : BaseViewModel, IPairMappingViewModel
	{
		private readonly ILanguageMappingDatabase _languageMappingDatabase;
		private readonly LanguagePair[] _langaugePairs;

		private ITranslationOptions _translationOptions;
		private ObservableCollection<PairMapping> _pairMappings;

		public CloudMappingViewModel(ITranslationOptions translationOptions, LanguagePair[] languagePairs)
		{
			_langaugePairs = languagePairs;
			_translationOptions = translationOptions;
			_languageMappingDatabase = new LanguageMappingDatabase("cloud", DatabaseControl.GetCloudLanguageCodes());
			InitializeCommands();
			LoadPairMapping();
		}

		public ObservableCollection<PairMapping> PairMappings
		{
			get => _pairMappings;
			set
			{
				if (_pairMappings == value) return;
				_pairMappings = value;
				OnPropertyChanged();
			}
		}

		public ICommand SaveCommand { get; set; }

		public ICommand CancelCommand { get; set; }

		private void InitializeCommands()
		{
			SaveCommand = new RelayCommand(SaveChanges);
			CancelCommand = new RelayCommand(CancelChanges);
		}

		private void LoadPairMapping()
		{
			if (_translationOptions.PairMappings is null
			 || !_translationOptions.PairMappings.Any())
			{
				CreatePairMapping();
				return;
			}
		}

		private void CreatePairMapping()
		{
			PairMappings = new();
			var mappedLanguages = _languageMappingDatabase.GetMappedLanguages();
			foreach (var languagePair in _langaugePairs)
			{
				PairMappings.Add(new(languagePair.SourceCulture, languagePair.TargetCulture));
			}
		}

		private void SaveChanges(object parameter)
		{

		}

		private void CancelChanges(object parameter)
		{

		}
	}
}