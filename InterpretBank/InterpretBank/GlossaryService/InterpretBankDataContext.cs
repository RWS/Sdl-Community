using InterpretBank.Extensions;
using InterpretBank.GlossaryService.DAL;
using InterpretBank.GlossaryService.DAL.Interface;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.Helpers;
using InterpretBank.Model;
using InterpretBank.SettingsService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Data.SQLite;
using System.Linq;

namespace InterpretBank.GlossaryService;

public class InterpretBankDataContext : IInterpretBankDataContext
{
    public event Action ShouldReloadEvent;

    public SQLiteConnection SqLiteConnection { get; set; }
    private DataContext DataContext { get; set; }

    public void AddCompatibleLanguageEquivalentsFromImport(GlossaryImport glossaryImport, string glossaryName)
    {
        var dbTerms = DataContext.GetTable<DbGlossaryEntry>();

        var id = GetMaxId<DbGlossaryEntry>() + 1;

        var (compatibleLanguages, notCompatible) = CheckLanguages(glossaryImport.Languages);
        for (var entryNumber = 0; entryNumber < glossaryImport.Count - 1; entryNumber++)
        {
            var newTerm = new DbGlossaryEntry();
            foreach (var language in compatibleLanguages)
            {
                var languageIndex = GetLanguageIndex(language);
                var languageEquivalent = glossaryImport[entryNumber, language];
                newTerm[$"Term{languageIndex}"] = languageEquivalent[0];
                newTerm[$"Comment{languageIndex}a"] = languageEquivalent[1];
                newTerm[$"Comment{languageIndex}b"] = languageEquivalent[2];
            }

            newTerm.Id = id++;
            newTerm.Tag1 = glossaryName;
            newTerm["CommentAll"] = glossaryImport.GetTermComment(entryNumber);

            dbTerms.InsertOnSubmit(newTerm);
        }

        SubmitData();
    }

    public void AddLanguageToGlossary(LanguageModel newLanguage, string glossaryName)
    {
        var glossary = GetTable<DbGlossary>().ToList().FirstOrDefault(g => g.Tag1 == glossaryName)
                       ?? DataContext.GetChangeSet().Inserts.OfType<DbGlossary>()
                           .FirstOrDefault(g => g.Tag1 == glossaryName);

        var glossarySetting = glossary.GlossarySetting;
        var indexToReplace = glossarySetting.IndexOf("0");

        if (indexToReplace == -1)
            return;

        glossary.GlossarySetting = glossarySetting.Substring(0, indexToReplace) + newLanguage.Index +
                                   glossarySetting.Substring(indexToReplace + 0.ToString().Length);
    }

    public void Dispose()
    {
        DataContext?.Dispose();
        SqLiteConnection?.Dispose();
    }

    public List<LanguageModel> GetDbLanguages()
    {
        var dbInfo = GetRows<DatabaseInfo>().ToList()[0];
        var dbInfoProperties = dbInfo.GetType().GetProperties();

        var languages = dbInfoProperties
            .Where(prop => prop.Name.Contains("LanguageName"))
            .Select(prop =>
                new LanguageModel
                {
                    Name = prop.GetValue(dbInfo)?.ToString(),
                    Index = int.Parse(prop.Name.Substring(12))
                })
            .ToList();

        languages.RemoveAll(l => l.Name is null or "undef");
        return languages;
    }

    public List<GlossaryModel> GetGlossaries()
    {
        var dbGlossaries = GetRows<DbGlossary>().ToList();

        var glossaries = new List<GlossaryModel>();
        foreach (var dbGlossary in dbGlossaries)
        {
            var tagLinks = GetRows<DbTagLink>();
            var currentGlossaryLinks = tagLinks.Where(tl => tl.GlossaryId == dbGlossary.Id)
                .Select(tm => new TagModel { TagName = tm.TagName }).ToList();

            var languages = GetLanguageNames(dbGlossary.GlossarySetting);
            glossaries.Add(new GlossaryModel
            {
                GlossaryName = dbGlossary.Tag1,
                SubGlossaryName = dbGlossary.Tag2,
                Languages = new ObservableCollection<LanguageModel>(languages),
                Tags = new ObservableCollection<TagModel>(currentGlossaryLinks),
                Id = dbGlossary.Id
            });
        }

        return glossaries;
    }

    public List<LanguageModel> GetGlossaryLanguages(string glossaryName)
    {
        var dbGlossary = DataContext.GetTable<DbGlossary>().ToList().FirstOrDefault(g => g.Tag1 == glossaryName);
        var settings = dbGlossary?.GlossarySetting;

        return GetLanguageNames(settings);
    }

