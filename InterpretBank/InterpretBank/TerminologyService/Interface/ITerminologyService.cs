using System;
using System.Collections.Generic;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.SettingsService.Model;
using InterpretBank.Studio.Model;
using InterpretBank.TermbaseViewer.Model;

namespace InterpretBank.TerminologyService.Interface
{
	public interface ITerminologyService : IDisposable
	{
		IInterpretBankDataContext InterpretBankDataContext { get; }

		List<StudioTermEntry> GetExactTerms(string word, string name1, string name2, List<string> glossaries);

		List<StudioTermEntry> GetFuzzyTerms(string word, string sourceLanguage, string targetLanguage, List<string> glossaries);

		List<LanguageModel> GetGlossaryLanguages(string glossaryName);
		List<TermModel> GetAllTerms(string source, string target, List<string> glossaries);
		void SaveAllTerms(List<TermModel> changedTerms);
		List<string> GetTaggedGlossaries(List<string> tagList);
	}
}