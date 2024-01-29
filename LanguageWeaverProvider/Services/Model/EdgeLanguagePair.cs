using System.Collections.Generic;
using LanguageWeaverProvider.Model;

namespace LanguageWeaverProvider.Services.Model
{
	public class EdgeLanguagePair
	{
		public string LanguagePairId { get; set; }

		public string SourceLanguage { get; set; }

		public string SourceLanguageId { get; set; }

		public string SourceLanguageTag { get; set; }

		public string TargetLanguage { get; set; }

		public string TargetLanguageId { get; set; }

		public string TargetLanguageTag { get; set; }

		public string Domain { get; set; }

		public string Model { get; set; }

		public string Platform { get; set; }

		public string Technology { get; set; }

		public string Version { get; set; }

		public bool IsEdgeCloud { get; set; }

		public List<LinguisticOption> LinguisticOptions { get; set; }

		public bool? Adaptable { get; set; }
	}
}