    //    dbTerms.DeleteAllOnSubmit(toRemove);
    //}
    public List<DbGlossaryEntry> GetItems(List<string> glossaries, int skipCount, int takeCount) =>
    [
        .. glossaries != null
            ? GetRows<DbGlossaryEntry>()
                .Where(dbTerm => glossaries.Contains(dbTerm.Tag1))
            : GetRows<DbGlossaryEntry>().Skip(skipCount).Take(takeCount)
    ];

    public List<TagLinkModel> GetLinks() => GetRows<DbTagLink>()
            .Select(t => new TagLinkModel { GlossaryId = t.GlossaryId, TagName = t.TagName, TagId = t.Id }).ToList();

    public IQueryable<T> GetRows<T>() where T : class, IInterpretBankTable => DataContext.GetTable<T>();

    public Table<T> GetTable<T>() where T : class, IInterpretBankTable => DataContext.GetTable<T>();

    public List<TagModel> GetTags() => GetRows<DbTag>().Select(t => new TagModel { TagName = t.TagName }).ToList();

    public ActionResult<int> InsertEntity<T>(T entity) where T : class, IInterpretBankTable
    {
        using var ibContext = new DataContext(SqLiteConnection);
        return ErrorHandler.WrapTryCatch(() =>
        {
            switch (entity)
            {
                case DbGlossaryEntry _:
                    ibContext.GetTable<T>().InsertOnSubmit(entity);
                    break;
            }

            ibContext.SubmitChanges();
            return entity.Id;
        });
    }

    public void InsertGlossary(GlossaryModel newGlossary)
    {
        var languages = "0#0#0#0#0#0#0#0#0#0#0#0";
        var table = GetTable<DbGlossary>();
        var maxId = GetMaxId<DbGlossary>() + 1;

        table.InsertOnSubmit(
            new DbGlossary { Tag1 = newGlossary.GlossaryName, Id = maxId, GlossarySetting = languages });
    }

    public void InsertLanguage(LanguageModel language)
    {
        var dbInfo = GetTable<DatabaseInfo>().ToList()[0];
        var dbInfoProperties = dbInfo.GetType().GetProperties().Where(p => p.Name.Contains("LanguageName"));
        dbInfoProperties
            .FirstOrDefault(p => int.Parse(p.Name.Substring(12)) == language.Index)?
            .SetValue(dbInfo, language.Name);
    }

    public void InsertTag(TagModel newTag)
    {
        var maxId = GetMaxId<DbTag>() + 1;
        GetTable<DbTag>().InsertOnSubmit(new DbTag { TagName = newTag.TagName, Id = maxId });
    }

    public ActionResult<DbGlossaryEntry> InsertTerm(string source, string target, string glossaryName,
        string sourceLanguage, string targetLanguage)
    {
        var newEntry = new DbGlossaryEntry
        {
            Id = GetMaxId<DbGlossaryEntry>() + 1,
            Tag1 = glossaryName,
            [$"Term{GetLanguageIndex(sourceLanguage)}"] = source,
            [$"Term{GetLanguageIndex(targetLanguage)}"] = target,
        };

        var actionResult = InsertEntity(newEntry);

        return actionResult.Success
            ? new ActionResult<DbGlossaryEntry>(true, newEntry, null)
            : new ActionResult<DbGlossaryEntry>(false, null, actionResult.Message);
    }

    public void RemoveGlossary(string selectedGlossaryGlossaryName)
    {
        var dbGlossaries = GetTable<DbGlossary>();
        var dbGlossariesWithPendingInserts = GetTableWithPendingInserts(dbGlossaries);

        var glossary = dbGlossariesWithPendingInserts.FirstOrDefault(g => g.Tag1 == selectedGlossaryGlossaryName);
        if (glossary is null) return;
        dbGlossaries.DeleteOnSubmit(glossary);

        var dbTerms = GetTable<DbGlossaryEntry>();
        var dbTermsWithPendingInserts = GetTableWithPendingInserts(dbTerms);

        var glossaryTerms = dbTermsWithPendingInserts.Where(t => t.Tag1 == selectedGlossaryGlossaryName);
        if (glossaryTerms.Any()) dbTerms.DeleteAllOnSubmit(glossaryTerms);

        var dbTagLinks = GetTable<DbTagLink>();
        var dbTagLinksWithPendingInserts = GetTableWithPendingInserts(dbTagLinks);

        var glossaryTags = dbTagLinksWithPendingInserts.Where(tl => tl.GlossaryId == glossary.Id);
        dbTagLinks.DeleteAllOnSubmit(glossaryTags);
    }

