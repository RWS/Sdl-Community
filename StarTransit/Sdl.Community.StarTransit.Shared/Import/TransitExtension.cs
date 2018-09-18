namespace Sdl.Community.StarTransit.Shared.Import
{
	public static class TransitExtension
	{
		// If Three letters Windows language is not the same with the one from Star Transit extension, map it with the equivalent one.
		// See: https://www.worldatlas.com/aatlas/ctycodes.htm, https://www.terena.org/activities/multiling/ml-docs/iso-639-2-dis.txt 
		// https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes or https://www.loc.gov/standards/iso639-2/php/code_list.php list of ISO 639-1 amd ISO 639-2 codes
		// ftp://ftp.star-group.net/LTS/TransitNXT/TecDoc/TransitNXT_Advanced_AdvancedFeatures_ENG.pdf list of language codes used by StarTransit
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
				case "BIN":
					return "EDO,EFI,NGA";
				case "FYN":
					return "FRY,FYN";
				case "GLA":
				case "GLE":
					return "GAE,GLA,GDH";
				case "ELL":
					return "GRC, ELL";
				case "ITA":
				case "ITS":
					return "ITS,ITA";
				case "KKZ":
					return "KAZ,KKZ";
				case "KIR":
				case "KYR":
					return "KYR,KIR";
				case "LAT":
					return "LAT";
				case "SOT":
					return "SXT";
				case "TSN":
					return "NBL,TSN,SRL,TNA,VEN";
				case "TSO":
					return "TSG";
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
				case "ENN":
					return "END";
				case "ENE":
					return "ENO";
				case "FRR":
					return "FRU";
				case "FRI":
					return "FRV";
				case "FRD":
				case "MAF":
				case "GLP":
					return "FRW";
				case "FUL":
				case "FUB":
					return "FUB";
				case "GRN":
					return "GUA";
				case "YOR":
					return "YBA";
				case "MLT":
					return "MTL";
				case "ORM":
					return "ORO";
				case "RMC":
					return "RMS";
				case "ROD":
					return "ROV";
				case "SOM":
					return "SML";
				case "SMB":
				case "SMA":
				case "SMK":
				case "SMJ":
				case "SMN":
				case "SMS":
					return "SZI";
				case "TIR":
					return "TGE";
				case "TIE":
					return "TGY";
				case "GLC":
					return "GAL";
				case "SRM":
				case "SRS":
				case "SRP":
					return "SRL";
				case "SSW":
					return "SSW";
				case "NDE":
					return "NDE";
				case "NBL":
					return "NBL";
				case "VEN":
					return "VEN";
				case "NLD":
				case "NLB":
					return "NLD,NLB,NLS"; 
				default:
					return fileExtension;
			}
		}
	}
}