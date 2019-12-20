using System;
using System.Collections.Generic;

namespace Sdl.Community.BeGlobalV4.Provider.Helpers
{
	public static class Utils
	{
		public static List<string> FormatLanguageName(string language)
		{
			try
			{
				if (!string.IsNullOrEmpty(language))
				{
					var splitedLanguage = language.Split('(');
					var languageName = splitedLanguage[0].TrimEnd().TrimStart();
					var region = splitedLanguage.Length > 1 ? splitedLanguage[1].Replace(')', ' ').TrimEnd().TrimStart() : string.Empty;

					return new List<string> { languageName, region };
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.FormatLanguageName}: {ex.Message}\n {ex.StackTrace}");
				throw new Exception(ex.Message);
			}
			return new List<string>();
		}
	}
}