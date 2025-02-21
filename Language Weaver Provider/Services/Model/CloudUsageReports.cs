using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Services.Model
{
	internal class CloudUsageReports
	{
		[JsonProperty("report")]
		public List<CloudUsageReport> Reports { get; set; }
	}
}