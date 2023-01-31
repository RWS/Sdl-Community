using System;
using System.Collections.Generic;
using System.Linq;
using InterpretBank.GlossaryService;
using InterpretBank.GlossaryService.DAL;
using InterpretBank.TermSearch.Extensions;
using InterpretBank.TermSearch.Model;

namespace InterpretBank.TermSearch
{
	public class TerminologyService : ITermSearchService
	{
		public TerminologyService(InterpretBankDataContext interpretBankDataContext, ISettingsService settingsService)
		{
			InterpretBankDataContext = interpretBankDataContext;
			SettingsService = settingsService;
		}

		private InterpretBankDataContext InterpretBankDataContext { get; }
		private List<Language> Languages { get; set; }
		private ISettingsService SettingsService { get; }

		public List<StudioTermEntry> GetExactTerms(string word, string name1, string name2)
		{
			//var termEntries = GlossaryService.GetTerms(word, SettingsService.LanguageIndices, SettingsService.GlossaryNames,
			//	SettingsService.Tags);

			return null;
		}

		public List<StudioTermEntry> GetFuzzyTerms(string word, string sourceLanguage, string targetLanguage)
		{
			var sourceLanguageIndex = GetLanguageIndex(sourceLanguage);
			var targetLanguageIndex = GetLanguageIndex(targetLanguage);
			var columns = GetTargetLanguageColumns(targetLanguageIndex);

			var filteredTerms = InterpretBankDataContext.GetRows<DbTerm>().WhereFuzzy($"Term{sourceLanguageIndex}", word);

			var studioTerms = new List<StudioTermEntry>();
			foreach (var term in filteredTerms)
			{
				//TODO: Add CommentAll as an entry level field
				studioTerms.Add(new StudioTermEntry
				{
					Text = term[columns[0]],
					Extra1 = term[columns[1]],
					Extra2 = term[columns[2]]
				});
			}

			studioTerms.RemoveAll(term => string.IsNullOrEmpty(term.Text));
			return studioTerms;
		}

		//TODO: Change to use property when event is in place
		private int GetLanguageIndex(string sourceLanguage) => GetLanguages()
			.First(lang =>
				string.Equals(lang.Name, sourceLanguage, StringComparison.CurrentCultureIgnoreCase)).Index;

		public List<Language> GetLanguages()
		{
			//TODO: Update this when settings change
			//Create property that just returns the list
			//Method will be called when settings change to repopulate list
			if (Languages != null) return Languages;

			Languages = new List<Language>();

			var dbInfo = InterpretBankDataContext.GetRows<DatabaseInfo>().ToList()[0];

			var dbInfoProperties = dbInfo.GetType().GetProperties();

			foreach (var prop in dbInfoProperties)
			{
				if (prop.Name.Contains("LanguageName"))
					Languages.Add(new Language
					{
						Name = prop.GetValue(dbInfo).ToString(),
						Index = int.Parse(prop.Name.Substring(12))
					});
			}

			Languages.RemoveAll(l => l.Name is null or "undef");
			return Languages;
		}

		//var terms = DataContext.GetTable<GlossaryData>();
		//var result = terms.WherePropertyEquals("Term1", word);

		private static List<string> GetTargetLanguageColumns(int languageIndex) => new()
		{
			$"Term{languageIndex}",
			$"Comment{languageIndex}a",
			$"Comment{languageIndex}b"
		};
	}
}