using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

		public void AddFromDictionary(BindingList<LanguageDetails> languages, Dictionary<string, bool> languagesDictionary)
		{
			foreach (var item in languagesDictionary)
			{
				var language = new LanguageDetails { LanguageName = item.Key, IsChecked = item.Value };
				languages.Add(language);
			}
		}
	}
}