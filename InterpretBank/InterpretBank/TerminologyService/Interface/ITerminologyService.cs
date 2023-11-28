using System;
using System.Collections.Generic;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.Helpers;
using InterpretBank.Model;
using InterpretBank.SettingsService.Model;
using InterpretBank.Studio.Model;
using System.Collections.ObjectModel;

namespace InterpretBank.TerminologyService.Interface
{
	public interface ITerminologyService : IDisposable
	{
		IInterpretBankDataContext InterpretBankDataContext { get; }

        event Action ShouldReload;

        List<StudioTermEntry> GetExactTerms(string word, string name1, string name2, List<string> glossaries);

		List<StudioTermEntry> GetFuzzyTerms(string word, string sourceLanguage, string targetLanguage, List<string> glossaries);

		List<LanguageModel> GetGlossaryLanguages(string glossaryName);
		void SaveAllTerms(List<TermModel> changedTerms);
		List<string> GetTaggedGlossaries(List<string> tagList);
        int GetLanguageIndex(string interpretBankLanguage);
        void Setup(string settingsDatabaseFilepath);
        ObservableCollection<EntryModel> GetEntriesFromDb(List<string> glossaries);
        void UpdateTerm(TermChange termChange);
        ActionResult<EntryModel> AddTerm(string source, string target, string glossaryName, string sourceLanguage,
            string targetLanguage);
    }
}