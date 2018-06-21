namespace Sdl.Community.StarTransit.Shared.Import
{
	public static class TransitExtension
	{
		// If Three letters Windows language is not the same with the one from Star Transit extension, map it with the equivalent one.
		// See https://www.worldatlas.com/aatlas/ctycodes.htm, https://www.terena.org/activities/multiling/ml-docs/iso-639-2-dis.txt 
		// https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes or https://www.loc.gov/standards/iso639-2/php/code_list.php list of codes
		public static string MapStarTransitLanguage(string fileExtension)
		{
			fileExtension = fileExtension.ToUpper();
			switch (fileExtension)
			{
				case "CYM":
					return "WEL";
				case "MNN":
					return "MNG";
				case "NGA":
					return "EDO,EFI,NGA";
				case "FYN":
					return "FRY,FYN";
				case "GLA":
					return "GAE,GLA,GDH";
				case "ELL":
					return "GRC, ELL";
				case "ITA":
					return "ITS,ITA";
				case "ITS":
					return "ITA,ITS";
				case "KKZ":
					return "KAZ,KKZ";
				case "KIR":
					return "KYR,KIR";
				case "KYR":
					return "KIR,KYR";
				case "LAT":
					return "LAT";
				case "SOT":
					return "NBL,SOT,SRL,VEN";
				case "TSN":
					return "NBL,TSN,SRL,TNA,VEN";
				case "TSO":
					return "NBL,TSO,SRL,VEN";
				case "AFK":
					return "NBL,TSO,SRL,VEN";
				case "XHO":
					return "NBL,XHO,SRL,VEN";
				case "ZUL":
					return "NBL,ZUL,SRL,VEN";
				case "ENW":
					return "NDE,ENW";
				case "SNA":
					return "NDE,SNA";
				case "ZZZ":
					return "SSW,ZZZ";
				case "TUK":
					return "TKM,TUK";
				default:
					return fileExtension;
			}
		}
	}
}