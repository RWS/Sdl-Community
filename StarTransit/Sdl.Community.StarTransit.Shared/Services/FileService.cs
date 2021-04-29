using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.Core.Globalization;

namespace Sdl.Community.StarTransit.Shared.Services
{
	public class FileService: IFileService
	{
		private const string TmFileType = "ExtFileType=\"Extract\"";
		private const string FileType = "Transit";
		private const string MtFilesName = "_AEXTR_MT";

		public string[] GetTransitCorrespondingExtension(CultureInfo languageCulture)
		{
			var extension = languageCulture.ThreeLetterWindowsLanguageName;
			extension = MapStarTransitLanguage(extension);

			// used for following scenario: for one Windows language (Ex: Nigeria), Star Transit might use different extensions (eg: EDO,EFI)
			return extension.Split(',');
		}

		/// <summary>
		/// MT Files are considered to be TMs. However the Transit MT file does not conain |"
		/// </summary>
		public bool IsTransitTm(string filePath)
		{
			if (!File.Exists(filePath)) return false;
			var name = Path.GetFileName(filePath);
			var containsMtName = name.Contains(MtFilesName);
			using (var reader = new StreamReader(filePath, Encoding.Default))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (line.Trim().Contains(TmFileType))
					{
						return true;
					}

					if (containsMtName)
					{
						if (line.Contains(FileType))
						{
							return true;
						}
					}
					if (line.Equals("<Header>")) break;
				}
			}