    public void RemoveTag(string tagName)
    {
        var tags = GetTable<DbTag>();
        var tagsWithPendingInserts = GetTableWithPendingInserts(tags);

        var tagLinks = GetTable<DbTagLink>();
        var tagLinksWithPendingInserts = GetTableWithPendingInserts(tagLinks);

        var tagMarkedForRemoval = tagsWithPendingInserts.FirstOrDefault(t => t.TagName == tagName);
        if (tagMarkedForRemoval is null)
        {
            var pendingInserts = DataContext.GetChangeSet().Inserts;
            tagMarkedForRemoval = pendingInserts.OfType<DbTag>()
                .FirstOrDefault(t => t.TagName == tagName);
        }

        var tagLinkMarkedForRemoval = tagLinksWithPendingInserts.Where(t => t.TagName == tagName);

        if (tagMarkedForRemoval is not null)
            tags.DeleteOnSubmit(tagMarkedForRemoval);
        if (tagLinkMarkedForRemoval.Any())
            tagLinks.DeleteAllOnSubmit(tagLinkMarkedForRemoval);
    }

    public void RemoveTagFromGlossary(string tagName, string glossaryName)
    {
        var dbGlossaries = GetTable<DbGlossary>();
        var glossaryId = GetTableWithPendingInserts(dbGlossaries)
            .FirstOrDefault(g => g.Tag1 == glossaryName)?.Id;

        var tagLinks = GetTable<DbTagLink>();
        var tagLinksWithPendingInserts = GetTableWithPendingInserts(tagLinks);

        var tagForRemoval =
            tagLinksWithPendingInserts.FirstOrDefault(tl => tl.TagName == tagName && tl.GlossaryId == glossaryId);

        if (tagForRemoval is not null) tagLinks.DeleteOnSubmit(tagForRemoval);
    }

    public void RemoveTerm(EntryModel selectedEntry)
    {
        using var ibContext = new DataContext(SqLiteConnection);
        var dbTerms = ibContext.GetTable<DbGlossaryEntry>();

        var terms = dbTerms.ToList();

        var toRemove = terms.FirstOrDefault(dbt => dbt.Id == selectedEntry.Id);
        if (toRemove is null) return;

        dbTerms.DeleteOnSubmit(toRemove);
        ibContext.SubmitChanges();
    }

    public void Setup(string filepath = null)
    {
        //Filepath = filepath;
        if (!string.IsNullOrWhiteSpace(filepath)) SqLiteConnection = new SQLiteConnection($"Data Source={filepath}");
        DataContext = new DataContext(SqLiteConnection);

        DataContext.Connection.Open();
    }

    //private string Filepath { get; set; }

    public void SubmitData()
    {
        DataContext.SubmitChanges();
        ShouldReloadEvent?.Invoke();
    }

    public void TagGlossary(TagModel newTag, string glossaryName)
    {
        var dbGlossaries = GetTable<DbGlossary>();
        var glossaryId = GetTableWithPendingInserts(dbGlossaries)
            .FirstOrDefault(g => g.Tag1 == glossaryName)?.Id;

        var dbTags = GetTable<DbTag>();
        var tagId = GetTableWithPendingInserts(dbTags).FirstOrDefault(t => t.TagName == newTag.TagName)?.Id;

        if (tagId is null || glossaryId is null)
            return;

        var maxId = GetMaxId<DbTagLink>() + 1;

        GetTable<DbTagLink>().InsertOnSubmit(new DbTagLink
        {
            GlossaryId = glossaryId.Value,
            TagName = newTag.TagName,
            Id = maxId
        });
    }

    public void UpdateTerm(TermChange termChange)
    {
        var languageIndex = GetLanguageIndex(termChange.LanguageName);
        var dbTermToBeUpdated = GetTable<DbGlossaryEntry>().FirstOrDefault(t => t.Id == termChange.EntryId);
        if (dbTermToBeUpdated is null) return;

        dbTermToBeUpdated[$"Term{languageIndex}"] = termChange.Term;
        dbTermToBeUpdated[$"Comment{languageIndex}a"] = termChange.FirstComment;
        dbTermToBeUpdated[$"Comment{languageIndex}b"] = termChange.SecondComment;

        SubmitData();
    }

