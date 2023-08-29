using System.Collections.Generic;

namespace LanguageWeaverProvider.Services.Model
{
	public class TranslationRequest
	{
		public string[] Dictionaries { get; set; }

		public string[] Input { get; set; }

		public string InputFormat { get; set; }

		public string Model { get; set; }

		public int QualityEstimation { get; set; }

		public string SourceLanguageId { get; set; }

		public string TargetLanguageId { get; set; }

		public Dictionary<string, string> LinguisticOptions { get; set; }
	}
}