using InterpretBank.GlossaryService.DAL;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.Helpers;
using InterpretBank.Model;
using InterpretBank.SettingsService.Model;
using InterpretBank.Studio;
using InterpretBank.Studio.Model;
using InterpretBank.TerminologyService.Extensions;
using InterpretBank.TerminologyService.Interface;
using Sdl.Core.Globalization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;

namespace InterpretBank.TerminologyService;

public class TerminologyService : ITerminologyService
{
    public TerminologyService(IInterpretBankDataContext interpretBankDataContext)
    {
        InterpretBankDataContext = interpretBankDataContext;
        InterpretBankDataContext.ShouldReloadEvent += () => ShouldReload?.Invoke();
    }

    public event Action ShouldReload;

    public IInterpretBankDataContext InterpretBankDataContext { get; }

    public ActionResult<EntryModel> AddTerm(string source, string target, string glossaryName, string sourceLanguage,
        string targetLanguage)
    {
        var entryModel = new EntryModel();
        var addTermAction = InterpretBankDataContext.InsertTerm(source, target, glossaryName, sourceLanguage, targetLanguage);

        if (!addTermAction.Success) return new ActionResult<EntryModel>(false, null, addTermAction.Message);

        var dbLanguages = GetLanguages();
        var studioLanguages = StudioContext.Languages.ToList();

        InitializeEntryModelTerms(dbLanguages, studioLanguages, entryModel, addTermAction.Result);
        return new ActionResult<EntryModel>(true, entryModel, null);
    }

    public void Dispose()
    {
        InterpretBankDataContext?.Dispose();
    }

    //+ support for subglossaries
    public ObservableCollection<EntryModel> GetEntriesFromDb(List<string> glossaries)
    {
        var dbTerms =
            glossaries != null
                ? InterpretBankDataContext
                    .GetRows<DbGlossaryEntry>()
                    .Where(dbTerm => glossaries.Contains(dbTerm.Tag1))
                : InterpretBankDataContext.GetRows<DbGlossaryEntry>();

        var entryModels = new ObservableCollection<EntryModel>();

        var dbLanguages = GetLanguages();
        var studioLanguages = StudioContext.Languages.ToList();

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

            InitializeEntryModelTerms(dbLanguages, studioLanguages, entryModel, dbEntry);
            entryModels.Add(entryModel);
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

    public List<StudioTermEntry> GetFuzzyTerms(string word, string sourceLanguage, string targetLanguage, List<string> glossaries)
    {
        var sourceLanguageIndex = GetLanguageIndex(sourceLanguage);

        var filteredTerms = InterpretBankDataContext
            .GetRows<DbGlossaryEntry>()
            .Where(t => glossaries.Contains(t.Tag1))
            .WhereFuzzy($"Term{sourceLanguageIndex}", word);

        var targetLanguageIndex = GetLanguageIndex(targetLanguage);

        var columns = GetTermColumns(targetLanguageIndex);

        var studioTerms = new List<StudioTermEntry>();
        foreach (var term in filteredTerms)
            //TODO: Add CommentAll as an entry level field
            studioTerms.Add(new StudioTermEntry
            {
                Id = term.Item1.Id,
                Text = term.Item1[columns[0]],
                Extra1 = term.Item1[columns[1]],
                Extra2 = term.Item1[columns[2]],
                Score = term.Item3,
                SearchText = term.Item2
            });

        studioTerms.RemoveAll(term => string.IsNullOrEmpty(term.Text));

        return studioTerms;
    }

    public List<GlossaryModel> GetGlossaries() => InterpretBankDataContext.GetGlossaries();

    public List<LanguageModel> GetGlossaryLanguages(string glossaryName) =>
            InterpretBankDataContext.GetGlossaryLanguages(glossaryName);

    public int GetLanguageIndex(string interpretBankLanguage) =>
        GetLanguages()
            .First(lang =>
                string.Equals(lang.Name, interpretBankLanguage, StringComparison.CurrentCultureIgnoreCase)).Index;

    public List<LanguageModel> GetLanguages()
    {
        var languages = InterpretBankDataContext.GetDbLanguages();
        return languages;
    }

    public List<string> GetTaggedGlossaries(List<string> tagList)
    {
        var glossaryIds = InterpretBankDataContext.GetLinks().Where(tl => tagList.Contains(tl.TagName)).Select(tl => tl.GlossaryId).ToList();
        return InterpretBankDataContext.GetRows<DbGlossary>()
            .Where(g => glossaryIds.Contains(g.Id)).Select(g => g.Tag1).ToList();
    }

    public List<TagModel> GetTags() => InterpretBankDataContext.GetTags();

   

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

    public void RemoveTerm(EntryModel selectedEntry)
    {
        InterpretBankDataContext.RemoveTerm(selectedEntry);
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

    private void InitializeEntryModelTerms(List<LanguageModel> dbLanguages, List<Language> studioLanguages, EntryModel entryModel, DbGlossaryEntry t)
    {
        entryModel.Terms ??= new ObservableCollection<TermModel>();
        for (int i = 1; i <= 10; i++)
        {
            var languageName = "";
            if (i - 1 < dbLanguages.Count) languageName = dbLanguages[i - 1].Name;

            Image languageFlag = null;
            if (!string.IsNullOrWhiteSpace(languageName))
                languageFlag = studioLanguages.FirstOrDefault(s => s.EnglishName.Contains(languageName) && !s.IsNeutral)
                    ?.GetFlagImage();

            if (!string.IsNullOrWhiteSpace(languageName))
                entryModel.Terms.Add(new TermModel
                {
                    FirstComment = t[$"Comment{i}a"],
                    SecondComment = t[$"Comment{i}b"],
                    Term = t[$"Term{i}"],
                    LanguageName = languageName,
                    LanguageFlag = languageFlag
                });
        }
    }
}