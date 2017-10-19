using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.DeelLMTProvider
{
	public static class Helpers
	{
		private static List<string> _sourceSupportedLanguages = new List<string> { "EN", "DE", "FR", "IT", "NL", "PL" };
		private static List<string> _targetSupportedLanguages = new List<string> { "EN", "DE", "FR", "IT", "NL", "PL" };

		public static bool IsSuportedLanguagePair(string sourceLang, string targetLang)
		{
			if(_sourceSupportedLanguages.Contains(sourceLang)&& _targetSupportedLanguages.Contains(targetLang))
			{
				return true;
			}
			return false;
		}



	}
}
