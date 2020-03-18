using System.Collections.Generic;
using RestSharp;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class SubscriptionInfo
	{
		public List<BeGlobalLanguagePair> LanguagePairs { get; set; }
		public RestResponse RestResponse { get; set; }
	}
}
