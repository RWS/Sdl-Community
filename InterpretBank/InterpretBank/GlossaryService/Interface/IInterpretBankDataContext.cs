using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using InterpretBank.GlossaryService.DAL.Interface;
using InterpretBank.Model;
using InterpretBank.SettingsService.Model;
using InterpretBank.TermbaseViewer.Model;

namespace InterpretBank.GlossaryService.Interface;

public interface IInterpretBankDataContext : IDisposable
{
	IQueryable<T> GetRows<T>() where T : class, IInterpretBankTable;
	Table<T> GetTable<T>() where T : class, IInterpretBankTable;
	List<TagModel> GetTags();
	void InsertTag(TagModel newTag);
	void SubmitData();
	List<GlossaryModel> GetGlossaries();
	void InsertGlossary(GlossaryModel newGlossary);
	void RemoveTag(string tagName);
	List<LanguageModel> GetDbLanguages();
	List<TagLinkModel> GetLinks();
	void RemoveTagFromGlossary(string tagName, string glossaryName);
	void Setup(string filepath);
	void InsertLanguage(LanguageModel language);
	void TagGlossary(TagModel newTag, string glossaryName);
	void AddLanguageToGlossary(LanguageModel newLanguage, string selectedGlossaryGlossaryName);
	List<LanguageModel> GetGlossaryLanguages(string glossaryName);
	void CommitAllChanges(IEnumerable<TermModel> changedTerms);
}