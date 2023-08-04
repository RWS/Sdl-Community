using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using InterpretBank.Constants;
using InterpretBank.Extensions;
using InterpretBank.GlossaryService.DAL;
using InterpretBank.GlossaryService.DAL.Interface;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.Model;
using InterpretBank.SettingsService.Model;
using InterpretBank.TermbaseViewer.Model;

namespace InterpretBank.GlossaryService;

public class InterpretBankDataContext : IInterpretBankDataContext
{
	public SQLiteConnection SqLiteConnection { get; set; }


	//TODO: Delete this; only used for debugging
	public int Id { get; set; }

	private DataContext DataContext { get; set; }


	public InterpretBankDataContext()
	{
		Id = new Random().Next(100);
	}

	public void Dispose()
	{
		DataContext?.Dispose();
		SqLiteConnection?.Dispose();
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

	public void InsertLanguage(LanguageModel language)
	{
		var dbInfo = GetTable<DatabaseInfo>().ToList()[0];
		var dbInfoProperties = dbInfo.GetType().GetProperties().Where(p => p.Name.Contains("LanguageName"));
		dbInfoProperties
			.FirstOrDefault(p => int.Parse(p.Name.Substring(12)) == language.Index)?
			.SetValue(dbInfo, language.Name);
	}

	public void TagGlossary(TagModel newTag, string glossaryName)
	{
		var glossaryId = GetTable<DbGlossary>().ToList().FirstOrDefault(g => g.Tag1 == glossaryName)?.Id;
		var tagId = GetTable<DbTag>().ToList().FirstOrDefault(t => t.TagName == newTag.TagName)?.TagId;
		var tagLinks = GetTable<DbTagLink>();

		if (tagId is null || glossaryId is null)
			return;

		var maxId = tagLinks.Select(tl => tl.TagId).Max();
		tagLinks.InsertOnSubmit(new DbTagLink
		{
			GlossaryId = glossaryId.Value,
			TagName = newTag.TagName,
			TagId = ++maxId
		});
	}

	public void AddLanguageToGlossary(LanguageModel newLanguage, string glossaryName)
	{
		var glossary = GetTable<DbGlossary>().ToList().FirstOrDefault(g => g.Tag1 == glossaryName);
		var glossarySetting = glossary.GlossarySetting;
		var indexToReplace = glossarySetting.IndexOf("0");

		if (indexToReplace == -1)
			return;

		glossary.GlossarySetting = glossarySetting.Substring(0, indexToReplace) + newLanguage.Index +
		                           glossarySetting.Substring(indexToReplace + 0.ToString().Length);
	}

	public List<LanguageModel> GetGlossaryLanguages(string glossaryName)
	{
		var dbGlossary = DataContext.GetTable<DbGlossary>().ToList().FirstOrDefault(g => g.Tag1 == glossaryName);
		var settings = dbGlossary.GlossarySetting;

		return GetLanguageNames(settings);
	}

	public void CommitAllChanges(IEnumerable<TermModel> changedTerms)
	{
		var addedTerms = changedTerms.Where(t => t.Id == -1).ToList();
		var updatedTerms = changedTerms.Except(addedTerms).ToList();

		AddTerms(addedTerms);
		UpdateTerms(updatedTerms);

		SubmitData();
	}

	private void AddTerms(List<TermModel> newTerms)
	{
		var dbTerms = DataContext.GetTable<DbTerm>();

		newTerms.ForEach(t => dbTerms.InsertOnSubmit(new DbTerm
		{
			[$"Term{t.SourceLanguageIndex}"] = t.SourceTerm,
			[$"Comment{t.SourceLanguageIndex}a"] = t.SourceTermComment1,
			[$"Comment{t.SourceLanguageIndex}b"] = t.SourceTermComment2,

			[$"Term{t.TargetLanguageIndex}"] = t.TargetTerm,
			[$"Comment{t.TargetLanguageIndex}a"] = t.TargetTermComment1,
			[$"Comment{t.TargetLanguageIndex}b"] = t.TargetTermComment2,

			["CommentAll"] = t.CommentAll,
		}));
	}

	public void UpdateTerms(List<TermModel> terms)
	{
		var termsIds = terms.Select(t => t.Id).ToList();
		var dbTerms = DataContext.GetTable<DbTerm>().ToList().Where(t => termsIds.Contains(t.Id));

		foreach (var term in terms)
		{
			var dbTerm = dbTerms.FirstOrDefault(t => t.Id == term.Id);

			dbTerm[$"Term{term.SourceLanguageIndex}"] = term.SourceTerm;
			dbTerm[$"Comment{term.SourceLanguageIndex}a"] = term.SourceTermComment1;
			dbTerm[$"Comment{term.SourceLanguageIndex}b"] = term.SourceTermComment2;

			dbTerm[$"Term{term.TargetLanguageIndex}"] = term.TargetTerm;
			dbTerm[$"Comment{term.TargetLanguageIndex}a"] = term.TargetTermComment1;
			dbTerm[$"Comment{term.TargetLanguageIndex}b"] = term.TargetTermComment2;

			dbTerm["CommentAll"] = term.CommentAll;
		}
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

	public List<TagLinkModel> GetLinks() => GetRows<DbTagLink>()
		.Select(t => new TagLinkModel { GlossaryId = t.GlossaryId, TagName = t.TagName, TagId = t.TagId }).ToList();

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
	}

	public void InsertTag(TagModel newTag)
	{
		var table = GetTable<DbTag>();

		var maxId = table.Select(r => r.TagId).Max();
		table.InsertOnSubmit(new DbTag { TagName = newTag.TagName, TagId = ++maxId });


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

	public void RemoveTagFromGlossary(string tagName, string glossaryName)
	{
		var glossaryId = GetTable<DbGlossary>().ToList().FirstOrDefault(g => g.Tag1 == glossaryName)?.Id;
		var tagLinks = GetTable<DbTagLink>();

		var tagForRemoval =
			tagLinks.ToList().FirstOrDefault(tl => tl.TagName == tagName && tl.GlossaryId == glossaryId);

		if (tagForRemoval is not null)
			tagLinks.DeleteOnSubmit(tagForRemoval);
	}

	public void Setup(string filepath)
	{
		SqLiteConnection = new SQLiteConnection($"Data Source={filepath}");
		DataContext = new DataContext(SqLiteConnection);
	}

	public void SubmitData()
	{
		DataContext.SubmitChanges();
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
}