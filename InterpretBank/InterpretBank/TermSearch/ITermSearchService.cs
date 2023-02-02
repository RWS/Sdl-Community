using System.Collections.Generic;
using InterpretBank.TermSearch.Model;

namespace InterpretBank.TermSearch
{
	public interface ITermSearchService
	{
		List<StudioTermEntry> GetExactTerms(string word, string name1, string name2);
		List<StudioTermEntry> GetFuzzyTerms(string word, string sourceLanguage, string targetLanguage);
		List<Language> GetLanguages();
	}
}