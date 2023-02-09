using System.Collections.Generic;
using InterpretBank.Model;
using InterpretBank.Studio.Model;

namespace InterpretBank.TerminologyService.Interface
{
	public interface ITerminologyService
	{
		List<StudioTermEntry> GetExactTerms(string word, string name1, string name2);
		List<StudioTermEntry> GetFuzzyTerms(string word, string sourceLanguage, string targetLanguage);
		List<Language> GetLanguages();
	}
}