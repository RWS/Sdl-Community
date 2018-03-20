using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sdl.Community.AdaptiveMT.Service.Model;

namespace Sdl.Community.AdaptiveMT.Service.Helpers
{
	public static class EngineDetails
	{
		/// <summary>
		/// Part of url has following form: en-US%2fde-DE%3a5a3b9b630cf26707d2cf1863
		/// The characters are not decoded by helper method
		/// Url decoded has following for : en-US/de-DE:5a3b9b630cf26707d2cf1863
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static List<EngineMappingDetails> GetDetailsFromEngineUrl(string url)
		{
			var providerLanguages = new List<EngineMappingDetails>();
			//remove first part of the url
			var urlWithoutBegining = url.Replace("https://lc-api.sdl.com/?languagePairEngineMapping=", string.Empty);
			var dictionaryPattern = "&dictionariesIds=";

			//split the url after dictionariesId
			var substrings = Regex.Split(urlWithoutBegining, dictionaryPattern);
			if (substrings.Any())
			{
				if (substrings[0].Contains(";"))
				{
					var sourcePattern = ";";
					var languagesSplit = Regex.Split(substrings[0], sourcePattern);

					foreach (var language in languagesSplit)
					{
						var providerLanguage = GetLanguageDetails(language);
						providerLanguages.Add(providerLanguage);
					}
				}
				else
				{
					providerLanguages.Add(GetLanguageDetails(substrings[0]));
				}
			}
			return providerLanguages;
		}

		public static EngineMappingDetails GetLanguageDetails(string language)
		{
			var slashPosition = language.IndexOf(@"/", StringComparison.Ordinal);
			var pointsPosition = language.IndexOf(@":", StringComparison.Ordinal);
			var providerLanguage = new EngineMappingDetails
			{
				TargetLang = language.Substring(slashPosition + 1, pointsPosition - slashPosition - 1),
				Id = language.Substring(pointsPosition + 1)
			};
			return providerLanguage;
		}
	}
}
