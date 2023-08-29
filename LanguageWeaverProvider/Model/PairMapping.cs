using System.Collections.Generic;
using System.Linq;
using LanguageWeaverProvider.ViewModel;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;

namespace LanguageWeaverProvider.Model
{
	public class PairMapping : BaseViewModel
	{
		PairModel _selectedModel;
		PairDictionary _selectedDictionary;

		public string DisplayName { get; set; }

		public string SourceCode { get; set; }

		public string TargetCode { get; set; }

		public LanguagePair LanguagePair { get; set; }

		public List<PairModel> Models { get; set; }

		public PairModel SelectedModel
		{
			get => _selectedModel;
			set
			{
				_selectedModel = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(LinguisticOptions));
			}
		}

		public List<PairDictionary> Dictionaries { get; set; }

		public PairDictionary SelectedDictionary
		{
			get => _selectedDictionary;
			set
			{
				_selectedDictionary = value;
				OnPropertyChanged();
			}
		}

		[JsonIgnore]
		public List<LinguisticOption> LinguisticOptions => SelectedModel?.LinguisticOptions;
	}
}