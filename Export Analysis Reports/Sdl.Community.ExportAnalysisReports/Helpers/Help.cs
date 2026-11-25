using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Sdl.Community.ExportAnalysisReports.Model;

namespace Sdl.Community.ExportAnalysisReports.Helpers
{
	public class Help
	{
		public Dictionary<string, bool> AddToDictionary(BindingList<LanguageDetails> languages)
		{
			// transform the languages into dictionary (used to deselect the single file projects languages)
			return languages.GroupBy(l => l.LanguageName).ToDictionary(lg => lg.Key, lg => lg.First().IsChecked);
		}
	}
}