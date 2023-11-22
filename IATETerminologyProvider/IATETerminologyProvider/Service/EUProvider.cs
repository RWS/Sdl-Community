using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.IATETerminologyProvider.Service
{
	public class EUProvider:IEUProvider
	{		
		public bool IsEULanguages(ILanguage source, ILanguage target)
		{
			string[] EULanguageArray = { "bg", "cs", "da", "de", "el", "en", "es", "et", "fi", "fr", "ga", "hr", "hu", "it", "lt", "lv", "mt", "nl", "pl", "pt", "ro", "sk", "sl", "sv" };
			var checkEULanguages = EULanguageArray.ToList().Where(a => a == source.Locale.RegionNeutralName.ToLower() || a == target.Locale.RegionNeutralName.ToLower()).ToList();
			if (checkEULanguages.Any() )
			{
				return true;
			}
			return false;
		}
	}
}
