using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.DeelLMTProvider
{
	public static class Helpers
	{
		private static List<string> _sourceSupportedLanguages = new List<string> { "EN", "DE", "FR", "IT", "NL", "PL", "ES" };
		private static List<string> _targetSupportedLanguages = new List<string> { "EN", "DE", "FR", "IT", "NL", "PL", "ES" };

		public static bool IsSuportedLanguagePair(string sourceLang, string targetLang)
		{
			if(_sourceSupportedLanguages.Contains(sourceLang)&& _targetSupportedLanguages.Contains(targetLang))
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