    //public void Reset()
    //{
    //    Setup();
    //}

    //public void Refresh() => DataContext.Refresh(RefreshMode.OverwriteCurrentValues);

    //public void UpdateTerms(List<TermModel> terms)
    //{
    //    var termsIds = terms.Select(t => t.Id).ToList();
    //    var dbTerms = DataContext.GetTable<DbTerm>()
    //        .Where(t => termsIds.Contains(t.Id))
    //        .ToList();

    //    terms.ForEach(term =>
    //    {
    //        var dbTerm = dbTerms.FirstOrDefault(t => t.Id == term.Id);

    //        if (dbTerm == null) return;

    //        dbTerm[$"Term{term.SourceLanguageIndex}"] = term.SourceTerm;
    //        dbTerm[$"Comment{term.SourceLanguageIndex}a"] = term.SourceTermComment1;
    //        dbTerm[$"Comment{term.SourceLanguageIndex}b"] = term.SourceTermComment2;

    //        dbTerm[$"Term{term.TargetLanguageIndex}"] = term.TargetTerm;
    //        dbTerm[$"Comment{term.TargetLanguageIndex}a"] = term.TargetTermComment1;
    //        dbTerm[$"Comment{term.TargetLanguageIndex}b"] = term.TargetTermComment2;

    //        dbTerm["CommentAll"] = term.CommentAll;
    //    });
    //}

    //private void AddTerms(List<TermModel> newTerms)
    //{
    //    var dbTerms = DataContext.GetTable<DbTerm>();
    //    var id = GetMaxId<DbTerm>() + 1;

    //    //TODO Add full index, maybe other missing fields
    //    newTerms.ForEach(t =>
    //    {
    //        dbTerms.InsertOnSubmit(new DbTerm
    //        {
    //            Id = id++,
    //            Tag1 = t.GlossaryName,
    //            [$"Term{t.SourceLanguageIndex}"] = t.SourceTerm,
    //            [$"Comment{t.SourceLanguageIndex}a"] = t.SourceTermComment1,
    //            [$"Comment{t.SourceLanguageIndex}b"] = t.SourceTermComment2,
    //            [$"Term{t.TargetLanguageIndex}"] = t.TargetTerm,
    //            [$"Comment{t.TargetLanguageIndex}a"] = t.TargetTermComment1,
    //            [$"Comment{t.TargetLanguageIndex}b"] = t.TargetTermComment2,
    //            ["CommentAll"] = t.CommentAll,
    //        });
    //    });
    //}

    private (List<string> contained, List<string> notContained) CheckLanguages(List<string> newTerms)
    {
        var languages = GetDbLanguages().Select(l => l.Name);
        var notContained = new List<string>();
        var contained = new List<string>();

        newTerms.ForEach(l =>
        {
            if (!languages.Contains(l)) notContained.Add(l);
            else contained.Add(l);
        });

        return (contained, notContained);
    }

    private int GetLanguageIndex(string language)
    {
        var dbLanguages = GetDbLanguages();
        return dbLanguages.FirstOrDefault(dl => dl.Name == language)?.Index ?? -1;
    }

    private List<LanguageModel> GetLanguageNames(string languageSetting)
    {
        var languageModels = GetDbLanguages();

        if (string.IsNullOrEmpty(languageSetting) || !languageSetting.Contains("#"))
            return new List<LanguageModel>();

        var langIndicesStrings = languageSetting.Split('#');

        var langIndices = new List<int>();
        if (langIndicesStrings.Length > 0)
        {
            langIndices = langIndicesStrings.Select(int.Parse).Where(ind => ind != 0).ToList();
        }

        return langIndices.Select(li => languageModels.FirstOrDefault(l => l.Index == li)).ToList();
    }

    private int GetMaxId<T>() where T : class, IInterpretBankTable =>
                                                (DataContext.GetChangeSet().Inserts.OfType<T>().Union(GetTable<T>())).Select(r => r.Id).Max();

    private IEnumerable<T> GetTableWithPendingInserts<T>(Table<T> table) where T : class, IInterpretBankTable =>
                                                        DataContext.GetTablePendingInserts<T>().Union(table);

    //private void RemoveTerms(List<TermModel> removedTerms)
    //{
    //    var dbTerms = DataContext.GetTable<DbTerm>();

    //    var idsRemove = removedTerms.Select(rt => rt.Id);
    //    var toRemove = dbTerms.Where(t => idsRemove.Contains(t.Id));
}