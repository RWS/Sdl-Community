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
		public static EngineMappingDetails GetDetailsFromEngineUrl(string url)
		{
			var engineMappingDetails = new EngineMappingDetails();
			SplitAfterDictionary(url, engineMappingDetails);
			return engineMappingDetails;
		}

		private static void SplitAfterDictionary(string url, EngineMappingDetails engineMapping)
		{
			var dictionaryPattern = "&dictionariesIds=";
			var substrings = Regex.Split(url, dictionaryPattern);
			if (substrings.Count() >= 2)
			{
				var resourceIds = substrings[1].Split(',').ToList();
				engineMapping.ResourcesIds = resourceIds;
				GetIdAndLanguagePair(substrings[0], engineMapping);
			}
			
		}

		private static void GetIdAndLanguagePair(string url, EngineMappingDetails engineMapping)
		{
			var split = url.Split('=');
			//get  source language by spliting after "/" code
			var sourcePattern = "%2f";
			var substrings = Regex.Split(split[1], sourcePattern);

			if (substrings.Count() >= 2)
			{
				engineMapping.SourceLang = substrings[0];
				//split after ":" code
				var targetPattern = "%3a";
				var targetSplit = Regex.Split(substrings[1], targetPattern);

				if (targetSplit.Count() >= 2)
				{
					engineMapping.TargetLang = targetSplit[0];
					engineMapping.Id = targetSplit[1];
				}
				
			}
			
		}
	}
}
