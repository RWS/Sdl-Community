using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Services.Model
{
	public class CloudSubscriptionPeriod
	{
		[JsonProperty("startDate")]
		public string StartDate { get; set; }

		[JsonProperty("endDate")]
		public string EndDate { get; set; }

		[JsonProperty("languagePairIds")]
		public List<string> LanguagePairIds { get; set; }

		[JsonProperty("userIds")]
		public List<string> UserIds { get; set; }

		[JsonProperty("labelIds")]
		public List<string> LabelIds { get; set; }

		[JsonProperty("groupIds")]
		public List<string> GroupIds { get; set; }

		[JsonProperty("applicationIds")]
		public List<string> ApplicationIds { get; set; }
	}
}
