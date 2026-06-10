using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Services.Model
{
	public class CloudTranslationStatus
	{
		[JsonProperty("translationStatus")]
		public string Status { get; set; }

		public string InputFormat { get; set; }

		public string OutputFormat { get; set; }

		public int TranslationProgressPercent { get; set; }

		public CloudTranslationStats TranslationStats { get; set; }

		public List<CloudLanguagePair> TranslationLanguagePairs { get; set; }

		[JsonProperty("qualityEstimation")]
		public List<CloudQualityEstimation> QualityEstimation { get; set; }
	}

	public class CloudQualityEstimation
	{
		[JsonProperty("good")]
		public int Good { get; set; }

		[JsonProperty("adequate")]
		public int Adequate { get; set; }

		[JsonProperty("poor")]
		public int Poor { get; set; }

		/// <summary>Returns the dominant quality label based on the highest percentage score.</summary>
		public string DominantLabel()
		{
			if (Good >= Adequate && Good >= Poor) return "Good";
			if (Adequate >= Poor) return "Adequate";
			return "Poor";
		}
	}
}
