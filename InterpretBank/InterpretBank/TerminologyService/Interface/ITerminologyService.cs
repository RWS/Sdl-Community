using InterpretBank.GlossaryService.Interface;
using InterpretBank.Helpers;
using InterpretBank.Model;
using InterpretBank.SettingsService.Model;
using InterpretBank.Studio.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace InterpretBank.TerminologyService.Interface
{
    public interface ITerminologyService : IDisposable
    {
        event Action ShouldReload;

        IInterpretBankDataContext InterpretBankDataContext { get; }

        ActionResult<EntryModel> AddTerm(string source, string target, string glossaryName, string sourceLanguage,
            string targetLanguage);

        ObservableCollection<EntryModel> GetEntriesFromDb(List<string> glossaries);

        List<StudioTermEntry> GetExactTerms(string word, string name1, string name2, List<string> glossaries);

        List<StudioTermEntry> GetFuzzyTerms(string word, string sourceLanguage, string targetLanguage, List<string> glossaries);

        List<GlossaryModel> GetGlossaries();

        List<LanguageModel> GetGlossaryLanguages(string glossaryName);

        int GetLanguageIndex(string interpretBankLanguage);

        public List<LanguageModel> GetLanguages();

        List<string> GetTaggedGlossaries(List<string> tagList);

        List<TagModel> GetTags();


        void SaveAllTerms(List<TermModel> changedTerms);

        void Setup(string settingsDatabaseFilepath);

        void UpdateTerm(TermChange termChange);
        void RemoveTerm(EntryModel selectedEntry);
    }
}