using InterpretBank.GlossaryService;
using InterpretBank.GlossaryService.DAL;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.Helpers;
using InterpretBank.Model;
using InterpretBank.SettingsService.Model;
using InterpretBank.Studio.Model;
using InterpretBank.TerminologyService.Extensions;
using InterpretBank.TerminologyService.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Data.SQLite;
using System.Linq;
using System.Linq.Expressions;
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
        using var ibContext = new DataContext(InterpretBankDataContext.SqLiteConnection);
        var dbTerms =
            (glossaries != null
                ? ibContext
                    .GetTable<DbGlossaryEntry>()
                    .Where(dbTerm => glossaries.Contains(dbTerm.Tag1))
                : ibContext.GetTable<DbGlossaryEntry>()).ToList();

        var entryModels = new ObservableCollection<EntryModel>();

        try
        {
            foreach (var dbEntry in dbTerms)
            {
                var entryModel = new EntryModel
                {
                    Id = dbEntry.Id,
                    EntryComment = dbEntry.CommentAll,
                    GlossaryName = dbEntry.Tag1,
                    SubGlossaryName = dbEntry.Tag2,
                    Terms = new ObservableCollection<TermModel>()
                };

                InitializeEntryModelTerms(entryModel, dbEntry);
                entryModels.Add(entryModel);
            }
        }
        catch
        {
        }

        return entryModels;
    }

    public List<StudioTermEntry> GetExactTerms(string word, string sourceLanguage, string targetLanguage, List<string> glossaries)
    {
        var sourceLanguageIndex = GetLanguageIndex(sourceLanguage);
        var targetLanguageIndex = GetLanguageIndex(targetLanguage);
        var columns = GetTermColumns(targetLanguageIndex);

        var parameter = Expression.Parameter(typeof(DbGlossaryEntry), "term");
        var property = Expression.Property(parameter, $"Term{sourceLanguageIndex}");

        var constant = Expression.Constant(word);
        var comparison = Expression.Equal(property, constant);

        var filterExpression = Expression.Lambda<Func<DbGlossaryEntry, bool>>(comparison, parameter);
        var filteredTerms = InterpretBankDataContext
            .GetRows<DbGlossaryEntry>()
            .Where(t => glossaries.Contains(t.Tag1))
            .Where(filterExpression);

        var studioTerms = new List<StudioTermEntry>();
        foreach (var term in filteredTerms)
            //TODO: Add CommentAll as an entry level field
            studioTerms.Add(new StudioTermEntry
            {
                Id = term.Id,
                Text = term[columns[0]],
                Extra1 = term[columns[1]],
                Extra2 = term[columns[2]],
                Score = 100,
                SearchText = word
            });

        studioTerms.RemoveAll(term => string.IsNullOrEmpty(term.Text));

        return studioTerms;
    }

    public List<StudioTermEntry> GetExactTerms2(string[] words, string sourceLanguage, string targetLanguage, List<string> glossaries)
    {
        var sourceLanguageIndex = GetLanguageIndex(sourceLanguage);
        var targetLanguageIndex = GetLanguageIndex(targetLanguage);
        var columns = GetTermColumns(targetLanguageIndex);

        var parameter = Expression.Parameter(typeof(DbGlossaryEntry), "term");
        var property = Expression.Property(parameter, $"Term{sourceLanguageIndex}");

        var studioTerms = new List<StudioTermEntry>();
        foreach (var word in words)
        {
            var constant = Expression.Constant(word);
            var comparison = Expression.Equal(property, constant);

            var filterExpression = Expression.Lambda<Func<DbGlossaryEntry, bool>>(comparison, parameter);
            var filteredTerms = InterpretBankDataContext
                .GetRows<DbGlossaryEntry>()
                .Where(t => glossaries.Contains(t.Tag1))
                .Where(filterExpression);

            var localStudioTerms = new List<StudioTermEntry>();
            foreach (var term in filteredTerms)
                //TODO: Add CommentAll as an entry level field
                localStudioTerms.Add(new StudioTermEntry
                {
                    Id = term.Id,
                    Text = term[columns[0]],
                    Extra1 = term[columns[1]],
                    Extra2 = term[columns[2]],
                    Score = 100,
                    SearchText = word
                });

            localStudioTerms.RemoveAll(term => string.IsNullOrEmpty(term.Text));
            studioTerms.AddRange(localStudioTerms);
        }

        return studioTerms;
    }

    //public List<StudioTermEntry> GetFuzzyTerms(string word, string sourceLanguage, string targetLanguage, List<string> glossaries, int minScore)
    //{
    //    var sourceLanguageIndex = GetLanguageIndex(sourceLanguage);

    //    var ibContext = new DataContext(InterpretBankDataContext.SqLiteConnection);
    //    var filteredTerms = ibContext
    //        .GetTable<DbGlossaryEntry>()
    //        .Where(t => glossaries.Contains(t.Tag1))
    //        .WhereFuzzy($"Term{sourceLanguageIndex}", word, minScore);

    //    var targetLanguageIndex = GetLanguageIndex(targetLanguage);

    //    var columns = GetTermColumns(targetLanguageIndex);

    //    var studioTerms = new List<StudioTermEntry>();
    //    foreach (var term in filteredTerms)
    //        //TODO: Add CommentAll as an entry level field
    //    {
    //        studioTerms.Add(new StudioTermEntry
    //        {
    //            Id = term.Item1.Id,
    //            Text = term.Item1[columns[0]],
    //            Extra1 = term.Item1[columns[1]],
    //            Extra2 = term.Item1[columns[2]],
    //            Score = term.Item3,
    //            SearchText = term.Item2
    //        });
    //    }

    //    studioTerms.RemoveAll(term => string.IsNullOrEmpty(term.Text));

    //    return studioTerms;
    //}

    public ConcurrentDictionary<string, List<StudioTermEntry>> GetFuzzyTerms2(string[] words, string sourceLanguage,
        string targetLanguage, List<string> glossaries, int minScore)
    {
        var sourceLanguageIndex = GetLanguageIndex(sourceLanguage);
        var termsDictionary = new ConcurrentDictionary<string, List<StudioTermEntry>>();

        var filepath = InterpretBankDataContext.SqLiteConnection.FileName;
        using var connection = new SQLiteConnection($"Data Source={filepath};Pooling=True;Max Pool Size=10000;");
        using var dataContext = new MyDataContext(connection);

        dataContext.Connection.Open();

        var targetLanguageIndex = GetLanguageIndex(targetLanguage);
        var columns = GetTermColumns(targetLanguageIndex);

        var allEntries =
            dataContext
                .GetTable<DbGlossaryEntry>()
                .Where(t => glossaries.Contains(t.Tag1))
                .Select(t => new SimpleEntry
                {
                    Id = t.Id,
                    Source = t[$"Term{sourceLanguageIndex}"],
                    Target = t[columns[0]],
                    Extra1 = t[columns[1]],
                    Extra2 = t[columns[2]]
                }).ToList();

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

    //public static IEnumerable<(DbGlossaryEntry, string, int)> WhereFuzzy(List<SimpleEntry> source, string propertyName, string word)
    //{
    //    var parameter = Expression.Parameter(typeof(DbGlossaryEntry), "term");
    //    var termProperty = Expression.Property(parameter, propertyName);
    //    var convertExpression = Expression.Convert(termProperty, typeof(string));
    //    var accessProperty = Expression.Lambda<Func<DbGlossaryEntry, string>>(convertExpression, parameter);

    //    var getTermValue = accessProperty.Compile();
    //    foreach (var dbTerm in source)
    //    {
    //        var termValue = getTermValue(dbTerm);

    //        var contains = termValue.ToLower().Contains(word.ToLower());
    //        //var score = FuzzyMatch(termValue, word);

    //        /*if (score > 50)*/
    //        if (contains) yield return (dbTerm, termValue, /*score*/100);
    //    }
    //}

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

    public void SaveAllTerms(List<TermModel> changedTerms)
    {
        //InterpretBankDataContext.CommitAllChanges(changedTerms);
    }

    public void Setup(string settingsDatabaseFilepath)
    {
        InterpretBankDataContext.Setup(settingsDatabaseFilepath);
    }

    public void UpdateTerm(TermChange termChange)
    {
        InterpretBankDataContext.UpdateTerm(termChange);
    }

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

    private List<EntryModel> InitializeEntries(List<DbGlossaryEntry> items)
    {
        var entryModels = new List<EntryModel>();
        foreach (var dbEntry in items)
        {
            var entryModel = new EntryModel
            {
                Id = dbEntry.Id,
                EntryComment = dbEntry.CommentAll,
                GlossaryName = dbEntry.Tag1,
                SubGlossaryName = dbEntry.Tag2,
                Terms = []
            };

            InitializeEntryModelTerms(entryModel, dbEntry);
            entryModels.Add(entryModel);
        }

        return entryModels;
    }

    private void InitializeEntryModelTerms(EntryModel entryModel, DbGlossaryEntry t)
    {
        entryModel.Terms ??= new ObservableCollection<TermModel>();
        for (int i = 1; i <= 10; i++)
        {
            var languageName = "";
            if (i - 1 < GetLanguages().Count) languageName = GetLanguages()[i - 1].Name;

            // Image languageFlag = null;
            if (!string.IsNullOrWhiteSpace(languageName))
            {
                // languageFlag = StudioContext.GetLanguageFlag(languageName);
            }

            if (!string.IsNullOrWhiteSpace(languageName))
            {
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
}