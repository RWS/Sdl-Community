using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Sdl.Community.ExportAnalysisReports.Model;

namespace Sdl.Community.ExportAnalysisReports.Helpers
{
	public class Help
	{
		public void CreateDirectory(string path)
		{
			Directory.CreateDirectory(path);
		}

		public Dictionary<string, bool> AddToDictionary(BindingList<LanguageDetails> languages)
		{
			var dictionaryResult = new Dictionary<string, bool>();
			if (languages != null)
			{
				foreach (var item in languages)
				{
					if (!dictionaryResult.ContainsKey(item.LanguageName))
					{
						dictionaryResult.Add(item.LanguageName, item.IsChecked);
					}
				}
			}

			return dictionaryResult;
		}

		public BindingList<LanguageDetails> AddFromDictionary(BindingList<LanguageDetails> languages, Dictionary<string, bool> languagesDictionary)
		{
			if (languagesDictionary != null && !languagesDictionary.Equals(new Dictionary<string, bool>()))
			{
				foreach (var item in languagesDictionary)
				{
					var language = new LanguageDetails {LanguageName = item.Key, IsChecked = item.Value};
					languages.Add(language);
				}
			}

			return languages;
		}
	}
}