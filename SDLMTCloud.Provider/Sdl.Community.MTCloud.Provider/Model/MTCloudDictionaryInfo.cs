using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class MTCloudDictionaryInfo
	{
		[JsonProperty("accountId")]
		public long AccountId { get; set; }

		[JsonProperty("dictionaries")]
		public List<MTCloudDictionary> Dictionaries {get; set;}
	}
}