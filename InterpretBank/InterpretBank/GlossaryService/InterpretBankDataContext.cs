using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Data.SQLite;
using System.Linq;
using InterpretBank.GlossaryService.DAL;
using InterpretBank.GlossaryService.DAL.Interface;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.Model;
using InterpretBank.SettingsService.Model;

namespace InterpretBank.GlossaryService;

public class InterpretBankDataContext : IInterpretBankDataContext
{
	public InterpretBankDataContext(SQLiteConnection sqLiteConnection)
	{
		DataContext = new DataContext(sqLiteConnection);
	}

	private DataContext DataContext { get; }

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
				Languages = languages,
				Tags = new ObservableCollection<TagModel>(currentGlossaryLinks),
				Id = dbGlossary.Id
			});
		}

		return glossaries;
	}

	public List<Language> GetLanguages()
	{
		var dbInfo = GetRows<DatabaseInfo>().ToList()[0];

		var dbInfoProperties = dbInfo.GetType().GetProperties();

		var languages = new List<Language>();
		foreach (var prop in dbInfoProperties)
			if (prop.Name.Contains("LanguageName"))
				languages.Add(new Language
				{
					Name = prop.GetValue(dbInfo).ToString(),
					Index = int.Parse(prop.Name.Substring(12))
				});

		languages.RemoveAll(l => l.Name is null or "undef");

		return languages;
	}

	public List<TagLinkModel> GetLinks() => GetRows<DbTagLink>()
		.Select(t => new TagLinkModel { GlossaryId = t.GlossaryId, TagName = t.TagName, TagId = t.TagId }).ToList();

	public void RemoveTagFromGlossary(string tagName, string glossaryName)
	{
		var glossaryId = GetTable<DbGlossary>().ToList().FirstOrDefault(g => g.Tag1 == glossaryName)?.Id;
		var tagLinks = GetTable<DbTagLink>();

		var tagForRemoval =
			tagLinks.ToList().FirstOrDefault(tl => tl.TagName == tagName && tl.GlossaryId == glossaryId);

		if (tagForRemoval is not null) tagLinks.DeleteOnSubmit(tagForRemoval);
	}

	public IQueryable<T> GetRows<T>() where T : class, IInterpretBankTable
	{
		return DataContext.GetTable<T>();
	}

	public Table<T> GetTable<T>() where T : class, IInterpretBankTable
	{
		return DataContext.GetTable<T>();
	}

	public List<TagModel> GetTags() => GetRows<DbTag>().Select(t => new TagModel { TagName = t.TagName }).ToList();

	public void InsertGlossary(GlossaryModel newGlossary)
	{
		var languages = newGlossary.Languages.Aggregate("", (current, language) => current + $"{language}#").TrimEnd('#');

		var table = GetTable<DbGlossary>();
		var maxId = table.Select(r => r.Id).Max();

		var currentDateTime = DateTime.Now.ToString();

		table.InsertOnSubmit(new DbGlossary { Tag1 = newGlossary.GlossaryName, Id = ++maxId, GlossarySetting = languages });

		//SubmitData();
	}

	public void InsertTag(TagModel newTag)
	{
		var table = GetTable<DbTag>();

		var maxId = table.Select(r => r.TagId).Max();
		table.InsertOnSubmit(new DbTag { TagName = newTag.TagName, TagId = ++maxId });

		//SubmitData();
	}

	public void RemoveTag(string tagName)
	{
		var tags = GetTable<DbTag>();
		var tagLinks = GetTable<DbTagLink>();

		var tagMarkedForRemoval = tags.ToList().FirstOrDefault(t => t.TagName == tagName);
		var tagLinkMarkedForRemoval = tagLinks.ToList().Where(t => t.TagName == tagName);

		if (tagMarkedForRemoval is not null)
			tags.DeleteOnSubmit(tagMarkedForRemoval);
		if (tagLinkMarkedForRemoval.Any())
			tagLinks.DeleteAllOnSubmit(tagLinkMarkedForRemoval);
	}

	public void SubmitData()
	{
		DataContext.SubmitChanges();
	}

	private List<Language> GetLanguageNames(string languageSetting)
	{
		var languageModels = GetLanguages();
		if (string.IsNullOrEmpty(languageSetting) || !languageSetting.Contains("#"))
			return new List<Language>();

		var langIndicesStrings = languageSetting.Split('#');

		var langIndices = new List<int>();
		if (langIndicesStrings.Length > 0)
		{
			langIndices = langIndicesStrings.Select(int.Parse).Where(ind => ind != 0).ToList();
		}

		return langIndices.Select(li => languageModels.FirstOrDefault(l => l.Index == li)).ToList();
	}
}