using DocumentFormat.OpenXml.Spreadsheet;
using InterpretBank.GlossaryService;
using InterpretBank.GlossaryService.DAL;
using InterpretBank.GlossaryService.Model;
using InterpretBank.Helpers;
using InterpretBank.Interface;
using InterpretBank.Model;
using InterpretBank.SettingsService.Model;
using InterpretBank.Studio.Model;
using InterpretBank.TerminologyService.Extensions;
using InterpretBank.TerminologyService.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace InterpretBank.TerminologyService;

public class TerminologyService : ITerminologyService
{
    private List<LanguageModel> _languages;

    public TerminologyService(IInterpretBankDataContext interpretBankDataContext)
    {
        InterpretBankDataContext = interpretBankDataContext;
        InterpretBankDataContext.ShouldReloadEvent += () => ShouldReload?.Invoke();
    }

    public event Action ShouldReload;

    public IInterpretBankDataContext InterpretBankDataContext { get; }

    private Dictionary<string, LanguageModel> LanguageDictionary { get; set; }

    public ActionResult<EntryModel> AddTerm(string source, string target, string glossaryName, string sourceLanguage,
                string targetLanguage)
    {
        var addTermAction = InterpretBankDataContext.InsertTerm(source, target, glossaryName, sourceLanguage, targetLanguage);

        if (!addTermAction.Success) return new ActionResult<EntryModel>(false, null, addTermAction.Message);

        var entryModel = new EntryModel { Id = addTermAction.Result.Id };
        InitializeEntryModelTerms(entryModel, addTermAction.Result);
        return new ActionResult<EntryModel>(true, entryModel, null);
    }

    public void Dispose() => InterpretBankDataContext?.Dispose();

    //+ support for subglossaries
    public ObservableCollection<EntryModel> GetEntriesFromDb(List<string> glossaries)
    {
        using var glossaryService = GetGlossaryService();
        var entries = glossaryService.GetTerms(null, null, glossaries, null)?.Cast<TermEntry>();

        return GetInitializedEntries(entries);
    }

    public List<StudioTermEntry> GetExactTerms(string searchText, string sourceLanguage, string targetLanguage,
        List<string> glossaries)
    {
        var allEntries = GetSourceAndTargetTerms(sourceLanguage, targetLanguage, glossaries);
        var filteredTerms = allEntries.Where(t => t.Source.ToLower().Equals(searchText.ToLower()));

        var localStudioTerms = filteredTerms
            .Select(term => new StudioTermEntry
            {
                Id = term.Id,
                Text = term.Target,
                Extra1 = term.Extra1,
                Extra2 = term.Extra2,
                Score = 100,
                SearchText = searchText
            }).ToList();

        localStudioTerms.RemoveAll(term => string.IsNullOrEmpty(term.Text));
        return localStudioTerms;
    }

    public ConcurrentDictionary<string, List<StudioTermEntry>> GetFuzzyTerms(string[] words, string sourceLanguage,
        string targetLanguage, List<string> glossaries, int minScore)
    {
        var allEntries = GetSourceAndTargetTerms(sourceLanguage, targetLanguage, glossaries);

        var termsDictionary = new ConcurrentDictionary<string, List<StudioTermEntry>>();
        Parallel.ForEach(words, word =>
        {
            var filteredTerms = allEntries.WhereFuzzy(word, minScore, string.Join(" ", words));

            termsDictionary[word] = [];
            foreach (var term in filteredTerms)
                //TODO: Add CommentAll as an entry level field
                termsDictionary[word].Add(new StudioTermEntry
                {
                    Id = term.Item1.Id,
                    Text = term.Item1.Target,
                    Extra1 = term.Item1.Extra1,
                    Extra2 = term.Item1.Extra2,
                    Score = term.Item3,
                    SearchText = term.Item1.Source
                });

            termsDictionary[word].RemoveAll(term => string.IsNullOrEmpty(term?.Text));
        });

        return termsDictionary;
    }

    public List<GlossaryModel> GetGlossaries() => InterpretBankDataContext.GetGlossaries();

    public List<LanguageModel> GetGlossaryLanguages(string glossaryName) =>
            InterpretBankDataContext.GetGlossaryLanguages(glossaryName);

    public int GetLanguageIndex(string interpretBankLanguage)
    {
        if (LanguageDictionary is not null) return LanguageDictionary[interpretBankLanguage].Index;

        LanguageDictionary = GetLanguages().ToDictionary(l => l.Name, l => l);

        return LanguageDictionary[interpretBankLanguage].Index;
    }

    public List<LanguageModel> GetLanguages()
    {
        if (_languages is not null) return _languages;
        _languages = InterpretBankDataContext.GetDbLanguages();
        return _languages;
    }

