using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.IATETerminologyProvider.Interface;
using TradosStudio.API.TranslationResources.Terminology;


namespace Sdl.Community.IATETerminologyProvider.Service
{
	public static class EUProvider
	{		
		public static bool IsEULanguages(ILanguage source, ILanguage target)
		{
			string[] EULanguageArray = { "bg", "cs", "da", "de", "el", "en", "es", "et", "fi", "fr", "ga", "hr", "hu", "it", "lt", "lv", "mt", "nl", "pl", "pt", "ro", "sk", "sl", "sv" };
			var checkEULanguages = EULanguageArray.ToList().Where(a => 
            a == CultureInfo.GetCultureInfo(source.LanguageIsoCode).TwoLetterISOLanguageName.ToLower()
            || a == CultureInfo.GetCultureInfo(target.LanguageIsoCode).TwoLetterISOLanguageName).ToList();
			if (checkEULanguages.Any() )
			{
				return true;
			}
			return false;
		}
	}
}
