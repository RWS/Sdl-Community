using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Services.Model
{
	public class CloudAccount
	{
		[JsonProperty("accountId")]
		public string AccountID { get; set; }

		[JsonProperty("totalCount")]
		public string TotalCount { get; set; }

		[JsonProperty("subscriptions")]
		public IEnumerable<CloudAccountSubscription> Subscriptions { get; set; }

		[JsonProperty("errors")]
		public List<CloudAccountError> Errors { get; set; }
	}
}
