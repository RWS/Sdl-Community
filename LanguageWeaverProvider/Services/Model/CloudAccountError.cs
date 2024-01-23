using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Services.Model
{
	public class CloudAccountError
	{
		[JsonProperty("code")]
		public int Code { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }
	}

	public class CloudAccountErrors
	{
		[JsonProperty("errors")]
		public List<CloudAccountError> Errors { get; set; }
	}
}