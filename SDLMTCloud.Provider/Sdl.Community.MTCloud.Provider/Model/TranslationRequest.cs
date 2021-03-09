﻿using Newtonsoft.Json;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class TranslationRequest
	{
		[JsonProperty("dictionaries")]
		public string[] Dictionaries { get; set; }

		[JsonProperty("input")]
		public string[] Input { get; set; }

		[JsonProperty("inputFormat")]
		public string InputFormat { get; set; }

		[JsonProperty("model")]
		public string Model { get; set; }

		[JsonProperty("qualityEstimation")]
		public int QualityEstimation { get; set; }

		[JsonProperty("sourceLanguageId")]
		public string SourceLanguageId { get; set; }

		[JsonProperty("targetLanguageId")]
		public string TargetLanguageId { get; set; }
	}
}