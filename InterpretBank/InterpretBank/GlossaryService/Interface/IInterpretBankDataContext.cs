using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using InterpretBank.GlossaryService.DAL.Interface;
using InterpretBank.Model;
using InterpretBank.SettingsService.Model;

namespace InterpretBank.GlossaryService.Interface;

public interface IInterpretBankDataContext
{
	IQueryable<T> GetRows<T>() where T : class, IInterpretBankTable;
	Table<T> GetTable<T>() where T : class, IInterpretBankTable;
	List<TagModel> GetTags();
	void InsertTag(TagModel newTag);
	void SubmitData();
	List<GlossaryModel> GetGlossaries();
	void InsertGlossary(GlossaryModel newGlossary);
	void RemoveTag(string tagName);
	List<Language> GetLanguages();
	List<TagLinkModel> GetLinks();
	void RemoveTagFromGlossary(string tagName, string glossaryName);
}