using InterpretBank.GlossaryService.DAL.Interface;
using InterpretBank.Model;
using InterpretBank.SettingsService.Model;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace InterpretBank.GlossaryService.Interface;

public interface IInterpretBankDataContext : IDisposable
{
    event Action ShouldReloadEvent;

    void AddCompatibleLanguageEquivalentsFromImport(GlossaryImport glossaryImport, string glossaryName);

    void AddLanguageToGlossary(LanguageModel newLanguage, string selectedGlossaryGlossaryName);

    //void CommitAllChanges(List<TermModel> changedTerms);

    List<LanguageModel> GetDbLanguages();

    List<GlossaryModel> GetGlossaries();

    List<LanguageModel> GetGlossaryLanguages(string glossaryName);

    List<TagLinkModel> GetLinks();

    IQueryable<T> GetRows<T>() where T : class, IInterpretBankTable;

    Table<T> GetTable<T>() where T : class, IInterpretBankTable;

    List<TagModel> GetTags();

    void InsertGlossary(GlossaryModel newGlossary);

    void InsertLanguage(LanguageModel language);

    void InsertTag(TagModel newTag);

    void RemoveGlossary(string selectedGlossaryGlossaryName);

    void RemoveTag(string tagName);

    void RemoveTagFromGlossary(string tagName, string glossaryName);

    void Setup(string filepath);

    void SubmitData();

    void TagGlossary(TagModel newTag, string glossaryName);
    void UpdateTerm(TermChange termChange);
}