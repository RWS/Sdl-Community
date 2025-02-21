using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Services.Model
{
	public class CloudAccountSettings
	{

		[JsonProperty("accountCategory")]
		public string AccountCategory { get; set; }

		[JsonProperty("accountCategoryFeatures")]
		public List<AccountCategoryFeature> AccountCategoryFeatures { get; set; }
	}
}