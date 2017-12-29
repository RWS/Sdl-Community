using System;

namespace Sdl.Community.AdaptiveMT.Service.Helpers
{
	public static class ApiUrls
	{
		public static readonly string BaseUri = @"https://lc-api.sdl.com";
		//public static readonly string Login = string.Concat(BaseUri, @"/studio/login");
		public static Uri Login()
		{
			return "{0}/studio/login".FormatUri(BaseUri);
		}

		public static Uri Feedback()
		{
			return "{0}/feedback".FormatUri(BaseUri);
		}
		public static Uri Session(string oosAccountId)
		{
			return "{0}/studio/session/oos-account/{1}".FormatUri(BaseUri,oosAccountId);
		}
	}
}
