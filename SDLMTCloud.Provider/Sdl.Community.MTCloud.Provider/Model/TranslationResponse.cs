using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class TranslationResponse
	{
		[JsonProperty("inputFormat")]
		public string InputFormat { get; set; }

		[JsonProperty("model")]
		public string Model { get; set; }

		[JsonProperty("requestId")]
		public string RequestId { get; set; }

		[JsonProperty("sourceLanguageId")]
		public string SourceLanguageId { get; set; }

		[JsonProperty("submissionType")]
		public string SubmissionType { get; set; }

		[JsonProperty("targetLanguageId")]
		public string TargetLanguageId { get; set; }

		[JsonProperty("translation")]
		public List<string> Translation { get; set; }
	}
}