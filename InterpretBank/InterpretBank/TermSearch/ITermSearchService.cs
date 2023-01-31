using System.Collections.Generic;

namespace InterpretBank.TermSearch
{
	public interface ITermSearchService
	{
		List<string> GetExactTerms(string word, string name1, string name2);
		List<string> GetFuzzyTerms(string word, string sourceLanguage, string targetLanguage);
	}
}