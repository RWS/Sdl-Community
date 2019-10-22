using System.Collections.Generic;
using RestSharp;

namespace Sdl.Community.BeGlobalV4.Provider.Model
{
	public class SubscriptionInfo
	{
		public List<BeGlobalLanguagePair> LanguagePairs { get; set; }
		public RestResponse RestResponse { get; set; }
	}
}
