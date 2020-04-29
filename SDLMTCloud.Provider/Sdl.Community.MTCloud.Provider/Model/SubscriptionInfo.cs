using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class SubscriptionInfo
	{
		[JsonProperty("languagePairs")]
		public List<MTCloudLanguagePair> LanguagePairs { get; set; }		
	}
}