    public List<string> GetTaggedGlossaries(List<string> tagList)
    {
        var glossaryIds = InterpretBankDataContext.GetLinks().Where(tl => tagList.Contains(tl.TagName)).Select(tl => tl.GlossaryId).ToList();
        return InterpretBankDataContext.GetRows<DbGlossary>()
            .Where(g => glossaryIds.Contains(g.Id)).Select(g => g.Tag1).ToList();
    }

    public List<TagModel> GetTags() => InterpretBankDataContext.GetTags();

    public void RemoveTerm(EntryModel selectedEntry)
    {
        InterpretBankDataContext.RemoveTerm(selectedEntry);
    }

    public void Setup(string settingsDatabaseFilepath)
    {
        _languages = null;
        InterpretBankDataContext.Setup(settingsDatabaseFilepath);
    }

    public void UpdateEntry(EntryChange entryChange) => InterpretBankDataContext.UpdateEntry(entryChange);

    public void UpdateTerm(TermChange termChange) => InterpretBankDataContext.UpdateEntry(termChange);

    private static List<string> GetTermColumns(int targetLanguageIndex, int sourceLanguageIndex = -1)
    {
        var columns = new List<string>
        {
            $"Term{targetLanguageIndex}",
            $"Comment{targetLanguageIndex}a",
            $"Comment{targetLanguageIndex}b"
        };

        if (sourceLanguageIndex > -1)
        {
            columns.Add($"Term{sourceLanguageIndex}");
            columns.Add($"Comment{sourceLanguageIndex}a");
            columns.Add($"Comment{sourceLanguageIndex}b");
        }

        return columns;
    }

    private SqlGlossaryService GetGlossaryService()
    {
        var filepath = InterpretBankDataContext.SqLiteConnection.FileName;
        var dbConnection = new DatabaseConnection(filepath);
        var glossaryService = new SqlGlossaryService(dbConnection, new SqlBuilder.SqlBuilder());
        return glossaryService;
    }

    private List<SimpleEntry> GetSourceAndTargetTerms(string sourceLanguage, string targetLanguage, List<string> glossaries)
    {
        using var glossaryService = GetGlossaryService();

        var targetLanguageIndex = GetLanguageIndex(targetLanguage);
        var sourceLanguageIndex = GetLanguageIndex(sourceLanguage);
        var entries = glossaryService.GetTerms(null, [sourceLanguageIndex, targetLanguageIndex], glossaries, null).Cast<TermEntry>();

        var allEntries = entries.Select(t =>
        {
            var sourceLe =
                t.LanguageEquivalents.FirstOrDefault(le => le.LanguageIndex == sourceLanguageIndex);
            var targetLe =
                t.LanguageEquivalents.FirstOrDefault(le => le.LanguageIndex == targetLanguageIndex);

            return new SimpleEntry
            {
                Id = t.ID,
                Source = sourceLe?.Term,
                Target = targetLe?.Term,
                Extra1 = targetLe?.Commenta,
                Extra2 = targetLe?.Commentb
            };
        }).ToList();
        return allEntries;
    }

    private ObservableCollection<EntryModel> GetInitializedEntries(IEnumerable<TermEntry> dbEntries)
    {
        var entries = new ObservableCollection<EntryModel>();
        try
        {
            foreach (var dbEntry in dbEntries)
            {
                var entryModel = new EntryModel
                {
                    Id = dbEntry.ID,
                    EntryComment = dbEntry.CommentAll,
                    GlossaryName = dbEntry.Tag1,
                    SubGlossaryName = dbEntry.Tag2,
                    Terms = []
                };

                foreach (var le in dbEntry.LanguageEquivalents)
                {
                    entryModel.Terms.Add(new TermModel
                    {
                        FirstComment = le.Commenta,
                        SecondComment = le.Commentb,
                        Term = le.Term,
                        LanguageName = GetLanguages()[le.LanguageIndex - 1].Name
                    });
                }

                entries.Add(entryModel);
            }

            return entries;
        }
        catch
        {
        }

        return [];
    }

    private void InitializeEntryModelTerms(EntryModel entryModel, DbGlossaryEntry t)
    {
        entryModel.Terms ??= [];
        for (var i = 1; i <= 10; i++)
        {
            var languageName = "";
            if (i - 1 < GetLanguages().Count) languageName = GetLanguages()[i - 1].Name;

            if (string.IsNullOrWhiteSpace(languageName)) continue;

            var termModel = new TermModel
            {
                FirstComment = t[$"Comment{i}a"],
                SecondComment = t[$"Comment{i}b"],
                Term = t[$"Term{i}"],
                LanguageName = languageName,
                //LanguageFlag = languageFlag
            };
            entryModel.Terms.Add(termModel);
        }
    }
}