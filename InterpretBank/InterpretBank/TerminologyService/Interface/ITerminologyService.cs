using InterpretBank.GlossaryService.Interface;
using InterpretBank.Helpers;
using InterpretBank.Model;
using InterpretBank.SettingsService.Model;
using InterpretBank.Studio.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace InterpretBank.TerminologyService.Interface
{
    public interface ITerminologyService : IDisposable
    {
        event Action ShouldReload;

        IInterpretBankDataContext InterpretBankDataContext { get; }

        ActionResult<EntryModel> AddTerm(string source, string target, string glossaryName, string sourceLanguage,
            string targetLanguage);

        public ObservableCollection<EntryModel> GetEntriesFromDb(List<string> glossaries);


        List<StudioTermEntry> GetExactTerms(string word, string name1, string name2, List<string> glossaries);

        public List<StudioTermEntry> GetExactTerms2(string[] word, string sourceLanguage, string targetLanguage,
            List<string> glossaries);

        //List<StudioTermEntry> GetFuzzyTerms(string word, string sourceLanguage, string targetLanguage,
        //    List<string> glossaries, int minScore);

        List<GlossaryModel> GetGlossaries();

        List<LanguageModel> GetGlossaryLanguages(string glossaryName);

        int GetLanguageIndex(string interpretBankLanguage);

        public List<LanguageModel> GetLanguages();

        List<string> GetTaggedGlossaries(List<string> tagList);

        List<TagModel> GetTags();

        void RemoveTerm(EntryModel selectedEntry);

        void SaveAllTerms(List<TermModel> changedTerms);

        void Setup(string settingsDatabaseFilepath);

        void UpdateTerm(TermChange termChange);

        //IItemsProvider<EntryModel> GetEntriesProvider(List<string> glossaries, string sourceLanguageName);
        public ConcurrentDictionary<string, List<StudioTermEntry>> GetFuzzyTerms2(string[] words, string sourceLanguage,
            string targetLanguage, List<string> glossaries, int minScore);
    }
}