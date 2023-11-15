using System.Collections.Generic;
using LanguageWeaverProvider.ViewModel;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;

namespace LanguageWeaverProvider.Model
{
	public class PairMapping : BaseViewModel
	{
		string _sourceCode;
		string _targetCode;
		PairModel _selectedModel;

		public string DisplayName { get; set; }

		public LanguagePair LanguagePair { get; set; }

		public List<PairModel> Models { get; set; }

		public List<PairDictionary> Dictionaries { get; set; }

		public string SourceCode
		{
			get => _sourceCode;
			set
			{
				if (_sourceCode == value) return;
				_sourceCode = value;
				OnPropertyChanged();
			}
		}

		public string TargetCode
		{
			get => _targetCode;
			set
			{
				if (_targetCode == value) return;
				_targetCode = value;
				OnPropertyChanged();
			}
		}

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

		[JsonIgnore]
		public List<LinguisticOption> LinguisticOptions => SelectedModel?.LinguisticOptions;
	}
}