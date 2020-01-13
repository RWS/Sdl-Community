using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.BeGlobalV4.Provider.Helpers
{
	public static class Utils
	{
		private static Constants _constants = new Constants();

		public static List<string> FormatLanguageName(string language)
		{
			try
			{
				if (!string.IsNullOrEmpty(language))
				{
					var splitedLanguage = language.Split('(');
					var languageName = splitedLanguage?[0].TrimEnd().TrimStart();
					var region = splitedLanguage.Length > 1 ? splitedLanguage[1].Replace(')', ' ').TrimEnd().TrimStart() : string.Empty;

					return new List<string> { languageName, region };
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{_constants.FormatLanguageName}: {ex.Message}\n {ex.StackTrace}");
				throw new Exception(ex.Message);
			}
			return new List<string>();
		}

		public static List<string> SplitLanguagePair(string languagePair)
		{
			try
			{
				if(!string.IsNullOrEmpty(languagePair))
				{
					var splittedLanguagePair = languagePair.Split('-');
					var sourceLangPair = splittedLanguagePair?.Count() > 1 ? splittedLanguagePair[0].TrimStart().TrimEnd() : string.Empty;
					var targetLangPair = splittedLanguagePair?.Count() > 1 ? splittedLanguagePair[1].TrimStart().TrimEnd() : string.Empty;

					return new List<string> { sourceLangPair, targetLangPair };
				}
			}
			catch(Exception ex)
			{
				Log.Logger.Error($"{_constants.SplitLanguagePair}: {ex.Message}\n {ex.StackTrace}");
				throw new Exception(ex.Message);
			}
			return new List<string>();
		}
	}
}