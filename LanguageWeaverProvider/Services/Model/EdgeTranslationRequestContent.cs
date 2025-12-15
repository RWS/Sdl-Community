using System.Collections.Generic;
using System.Linq;
using LanguageWeaverProvider.Model;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Services.Model
{
	public class EdgeTranslationRequestContent
	{
		const string XliffMimeType = "application/x-xliff";

		public EdgeTranslationRequestContent(PairMapping pairMapping, string input)
		{
			LanguagePairId = pairMapping.SelectedModel.Model;
			Input = input;
			DictionaryIds = ExtractDictionaryIds(pairMapping);
			LinguisticOptions = ExtractLinguisticOptions(pairMapping);
            Title = "Trados Studio";
        }

		[JsonProperty("languagePairId")]
		public string LanguagePairId { get; }
        
        [JsonProperty("title")]
		public string Title { get; }

		[JsonProperty("input")]
		public string Input { get;  }

		[JsonProperty("inputFormat")]
		public string InputFormat => XliffMimeType;

		[JsonProperty("dictionaryIds")]
		public string DictionaryIds { get; }

		[JsonProperty("linguisticOptions")]
		public string LinguisticOptions { get; }

		private string ExtractLinguisticOptions(PairMapping pairMapping)
		{
			if (!HasElements(pairMapping.LinguisticOptions))
			{
				return default;
			}

			var activeLinguisticOptions = pairMapping
				.LinguisticOptions
				.ToDictionary(lo => lo.Id, lo => lo.SelectedValue)
				.Select(lo => $"{lo.Key}:{lo.Value}");

			var output = string.Join(",", activeLinguisticOptions);
			return output;
		}

		private string ExtractDictionaryIds(PairMapping pairMapping)
		{
			if (!HasElements(pairMapping.Dictionaries))
			{
				return default;
			}

			var selectedDictionaries = pairMapping
				.Dictionaries
				.Where(x => x.IsSelected)
				.Select(x => x.DictionaryId);
			if (!selectedDictionaries.Any())
			{
				return default;
			}

			var output = string.Join(",", selectedDictionaries);
			return output;
		}

		private bool HasElements<T>(IEnumerable<T> collection)
		{
			return collection?.Any() ?? false;
		}
	}
}
