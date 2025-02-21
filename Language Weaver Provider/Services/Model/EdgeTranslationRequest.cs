using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Services.Model
{
	public class EdgeTranslationRequest
	{
		[JsonProperty("input")]
		public string[] Input { get; set; }

		[JsonProperty("inputFormat")]
		public string InputFormat { get; set; }

		[JsonProperty("languagePairId")]
		public string LanguagePairId { get; set; }

		[JsonProperty("dictionaryIds")]
		public object[] DictionaryIds { get; set; }

		[JsonProperty("linguisticOptions")]
		public Dictionary<string, string> LinguisticOptions { get; set; }
	}
}