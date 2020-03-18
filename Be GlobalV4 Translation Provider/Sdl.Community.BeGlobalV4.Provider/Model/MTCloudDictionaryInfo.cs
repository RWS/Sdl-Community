using System.Collections.Generic;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class MTCloudDictionaryInfo
	{
		public int AccountId { get; set; }
		public List<MTCloudDictionary> Dictionaries {get; set;}
	}
}