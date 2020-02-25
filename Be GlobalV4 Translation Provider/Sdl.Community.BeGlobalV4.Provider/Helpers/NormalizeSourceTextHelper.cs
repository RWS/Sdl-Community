using System.Globalization;

namespace Sdl.Community.BeGlobalV4.Provider.Helpers
{
	public class NormalizeSourceTextHelper
	{
		public string GetCorrespondingLangCode(CultureInfo cultureInfo)
		{
			if (cultureInfo != null)
			{
				switch (cultureInfo.ThreeLetterISOLanguageName)
				{
					case "zho":
						//Chinese (Traditional, Macao S.A.R.),(Traditional, Hong Kong SAR),(Traditional, Taiwan)
						if (cultureInfo.Name.Equals("zh-MO") || cultureInfo.Name.Equals("zh-HK") || cultureInfo.Name.Equals("zh-TW"))
						{
							return "cht";
						}
						//Simplified Chinese
						return "chi";
					case "deu":
						return "ger";
					case "nld":
						return "dut";
					case "slk":
						return "slo";
					case "ron":
						return "rum";
					case "ces":
						return "cze";
					case "ell":
						return "gre";
					case "msa":
						return "may";
					case "nno":
					case "nob":
						return "nor";
					case "por":
						// Portuguese (Brazil)
						if (cultureInfo.Name.Equals("pt-BR"))
						{
							return "ptb";
						}						
						return "por";
				}
				return cultureInfo.ThreeLetterISOLanguageName;
			}
			return string.Empty;
		}
	}
}