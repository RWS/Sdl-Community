using System;
using System.Collections.Generic;
using System.Linq;
using LanguageWeaverProvider.ViewModel;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;

namespace LanguageWeaverProvider.Model
{
	public class PairMapping : BaseViewModel
	{
		string _sourceCode;
		string _targetCode;
		string _dictionaryLabel;
		PairModel _selectedModel;
		private List<PairDictionary> _dictionaries;

		public string DisplayName { get; set; }

		public LanguagePair LanguagePair { get; set; }

		public List<PairModel> Models { get; set; }

		public List<PairDictionary> Dictionaries
		{
			get => _dictionaries;
			set
			{
				_dictionaries = value;
				if (_dictionaries is null)
				{
					return;
				}

				foreach (var pairDictionary in _dictionaries)
				{
					pairDictionary.IsSelectedChanged -= PairDictionary_IsSelectedChanged;
					pairDictionary.IsSelectedChanged += PairDictionary_IsSelectedChanged;
				}

				PairDictionary_IsSelectedChanged(null, null);
			}
		}


		public string DictionaryUILabel
		{
			get => _dictionaryLabel;
			set
			{
				_dictionaryLabel = value;
				OnPropertyChanged();
			}
		}

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

		private void PairDictionary_IsSelectedChanged(object sender, EventArgs e)
		{
			var limit = 24;
			var strings = Dictionaries.Where(x => x.IsSelected).Select(x => x.Name).ToList();
			if (!strings.Any())
			{
				DictionaryUILabel = "Select dictionaries";
				return;
			}

			var numberOfStrings = strings.Count;
			var lengthMatrix = new int[numberOfStrings + 1, limit + 1];
			for (var i = 0; i <= numberOfStrings; i++)
			{
				for (var j = 0; j <= limit; j++)
				{
					if (i == 0 || j == 0)
						lengthMatrix[i, j] = 0;
					else if (strings[i - 1].Length <= j)
						lengthMatrix[i, j] = Math.Max(1 + lengthMatrix[i - 1, j - strings[i - 1].Length], lengthMatrix[i - 1, j]);
					else
						lengthMatrix[i, j] = lengthMatrix[i - 1, j];
				}
			}

			var remainingChars = limit;
			var count = lengthMatrix[numberOfStrings, limit];
			var selectedStrings = new List<string>();

			for (var i = numberOfStrings; i > 0 && count > 0; i--)
			{
				if (lengthMatrix[i, remainingChars] != lengthMatrix[i - 1, remainingChars])
				{
					selectedStrings.Add(strings[i - 1]);
					remainingChars -= strings[i - 1].Length;
					count--;
				}
			}

			var remainingStrings = numberOfStrings - selectedStrings.Count;
			DictionaryUILabel = remainingStrings > 0 ? string.Join(", ", selectedStrings) + $" + {remainingStrings} more" : string.Join(", ", selectedStrings);
		}
	}
}