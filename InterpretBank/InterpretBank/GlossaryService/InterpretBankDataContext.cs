using InterpretBank.Extensions;
using InterpretBank.GlossaryService.DAL;
using InterpretBank.GlossaryService.DAL.Interface;
using InterpretBank.GlossaryService.Model;
using InterpretBank.Helpers;
using InterpretBank.Interface;
using InterpretBank.Model;
using InterpretBank.SettingsService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Linq;
using System.Data.SQLite;
using System.Linq;

namespace InterpretBank.GlossaryService;

public class InterpretBankDataContext : IInterpretBankDataContext
{
    public event Action ShouldReloadEvent;

    public bool IsValid
    {
        get
        {
            try
            {
                var x = DataContext.GetTable<DatabaseInfo>().ToList();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

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
        var glossary = GetTable<DbGlossary>().ToList().Where(g => g.Tag1 == glossaryName).ToList()[0];

        var glossarySetting = glossary.GlossarySetting;
        var indexToReplace = glossarySetting.IndexOf("0");

        if (indexToReplace == -1) return;

        glossary.GlossarySetting = glossarySetting.Substring(0, indexToReplace) + newLanguage.Index +
                                   glossarySetting.Substring(indexToReplace + 0.ToString().Length);

        SubmitData();
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
            //var tagLinks = GetRows<DbTagLink>();
            //var currentGlossaryLinks = tagLinks.Where(tl => tl.GlossaryId == dbGlossary.Id)
            //    .Select(tm => new TagModel { TagName = tm.TagName }).ToList();

            var languages = GetLanguageNames(dbGlossary.GlossarySetting);
            glossaries.Add(new GlossaryModel
            {
                GlossaryName = dbGlossary.Tag1,
                SubGlossaryName = dbGlossary.Tag2,
                Languages = new ObservableCollection<object>(languages),
                //Tags = new ObservableCollection<TagModel>(currentGlossaryLinks),
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

    //public List<DbGlossaryEntry> GetItems(List<string> glossaries, int skipCount, int takeCount) =>
    //[
    //    .. glossaries != null
    //        ? GetRows<DbGlossaryEntry>() 
    //            .Where(dbTerm => glossaries.Contains(dbTerm.Tag1))
    //        : GetRows<DbGlossaryEntry>().Skip(skipCount).Take(takeCount)
    //];

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

    public int InsertGlossary(GlossaryModel newGlossary)
    {
        var languages = "0#0#0#0#0#0#0#0#0#0#0#0";
        var maxId = GetMaxId<DbGlossary>() + 1;

        using var sqlService = GetSqlGlossaryService();
        sqlService.Create(new GlossaryMetadataEntry
        {
            Tag1 = newGlossary.GlossaryName,
            ID = maxId,
            GlossarySetting = languages
        });

        return maxId;
    }

    public void InsertLanguage(LanguageModel language)
    {
        var dbInfo = GetTable<DatabaseInfo>().ToList()[0];
        var dbInfoProperties = dbInfo.GetType().GetProperties().Where(p => p.Name.Contains("LanguageName"));
        dbInfoProperties
            .FirstOrDefault(p => int.Parse(p.Name.Substring(12)) == language.Index)?
            .SetValue(dbInfo, language.Name);

        SubmitData();
    }

    public void InsertTag(TagModel newTag)
    {
        var maxId = GetMaxId<DbTag>() + 1;
        GetTable<DbTag>().InsertOnSubmit(new DbTag { TagName = newTag.TagName, Id = maxId });
        SubmitData();
    }

    public ActionResult<DbGlossaryEntry> InsertTerm(string source, string target, string glossaryName,
        string sourceLanguage, string targetLanguage)
    {
        var newEntry = new DbGlossaryEntry
        {
            Id = GetMaxId<DbGlossaryEntry>() + 1,
            Tag1 = glossaryName,
            [$"Term{GetLanguageIndex(sourceLanguage)}"] = source,
            [$"Term{GetLanguageIndex(targetLanguage)}"] = target
        };

        var actionResult = InsertEntity(newEntry);

        return actionResult.Success
            ? new ActionResult<DbGlossaryEntry>(true, newEntry, null)
            : new ActionResult<DbGlossaryEntry>(false, null, actionResult.Message);
    }

    public void RemoveGlossary(int glossaryId)
    {
        using var glossaryService = GetSqlGlossaryService();
        glossaryService.DeleteGlossary(glossaryId);
    }

    public List<int> RemoveTag(string tagName)
    {
        var tags = GetTable<DbTag>();
        var tagLinks = GetTable<DbTagLink>();

        var tagMarkedForRemoval = tags.Where(t => t.TagName == tagName).ToList()[0];
        var tagLinkMarkedForRemoval = tagLinks.Where(t => t.TagName == tagName).ToList();

        if (tagMarkedForRemoval is not null)
            tags.DeleteOnSubmit(tagMarkedForRemoval);
        if (tagLinkMarkedForRemoval.Any())
            tagLinks.DeleteAllOnSubmit(tagLinkMarkedForRemoval);

        SubmitData();
        var removeTag = tagLinkMarkedForRemoval.Select(tl => tl.GlossaryId).ToList();
        return removeTag;
    }

    public void RemoveTagFromGlossary(string tagName, string glossaryName)
    {
        var dbGlossaries = GetTable<DbGlossary>();
        var glossaryId = GetTableWithPendingInserts(dbGlossaries)
            .FirstOrDefault(g => g.Tag1 == glossaryName)?.Id;

        var tagLinks = GetTable<DbTagLink>();

        var tagForRemoval = tagLinks.Where(tl => tl.TagName == tagName && tl.GlossaryId == glossaryId).ToList();
        if (tagForRemoval.Any()) tagLinks.DeleteOnSubmit(tagForRemoval[0]);

        SubmitData();
    }

    public void RemoveTerm(EntryModel selectedEntry)
    {
        using var ibContext = new DataContext(SqLiteConnection);
        var dbTerms = ibContext.GetTable<DbGlossaryEntry>();

        var toRemove = dbTerms.Where(dbt => dbt.Id == selectedEntry.Id).ToList()[0];
        if (toRemove is null) return;

        dbTerms.DeleteOnSubmit(toRemove);
        ibContext.SubmitChanges();
    }

    public void Setup(string filepath = null)
    {
        if (!string.IsNullOrWhiteSpace(filepath)) SqLiteConnection = new SQLiteConnection($"Data Source={filepath}");
        DataContext = new DataContext(SqLiteConnection);

        try
        {
            DataContext.Connection.Open();
        }
        catch { }
    }

    public void SubmitData()
    {
        DataContext.SubmitChanges();
        ShouldReloadEvent?.Invoke();
    }

    public void TagGlossary(TagModel newTag, string glossaryName)
    {
        var dbGlossaries = GetTable<DbGlossary>();
        var glossaryId = dbGlossaries.Where(g => g.Tag1 == glossaryName).ToList()[0]?.Id;

        var dbTags = GetTable<DbTag>();
        var tagId = dbTags.Where(t => t.TagName == newTag.TagName).ToList()[0]?.Id;

        if (tagId is null || glossaryId is null)
            return;

        var maxId = GetMaxId<DbTagLink>() + 1;

        GetTable<DbTagLink>().InsertOnSubmit(new DbTagLink
        {
            GlossaryId = glossaryId.Value,
            TagName = newTag.TagName,
            Id = maxId
        });

        SubmitData();
    }

    public void UpdateEntry(EntryChange entryChange)
    {
        using var dataContext = GetDataContext();

        var dbTerms = dataContext.GetTable<DbGlossaryEntry>();

        DbGlossaryEntry updateTerm = null;
        foreach (var term in dbTerms)
        {
            if (term.Id == entryChange.EntryId) updateTerm = term;
        }

        if (updateTerm is null) return;
        updateTerm["CommentAll"] = entryChange.EntryComment;

        dataContext.SubmitChanges();
    }

    public void UpdateEntry(TermChange termChange)
    {
        var sqlGlossaryService = GetSqlGlossaryService();

        var entries = sqlGlossaryService.GetTerms(null, null, null, null).Cast<TermEntry>();
        var updateTerm = entries.FirstOrDefault(e => e.ID == termChange.EntryId);

        var term = updateTerm?.LanguageEquivalents.FirstOrDefault(le =>
            le.LanguageIndex == GetLanguageIndex(termChange.LanguageName));
        if (term is null) return;

        term.Term = termChange.Term;
        term.Commenta = termChange.FirstComment;
        term.Commentb = termChange.SecondComment;

        sqlGlossaryService.UpdateContent(updateTerm);
    }

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

    private MyDataContext GetDataContext()
    {
        var dataContext = new MyDataContext(SqLiteConnection.FileName);
        return dataContext;
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
        (GetTable<T>()).Select(r => r.Id).Max();

    private SqlGlossaryService GetSqlGlossaryService()
    {
        var dbConnection = new DatabaseConnection(SqLiteConnection.FileName);
        return new SqlGlossaryService(dbConnection, new SqlBuilder.SqlBuilder());
    }

    private IEnumerable<T> GetTableWithPendingInserts<T>(Table<T> table) where T : class, IInterpretBankTable =>
                                                        DataContext.GetTablePendingInserts<T>().Union(table);
}