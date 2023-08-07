using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using InterpretBank.GlossaryService.DAL;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.SettingsService.Model;
using InterpretBank.Studio.Model;
using InterpretBank.TermbaseViewer.Model;
using InterpretBank.TerminologyService.Extensions;
using InterpretBank.TerminologyService.Interface;

namespace InterpretBank.TerminologyService;

public class TerminologyService : ITerminologyService
{
	public TerminologyService(IInterpretBankDataContext interpretBankDataContext)
	{
		InterpretBankDataContext = interpretBankDataContext;
	}

	public IInterpretBankDataContext InterpretBankDataContext { get; }

	public void Dispose()
	{
		InterpretBankDataContext?.Dispose();
	}

	public List<TermModel> GetAllTerms(string source, string target, List<string> glossaries)
	{
		List<DbTerm> dbTerms = null;
		try
		{
			dbTerms = InterpretBankDataContext
				.GetRows<DbTerm>()
				.Where(t => glossaries.Contains(t.Tag1))
				.ToList();
		}
		catch { }

		//TODO: optimize this to use IQueryable and not .ToList()

		var sourceLanguageIndex = GetLanguageIndex(source);
		var targetLanguageIndex = GetLanguageIndex(target);
		var columns = GetTermColumns(targetLanguageIndex, sourceLanguageIndex);

		var termbaseViewerTerms = new List<TermModel>();
		dbTerms.ForEach(dbT => termbaseViewerTerms.Add(new TermModel
		(
			dbT.Id,
			dbT[columns[0]],
			dbT[columns[1]],
			dbT[columns[2]],
			dbT[columns[3]],
			dbT[columns[4]],
			dbT[columns[5]],
			dbT.CommentAll,
			sourceLanguageIndex,
			targetLanguageIndex,
			dbT.Tag1
		)));

		return termbaseViewerTerms;
	}

	public List<StudioTermEntry> GetExactTerms(string word, string sourceLanguage, string targetLanguage, List<string> glossaries)
	{
		var sourceLanguageIndex = GetLanguageIndex(sourceLanguage);
		var targetLanguageIndex = GetLanguageIndex(targetLanguage);
		var columns = GetTermColumns(targetLanguageIndex);

		var parameter = Expression.Parameter(typeof(DbTerm), "term");
		var property = Expression.Property(parameter, $"Term{sourceLanguageIndex}");

		var constant = Expression.Constant(word);
		var comparison = Expression.Equal(property, constant);

		var filterExpression = Expression.Lambda<Func<DbTerm, bool>>(comparison, parameter);
		var filteredTerms = InterpretBankDataContext
			.GetRows<DbTerm>()
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
			.GetRows<DbTerm>()
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

	public List<LanguageModel> GetGlossaryLanguages(string glossaryName) =>
		InterpretBankDataContext.GetGlossaryLanguages(glossaryName);

	public void SaveAllTerms(List<TermModel> changedTerms)
	{
		InterpretBankDataContext.CommitAllChanges(changedTerms);
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

	private int GetLanguageIndex(string language)
	{
		return GetLanguages()
			.First(lang =>
				string.Equals(lang.Name, language, StringComparison.CurrentCultureIgnoreCase)).Index;
	}

	private List<LanguageModel> GetLanguages()
	{
		var languages = InterpretBankDataContext.GetDbLanguages();
		return languages;
	}
}