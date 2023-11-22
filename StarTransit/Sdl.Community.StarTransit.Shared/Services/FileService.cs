using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.Core.Globalization;
using Sdl.Core.Globalization.LanguageRegistry;

namespace Sdl.Community.StarTransit.Shared.Services
{
	public class FileService : IFileService
	{
		private const string FileType = "Transit";
		private Dictionary<string, string> _starTransitLanguageDictionary;
		private Dictionary<string, string> _starTransitFileLanguageDictionary;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public FileService()
		{
			BuildTransitLanguageDictionary();
			BuildTransitFileLanguage();
		}

		public string[] GetTransitCorrespondingExtension(CultureInfo languageCulture)
		{
			var extension = languageCulture.ThreeLetterWindowsLanguageName;
			extension = MapStarTransitLanguage(extension);

			// used for following scenario: for one Windows language (Ex: Nigeria), Star Transit might use different extensions (eg: EDO,EFI)
			return extension.Split(',');
		}

		public string[] GetTransitCorrespondingExtension(string fileExtension)
		{
			return MapStarTransitLanguage(fileExtension).Split(',');
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
					if (line.Equals("<Header>")) break;
				}
			}
			return false;
		}

		public bool IsValidNode(string dataAttributeHexCode)
		{
			return !dataAttributeHexCode.StartsWith("C0E80FE9");
		}

		public Language[] GetStudioTargetLanguages(List<LanguagePair> languagePairs)
		{
			return languagePairs != null ? languagePairs.Select(pair => LanguageRegistryApi.Instance.GetLanguage(pair.TargetLanguage.Name)).ToArray() : new List<Language>().ToArray();
		}

		public bool AreFilesExtensionsSupported(string sourceFileExtension, string targetFileExtension)
		{
			var sourceExtensions = GetTransitCorrespondingExtension(sourceFileExtension);
			var sourceCodeExists = sourceExtensions.Any(s => s.Contains(sourceFileExtension));

			var targetExtensions = GetTransitCorrespondingExtension(targetFileExtension);
			var targetCodeExists = targetExtensions.Any(t => t.Contains(targetFileExtension));

			if (sourceCodeExists && targetCodeExists) return true;
			_logger.Info($"Transit source file extension {sourceFileExtension} or target file extension: {targetFileExtension} could not be mapped by the plugin.");
			return false;
		}

		public string MapFileLanguage(string fileExtension)
		{
			if (string.IsNullOrEmpty(fileExtension)) return string.Empty;
			fileExtension = fileExtension.ToUpper();
			var fileExtensionExists =
				_starTransitFileLanguageDictionary.TryGetValue(fileExtension, out var languageCode);
			return fileExtensionExists ? languageCode : string.Empty;
		}

		public string MapStarTransitLanguage(string fileExtension)
		{
			if (string.IsNullOrEmpty(fileExtension)) return string.Empty;

			var languageExists = _starTransitLanguageDictionary.TryGetValue(fileExtension, out var transitLanguageExtension);
			return languageExists ? transitLanguageExtension : fileExtension;
		}

		public string ConvertStringToHex(string input, Encoding encoding)
		{
			var stringBytes = encoding.GetBytes(input);
			var sbBytes = new StringBuilder(stringBytes.Length * 2);
			foreach (var b in stringBytes)
			{
				sbBytes.AppendFormat("{0:X2}", b);
			}
			return sbBytes.ToString();
		}

		private void BuildTransitLanguageDictionary()
		{
			_starTransitLanguageDictionary = new Dictionary<string, string>
			{
				{"CYM", "WEL"},
				{"MNN", "MNG"},
				{"NGA", "EDO,EFI,NGA"},
				{"BIN", "EDO,EFI,NGA"},
				{"FYN", "FRY,FYN"},
				{"GLA", "GAE,GLA,GDH"},
				{"GLE", "GAE,GLA,GDH"},
				{"ELL", "GRC,ELL"},
				{"ITA", "ITS,ITA"},
				{"ITS", "ITS,ITA"},
				{"KKZ", "KAZ,KKZ"},
				{"KIR", "KYR,KIR"},
				{"KYR", "KYR,KIR"},
				{"LAT", "LAT"},
				{"SOT", "SXT"},
				{"TSN", "NBL,TSN,SRL,TNA,VEN"},
				{"TSO", "TSG"},
				{"AFK", "NBL,TSO,SRL,VEN"},
				{"XHO", "NBL,XHO,SRL,VEN"},
				{"ZUL", "NBL,ZUL,SRL,VEN"},
				{"ENW", "NDE,ENW"},
				{"SNA", "NDE,SNA"},
				{"ZZZ", "SSW,ZZZ"},
				{"TUK", "TKM,TUK"},
				{"ENN", "END"},
				{"ENE", "ENO"},
				{"FRR", "FRU"},
				{"FRI", "FRV"},
				{"FRD", "FRW"},
				{"MAF", "FRW"},
				{"GLP", "FRW"},
				{"FUL", "FUB"},
				{"FUB", "FUB"},
				{"GRN", "GUA"},
				{"YOR", "YBA"},
				{"MLT", "MLT"},
				{"ORM", "ORO"},
				{"RMC", "RMS"},
				{"ROD", "ROV"},
				{"SOM", "SML"},
				{"SMB", "SZI"},
				{"SMA", "SZI"},
				{"SMK", "SZI"},
				{"SMJ", "SZI"},
				{"SMN", "SZI"},
				{"SMS", "SZI"},
				{"TIR", "TGE"},
				{"TIE", "TGY"},
				{"GLC", "GAL"},
				{"SRM", "SRL"},
				{"SRS", "SRL"},
				{"SRP", "SRL"},
				{"SRO","SRB" }, // Serbian Cyrillic
				{"SSW", "SSW"},
				{"NDE", "NDE"},
				{"NBL", "NBL"},
				{"VEN", "VEN"},
				{"NLD", "NLD,NLB,NLS"},
				{"NLB", "NLD,NLB,NLS"}
			};
		}

		private void BuildTransitFileLanguage()
		{
			_starTransitFileLanguageDictionary = new Dictionary<string, string>
			{
				{"DEU", "de-DE"},
				{"AFK", "af-ZA"},
				{"AMH", "am-ET"},
				{"SQI", "sq-AL"},
				{"ARG", "ar-DZ"},
				{"ARH", "ar-BH"},
				{"ARE", "ar-EG"},
				{"ARI", "ar-IQ"},
				{"ARJ", "ar-JO"},
				{"ARK", "ar-KW"},
				{"ARB", "ar-LB"},
				{"ARL", "ar-LY"},
				{"ARM", "ar-MA"},
				{"ARO", "ar-OM"},
				{"ARQ", "ar-QA"},
				{"ARA", "ar-SA"},
				{"ARS", "ar-SY"},
				{"ART", "ar-TN"},
				{"ARU", "ar-AE"},
				{"ARY", "ar-YE"},
				{"EUQ", "eu-ES"},
				{"BEL", "be-BY"},
				{"BGR", "bg-BG"},
				{"CAT", "ca-ES"},
				{"CHS", "zh-CN"},
				{"ZHH", "zh-HK"},
				{"ZHI", "zh-SG"},
				{"CHT", "zh-TW"},
				{"HRV", "hr-HR"},
				{"CSY", "cs-CZ"},
				{"DAN", "da-DK"},
				{"NLB", "nl-BE"},
				{"NLD", "nl-NL"},
				{"ENA", "en-AU"},
				{"ENL", "en-BZ"},
				{"ENC", "en-CA"},
				{"ENI", "en-IE"},
				{"ENJ", "en-JM"},
				{"ENZ", "en-NZ"},
				{"ENS", "en-ZA"},
				{"ENT", "en-TT"},
				{"ENG", "en-GB"},
				{"ENU", "en-US"},
				{"ETI", "et-EE"},
				{"FOS", "fo-FO"},
				{"FAR", "fa-IR"},
				{"FIN", "fi-FI"},
				{"FRB", "fr-BE"},
				{"FRC", "fr-CA"},
				{"FRL", "fr-LU"},
				{"FRS", "fr-CH"},
				{"DEA", "de-AT"},
				{"DEC", "de-LI"},
				{"DEL", "de-LU"},
				{"DES", "de-CH"},
				{"ELL", "el-GR"},
				{"HEB", "he-IL"},
				{"HIN", "i-IN"},
				{"HUN", "hu-HU"},
				{"ISL", "is-IS"},
				{"ITA", "it-IT"},
				{"ITS", "it-CH"},
				{"JPN", "ja-JP"},
				{"KOR", "ko-KR"},
				{"LVI", "lv-LV"},
				{"LTH", "lt-LT"},
				{"MKD", "mk-MK"},
				{"PLK", "pl-PL"},
				{"PTB", "pt-BR"},
				{"ROM", "ro-RO"},
				{"RUS", "ru-RU"},
				{"SKY", "sk-SK"},
				{"SLV", "sl-SI"},
				{"ESS", "es-AR"},
				{"ESB", "es-BO"},
				{"ESL", "es-CL"},
				{"ESO", "es-CO"},
				{"ESC", "es-CR"},
				{"ESD", "es-DO"},
				{"ESF", "es-EC"},
				{"ESE", "es-SV"},
				{"ESG", "es-GT"},
				{"ESH", "es-HN"},
				{"ESM", "es-MX"},
				{"ESI", "es-NI"},
				{"ESA", "es-PA"},
				{"ESZ", "es-PY"},
				{"ESR", "es-PE"},
				{"ES", "es-PR"},
				{"ESP", "es-ES"},
				{"ESY", "es-UY"},
				{"ESV", "es-VE"},
				{"SVF", "sv-FI"},
				{"THA", "th-TH"},
				{"TRK", "tr-TR"},
				{"UKR", "uk-UA"},
				{"URD", "ur-PK"},
				{"VIT", "vi-VN"},
				{"AZC", "az-Cyrl-AZ"},
				{"AZE", "az-Cyrl-AZ"},
				{"ENN", "en-IN"},
				{"ENM", "en-MY"},
				{"ENP", "en-PH"},
				{"FRA", "fr-FR"},
				{"FRM", "fr-MC"},
				{"FRO", "fo-FO"},
				{"FRY", "fy-NL"},
				{"GAL", "gl-ES"},
				{"GRC", "el-GR"},
				{"IBO", "ig-NG"},
				{"IND", "id-ID"},
				{"KAZ", "kk-KZ"},
				{"MNG", "mn-MN"},
				{"MSB", "ms-BN"},
				{"MSL", "ms-MY"},
				{"NON", "nn-NO"},
				{"NOR", "nb-NO"},
				{"NSO", "nso-ZA"},
				{"PTG", "pt-PT"},
				{"SRL", "sr-Latn-RS"}, 
				{"SVE", "sv-SE"},
				{"SRB", "sr-Cyrl-RS"},
				{"SWK", "sw-KE"},
				{"TKM", "tk-TM"},
				{"UZB", "es-VE"},
				{"VEN", "uz-Cyrl-UZ"},
				{"ZHM", "zh-MO"},
				{"ZUL", "zu-ZA"},
				{"ROU", "ro-RO"},
				{"WEL", "cy-GB"},
				{"RMS", "rm-CH"}
			};
		}
	}
}
