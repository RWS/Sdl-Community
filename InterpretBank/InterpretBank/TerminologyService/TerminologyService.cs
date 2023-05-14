using System;
using System.Collections.Generic;
using System.Linq;
using InterpretBank.GlossaryService;
using InterpretBank.GlossaryService.DAL;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.Model;
using InterpretBank.Studio.Model;
using InterpretBank.TerminologyService.Extensions;
using InterpretBank.TerminologyService.Interface;

namespace InterpretBank.TerminologyService;

public class TerminologyService : ITerminologyService
{
	public TerminologyService(IInterpretBankDataContext interpretBankDataContext)
	{
		InterpretBankDataContext = interpretBankDataContext;
	}

	private IInterpretBankDataContext InterpretBankDataContext { get; }

	public List<StudioTermEntry> GetExactTerms(string word, string name1, string name2)
	{
		//var termEntries = GlossaryService.GetTerms(word, SettingsService.LanguageIndices, SettingsService.GlossaryNames,
		//	SettingsService.Tags);

		return null;
	}

	public List<StudioTermEntry> GetFuzzyTerms(string word, string sourceLanguage, string targetLanguage)
	{
		var sourceLanguageIndex = GetLanguageIndex(sourceLanguage);
		var targetLanguageIndex = GetLanguageIndex(targetLanguage);
		var columns = GetTargetLanguageColumns(targetLanguageIndex);

		var filteredTerms = InterpretBankDataContext.GetRows<DbTerm>().WhereFuzzy($"Term{sourceLanguageIndex}", word);

		var studioTerms = new List<StudioTermEntry>();
		foreach (var term in filteredTerms)
			//TODO: Add CommentAll as an entry level field
			studioTerms.Add(new StudioTermEntry
			{
				Text = term.Item1[columns[0]], Extra1 = term.Item1[columns[1]], Extra2 = term.Item1[columns[2]]
			});

		studioTerms.RemoveAll(term => string.IsNullOrEmpty(term.Text));
		return studioTerms;
	}

	public List<Language> GetLanguages() => InterpretBankDataContext.GetLanguages();

	private static List<string> GetTargetLanguageColumns(int languageIndex)
	{
		return new() { $"Term{languageIndex}", $"Comment{languageIndex}a", $"Comment{languageIndex}b" };
	}

	//TODO: Change to use property when event is in place
	private int GetLanguageIndex(string sourceLanguage)
	{
		return GetLanguages()
			.First(lang =>
				string.Equals(lang.Name, sourceLanguage, StringComparison.CurrentCultureIgnoreCase)).Index;
	}

	//var terms = DataContext.GetTable<GlossaryData>();
	//var result = terms.WherePropertyEquals("Term1", word);
	public void Dispose()
	{
		InterpretBankDataContext?.Dispose();
	}
}