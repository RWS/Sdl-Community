using System.Globalization;

namespace Sdl.Community.BeGlobalV4.Provider.Helpers
{
	public class NormalizeSourceTextHelper
	{
		public string GetCorespondingLangCode(CultureInfo cultureInfo)
		{
			if (cultureInfo != null)
			{
				if (cultureInfo.ThreeLetterISOLanguageName.Equals("zho"))
				{
					//Chinese (Traditional, Macao S.A.R.),(Traditional, Hong Kong SAR),(Traditional, Taiwan)
					if (cultureInfo.Name.Equals("zh-MO") || cultureInfo.Name.Equals("zh-HK") || cultureInfo.Name.Equals("zh-TW"))
					{
						return "cht";
					}
					//Simplified Chinese
					return "chi";
				}
				if (cultureInfo.ThreeLetterISOLanguageName.Equals("deu"))
				{
					return "ger";
				}
				//Language code for Dutch in BeGlobal is dut
				if (cultureInfo.ThreeLetterISOLanguageName.Equals("nld"))
				{
					return "dut";
				}
				//Language code for Slovak in BeGlobal is slo
				if (cultureInfo.ThreeLetterISOLanguageName.Equals("slk"))
				{
					return "slo";
				}
				//Language code for Romanian in BeGlobal is rum
				if (cultureInfo.ThreeLetterISOLanguageName.Equals("ron"))
				{
					return "rum";
				}
				//Language code for Czech in BeGlobal is cze
				if (cultureInfo.ThreeLetterISOLanguageName.Equals("ces"))
				{
					return "cze";
				}
				//Language code for Greek in BeGlobal is gre
				if (cultureInfo.ThreeLetterISOLanguageName.Equals("ell"))
				{
					return "gre";
				}
				//Language code for Malay in BeGlobal is may
				if (cultureInfo.ThreeLetterISOLanguageName.Equals("msa"))
				{
					return "may";
				}
				//Language code for Norwegian (Bokmal) or Norwegian (Nynorsk) in BeGlobal is nor
				if (cultureInfo.ThreeLetterISOLanguageName.Equals("nno") || cultureInfo.ThreeLetterISOLanguageName.Equals("nob"))
				{
					return "nor";
				}				
				return cultureInfo.ThreeLetterISOLanguageName;
			}
			return string.Empty;
		}
	}
}
