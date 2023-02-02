//using System.Collections.Generic;
//using InterpretBank.GlossaryService.Interface;

//namespace InterpretBank.TermSearch
//{
//	public class TermSearchService : ITermSearchService
//	{
//		public TermSearchService(IGlossaryService glossaryService, ISettingsService settingsService)
//		{
//			GlossaryService = glossaryService;
//			SettingsService = settingsService;
//		}

//		private IGlossaryService GlossaryService { get; }
//		private ISettingsService SettingsService { get; }

//		public List<TermEntry> GetExactTerms(string word, string name1, string name2)
//		{
//			var termEntries = GlossaryService.GetTerms(word, SettingsService.LanguageIndices, SettingsService.GlossaryNames,
//				SettingsService.Tags);

//			return null;
//		}

//		public List<TermEntry> GetFuzzyTerms(string word, string sourceLanguage, string targetLanguage)
//		{
//			var termEntries = GlossaryService.GetTerms(word, SettingsService.LanguageIndices, SettingsService.GlossaryNames,
//				SettingsService.Tags);

//			return null;
//		}
//	}
//}