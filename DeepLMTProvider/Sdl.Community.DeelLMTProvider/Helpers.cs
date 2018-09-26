using System.Collections.Generic;
using System.Net;

namespace Sdl.Community.DeepLMTProvider
{
	public static class Helpers
	{
		private static readonly List<string> SourceSupportedLanguages = new List<string> { "EN", "DE", "FR", "IT", "NL", "PL", "ES" };
		private static readonly List<string> TargetSupportedLanguages = new List<string> { "EN", "DE", "FR", "IT", "NL", "PL", "ES" };
		public static bool IsSuportedLanguagePair(string sourceLang, string targetLang)
		{
			if(SourceSupportedLanguages.Contains(sourceLang)&& TargetSupportedLanguages.Contains(targetLang))
			{
				return true;
			}
			return false;
		}

		public static string ProcessWebException(WebException exception)
		{			
			var response = (HttpWebResponse)exception.Response;		
			return string.Format("Http status code={0}, error message= {1}", response.StatusCode, exception.Message);
		}

	}
}
