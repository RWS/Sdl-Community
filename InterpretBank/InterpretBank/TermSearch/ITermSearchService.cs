using System.Collections.Generic;

namespace InterpretBank.TermSearch
{
	public interface ITermSearchService
	{
		List<TermEntry> GetExactTerms(string word, string name1, string name2);
		List<TermEntry> GetFuzzyTerms(string word, string sourceLanguage, string targetLanguage);
	}
}