			return false;
		}

		public bool IsTransitFile(string filePath)
		{
			if (!File.Exists(filePath)) return false;
			using (var reader = new StreamReader(filePath, Encoding.Default))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (line.Contains(FileType))
					{
						return true;
					}
					if(line.Equals("<Header>")) break;
				}
			}

			return false;
		}

		public bool IsValidNode(XmlNode originalXmlNode)
		{
			return originalXmlNode.ChildNodes.Cast<XmlNode>().Any(childNode => childNode.NodeType == XmlNodeType.Text && !string.IsNullOrWhiteSpace(childNode.Value));
		}

		public Language[] GetStudioTargetLanguages(List<LanguagePair> languagePairs)
		{
			return languagePairs.Select(pair => new Language(pair.TargetLanguage)).ToArray();
		}

		public string MapFileLanguage(string fileExtension)
		{
			if (string.IsNullOrEmpty(fileExtension)) return string.Empty;
			fileExtension = fileExtension.ToUpper();
			switch (fileExtension)
			{
				case "DEU":
					return "de-DE";
				case "AFK":
					return "af-ZA";
				case "AMH":
					return "am-ET";
				case "SQI":
					return "sq-AL";
				case "ARG":
					return "ar-DZ";
				case "ARH":
					return "ar-BH";
				case "ARE":
					return "ar-EG";
				case "ARI":
					return "ar-IQ";
				case "ARJ":
					return "ar-JO";
				case "ARK":
					return "ar-KW";
				case "ARB":
					return "ar-LB";
				case "ARL":
					return "ar-LY";
				case "ARM":
					return "ar-MA";
				case "ARO":
					return "ar-OM";
				case "ARQ":
					return "ar-QA";
				case "ARA":
					return "ar-SA";
				case "ARS":
					return "ar-SY";
				case "ART":
					return "ar-TN";
				case "ARU":
					return "ar-AE";
				case "ARY":
					return "ar-YE";
				case "EUQ":
					return "eu-ES";
				case "BEL":
					return "be-BY";
				case "BGR":
					return "bg-BG";
				case "CAT":
					return "ca-ES";
				case "CHS":
					return "zh-CN";
				case "ZHH":
					return "zh-HK";
				case "ZHI":
					return "zh-SG";
				case "CHT":
					return "zh-TW";
				case "HRV":
					return "hr-HR";
				case "CSY":
					return "cs-CZ";
				case "DAN":
					return "da-DK";
				case "NLB":
					return "nl-BE";
				case "NLD":
					return "nl-NL";
				case "ENA":
					return "en-AU";
				case "ENL":
					return "en-BZ";
				case "ENC":
					return "en-CA";
				case "ENI":
					return "en-IE";
				case "ENJ":
					return "en-JM";
				case "ENZ":
					return "en-NZ";
				case "ENS":
					return "en-ZA";
				case "ENT":
					return "en-TT";
				case "ENG":
					return "en-GB";
				case "ENU":
					return "en-US";
				case "ETI":
					return "et-EE";
				case "FOS":
					return "fo-FO";
				case "FAR":
					return "fa-IR";
				case "FIN":
					return "fi-FI";
				case "FRB":
					return "fr-BE";
				case "FRC":
					return "fr-CA";
				case "FRL":
					return "fr-LU";
				case "FRS":
					return "fr-CH";
				case "DEA":
					return "de-AT";
				case "DEC":
					return "de-LI";
				case "DEL":
					return "de-LU";
				case "DES":
					return "de-CH";
				case "ELL":
					return "el-GR";
				case "HEB":
					return "he-IL";
				case "HIN":
					return "hi-IN";
				case "HUN":
					return "hu-HU";
				case "ISL":
					return "is-IS";
				case "ITA":
					return "it-IT";
				case "ITS":
					return "it-CH";
				case "JPN":
					return "ja-JP";
				case "KOR":
					return "ko-KR";
				case "LVI":
					return "lv-LV";
				case "LTH":
					return "lt-LT";
				case "MKD":
					return "mk-MK";
				case "PLK":
					return "pl-PL";
				case "PTB":
					return "pt-BR";
				case "ROM":
					return "ro-RO";
				case "RUS":
					return "ru-RU";
				case "SKY":
					return "sk-SK";
				case "SLV":
					return "sl-SI";
				case "ESS":
					return "es-AR";
				case "ESB":
					return "es-BO";
				case "ESL":
					return "es-CL";
				case "ESO":
					return "es-CO";
				case "ESC":
					return "es-CR";
				case "ESD":
					return "es-DO";
				case "ESF":
					return "es-EC";
				case "ESE":
					return "es-SV";
				case "ESG":
					return "es-GT";
				case "ESH":
					return "es-HN";
				case "ESM":
					return "es-MX";
				case "ESI":
					return "es-NI";
				case "ESA":
					return "es-PA";
				case "ESZ":
					return "es-PY";
				case "ESR":
					return "es-PE";
				case "ES":
					return "es-PR";
				case "ESP":
					return "es-ES";
				case "ESY":
					return "es-UY";
				case "ESV":
					return "es-VE";
				case "SVF":
					return "sv-FI";
				case "THA":
					return "th-TH";
				case "TRK":
					return "tr-TR";
				case "UKR":
					return "uk-UA";
				case "URD":
					return "ur-PK";
				case "VIT":
					return "vi-VN";
				case "AZC":
					return "az-Cyrl-AZ";
				case "AZE":
					return "az-Cyrl-AZ";
				case "ENN":
					return "en-IN";
				case "ENM":
					return "en-MY";
				case "ENP":
					return "en-PH";
				case "FRA":
					return "fr-FR";
				case "FRM":
					return "fr-MC";
				case "FRO":
					return "fo-FO";
				case "FRY":
					return "fy-NL";
				case "GAL":
					return "gl-ES";
				case "GRC":
					return "el-GR";
				case "IBO":
					return "ig-NG";
				case "IND":
					return "id-ID";
				case "KAZ":
					return "kk-KZ";
				case "MNG":
					return "mn-MN";
				case "MSB":
					return "ms-BN";
				case "MSL":
					return "ms-MY";
				case "NON":
					return "nn-NO";
				case "NOR":
					return "nb-NO";
				case "NSO":
					return "nso-ZA";
				case "PTG":
					return "pt-PT";
				case "SRL":
					return "sr";
				case "SVE":
					return "sv-SE";
				case "SRB":
					return "sr-Latn";
				case "SWK":
					return "sw-KE";
				case "TKM":
					return "tk-TM";
				case "UZB":
					return "es-VE";
				case "VEN":
					return "uz-Cyrl-UZ";
				case "ZHM":
					return "zh-MO";
				case "ZUL":
					return "zu-ZA";
				case "ROU":
					return "ro-RO";
				case "WEL":
					return "cy-GB";

				default:
					return "";
			}
		}

		public string MapStarTransitLanguage(string fileExtension)
		{
			if (string.IsNullOrEmpty(fileExtension)) return string.Empty;
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
					return "GRC,ELL";
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
