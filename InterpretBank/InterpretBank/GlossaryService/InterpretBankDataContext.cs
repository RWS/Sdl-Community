using InterpretBank.GlossaryService.DAL;
using InterpretBank.GlossaryService.DAL.Interface;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.Model;
using InterpretBank.SettingsService.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Data.SQLite;
using System.Linq;

namespace InterpretBank.GlossaryService;

public class InterpretBankDataContext : IInterpretBankDataContext
{
    public SQLiteConnection SqLiteConnection { get; set; }
    private DataContext DataContext { get; set; }

    public void AddCompatibleLanguageEquivalentsFromImport(GlossaryImport glossaryImport, string glossaryName)
    {
        var dbTerms = DataContext.GetTable<DbTerm>();

        var id = GetMaxId<DbTerm>() + 1;

        var (compatibleLanguages, notCompatible) = CheckLanguages(glossaryImport.Languages);
        for (var entryNumber = 0; entryNumber < glossaryImport.Count - 1; entryNumber++)
        {
            var newTerm = new DbTerm();
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

    public void CommitAllChanges(List<TermModel> changedTerms)
    {
        var addedTerms = changedTerms.Where(t => t.Id == -1).ToList();
        var removedTerms = changedTerms.Where(t => t.IsRemoved).ToList();
        var updatedTerms = changedTerms.Except(addedTerms).Except(removedTerms).ToList();

        AddTerms(addedTerms);
        UpdateTerms(updatedTerms);
        RemoveTerms(removedTerms);

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
                    Name = prop.GetValue(dbInfo).ToString(),
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
            var currentGlossaryLinks = tagLinks.Where(tl => tl.Id == dbGlossary.Id)
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

    public List<TagLinkModel> GetLinks() => GetRows<DbTagLink>()
        .Select(t => new TagLinkModel { GlossaryId = t.Id, TagName = t.TagName, TagId = t.TagId }).ToList();

    public IQueryable<T> GetRows<T>() where T : class, IInterpretBankTable => DataContext.GetTable<T>();

    public Table<T> GetTable<T>() where T : class, IInterpretBankTable => DataContext.GetTable<T>();

    public List<TagModel> GetTags() => GetRows<DbTag>().Select(t => new TagModel { TagName = t.TagName }).ToList();

    private int GetMaxId<T>() where T : class, IInterpretBankTable =>
        (DataContext.GetChangeSet().Inserts.OfType<T>().Union(GetTable<T>())).Select(r => r.Id).Max();

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
        var table = GetTable<DbTag>();

        var maxId = GetMaxId<DbTag>();
        table.InsertOnSubmit(new DbTag { TagName = newTag.TagName, Id = ++maxId });
    }

    public void RemoveTag(string tagName)
    {
        var tags = GetTable<DbTag>();
        var tagLinks = GetTable<DbTagLink>();


        var tagMarkedForRemoval = tags.ToList().FirstOrDefault(t => t.TagName == tagName);

        if (tagMarkedForRemoval is null)
        {
            var pendingInserts = DataContext.GetChangeSet().Inserts;
            tagMarkedForRemoval = pendingInserts.OfType<DbTag>()
                .FirstOrDefault(t => t.TagName == tagName);
        }

        var tagLinkMarkedForRemoval = tagLinks.ToList().Where(t => t.TagName == tagName);

        if (tagMarkedForRemoval is not null)
            tags.DeleteOnSubmit(tagMarkedForRemoval);
        if (tagLinkMarkedForRemoval.Any())
            tagLinks.DeleteAllOnSubmit(tagLinkMarkedForRemoval);
    }

    public void RemoveTagFromGlossary(string tagName, string glossaryName)
    {
        var glossaryId = GetTable<DbGlossary>().ToList().FirstOrDefault(g => g.Tag1 == glossaryName)?.Id;
        var tagLinks = GetTable<DbTagLink>();

        var tagForRemoval =
            tagLinks.ToList().FirstOrDefault(tl => tl.TagName == tagName && tl.Id == glossaryId);

        if (tagForRemoval is not null)
            tagLinks.DeleteOnSubmit(tagForRemoval);
    }

    public void Setup(string filepath)
    {
        SqLiteConnection = new SQLiteConnection($"Data Source={filepath}");
        DataContext = new DataContext(SqLiteConnection);
    }

    public void SubmitData() => DataContext.SubmitChanges();

    public void TagGlossary(TagModel newTag, string glossaryName)
    {
        var glossaryId = GetTable<DbGlossary>().ToList().FirstOrDefault(g => g.Tag1 == glossaryName)?.Id;
        var tagId = GetTable<DbTag>().ToList().FirstOrDefault(t => t.TagName == newTag.TagName)?.Id;

        var dbTagLinks = GetTable<DbTagLink>();
        var tagLinks = DataContext.GetChangeSet().Inserts.OfType<DbTagLink>().Union(dbTagLinks);

        if (tagId is null || glossaryId is null)
            return;

        var maxId = GetMaxId<DbTagLink>();
        dbTagLinks.InsertOnSubmit(new DbTagLink
        {
            Id = glossaryId.Value,
            TagName = newTag.TagName,
            TagId = ++maxId
        });
    }

    public void UpdateTerms(List<TermModel> terms)
    {
        var termsIds = terms.Select(t => t.Id).ToList();
        var dbTerms = DataContext.GetTable<DbTerm>()
            .Where(t => termsIds.Contains(t.Id))
            .ToList();

        terms.ForEach(term =>
        {
            var dbTerm = dbTerms.FirstOrDefault(t => t.Id == term.Id);

            if (dbTerm == null) return;

            dbTerm[$"Term{term.SourceLanguageIndex}"] = term.SourceTerm;
            dbTerm[$"Comment{term.SourceLanguageIndex}a"] = term.SourceTermComment1;
            dbTerm[$"Comment{term.SourceLanguageIndex}b"] = term.SourceTermComment2;

            dbTerm[$"Term{term.TargetLanguageIndex}"] = term.TargetTerm;
            dbTerm[$"Comment{term.TargetLanguageIndex}a"] = term.TargetTermComment1;
            dbTerm[$"Comment{term.TargetLanguageIndex}b"] = term.TargetTermComment2;

            dbTerm["CommentAll"] = term.CommentAll;
        });
    }

    private void AddTerms(List<TermModel> newTerms)
    {
        var dbTerms = DataContext.GetTable<DbTerm>();
        var id = GetMaxId<DbTerm>() + 1;

        //TODO Add full index, maybe other missing fields
        newTerms.ForEach(t =>
        {
            dbTerms.InsertOnSubmit(new DbTerm
            {
                Id = id++,
                Tag1 = t.GlossaryName,
                [$"Term{t.SourceLanguageIndex}"] = t.SourceTerm,
                [$"Comment{t.SourceLanguageIndex}a"] = t.SourceTermComment1,
                [$"Comment{t.SourceLanguageIndex}b"] = t.SourceTermComment2,
                [$"Term{t.TargetLanguageIndex}"] = t.TargetTerm,
                [$"Comment{t.TargetLanguageIndex}a"] = t.TargetTermComment1,
                [$"Comment{t.TargetLanguageIndex}b"] = t.TargetTermComment2,
                ["CommentAll"] = t.CommentAll,
            });
        });
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

    private void RemoveTerms(List<TermModel> removedTerms)
    {
        var dbTerms = DataContext.GetTable<DbTerm>();

        var idsRemove = removedTerms.Select(rt => rt.Id);
        var toRemove = dbTerms.Where(t => idsRemove.Contains(t.Id));

        dbTerms.DeleteAllOnSubmit(toRemove);
    }
}