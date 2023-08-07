using System.Collections.ObjectModel;
using System.Windows.Input;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using Sdl.LanguagePlatform.Core;

namespace LanguageWeaverProvider.ViewModel.Cloud
{
	public class PairsMappingViewModel : BaseViewModel
	{
		private readonly LanguagePair[] _langaugePairs;

		private ITranslationOptions _translationOptions;
		private ObservableCollection<PairMapping> _pairMappings;

		public PairsMappingViewModel(ITranslationOptions translationOptions, LanguagePair[] languagePairs)
		{
			_langaugePairs = languagePairs;
			_translationOptions = translationOptions;
			InitializeCommands();
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

		public ICommand OpenLanguageMappingProviderCommand { get; set; } 

		private void InitializeCommands()
		{
			SaveCommand = new RelayCommand(SaveChanges);
			CancelCommand = new RelayCommand(CancelChanges);
			OpenLanguageMappingProviderCommand = new RelayCommand(OpenLanguageMappingProvider);
		}

		private void SaveChanges(object parameter)
		{

		}

		private void CancelChanges(object parameter)
		{

		}

		private void OpenLanguageMappingProvider(object parameter)
		{

		}
	}
}