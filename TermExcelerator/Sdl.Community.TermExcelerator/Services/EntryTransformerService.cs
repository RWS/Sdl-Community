using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sdl.Community.TermExcelerator.Model;
using Sdl.Community.TermExcelerator.Services.Interfaces;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.TermExcelerator.Services
{
	public class EntryTransformerService : IEntryTransformerService
	{
		public const string ApprovedFieldName = "Approved";
		private readonly IParser _parser;

		public EntryTransformerService(IParser parser)
		{
			_parser = parser;
		}

		public IList<EntryLanguage> CreateEntryLanguages(ExcelTerm excelTerm)
		{
			var result = new List<EntryLanguage>();

			var sourceEntryLanguage = new ExcelEntryLanguage
			{
				Locale = excelTerm.SourceCulture,
				Name = excelTerm.SourceCulture.EnglishName,
				Terms = CreateEntryTerms(excelTerm.Source),
				Fields = new List<EntryField>(),
				IsSource = true
			};
			result.Add(sourceEntryLanguage);

			if (excelTerm.Target != null)
			{
				var targetEntryLanguage = new ExcelEntryLanguage
				{
					Locale = excelTerm.TargetCulture,
					Name = excelTerm.TargetCulture.EnglishName,
					Terms = CreateEntryTerms(excelTerm.Target, excelTerm.Approved),
					Fields = new List<EntryField>(),
					IsSource = false
				};
				result.Add(targetEntryLanguage);
			}
			return result;
		}

		public IList<EntryField> CreateEntryTermFields(int index, IList<string> approvals)
		{
			var result = new List<EntryField>();
			if (approvals.Count == 0) return result;
			if (index >= approvals.Count) return result;

			var entryField = new EntryField
			{
				Name = ApprovedFieldName,
				Value = approvals[index]
			};

			result.Add(entryField);
			return result;
		}

		public IList<EntryTerm> CreateEntryTerms(string term, string approved = null)
		{
			var terms = _parser.Parse(term);
			var approvals = _parser.Parse(approved);

			return terms.Select((t, i) => new EntryTerm
			{
				Fields = CreateEntryTermFields(i, approvals),
				Value = t
			}).Cast<EntryTerm>().ToList();
		}

		public ExcelEntry CreateExcelEntry(string source, string target, CultureInfo sourceLanguage, CultureInfo targetLanguage, int id = 0)
		{
			var excelTerm = GetExcelTerm(source, target, sourceLanguage, targetLanguage);

			var entryLanguages = CreateEntryLanguages(excelTerm);

			var excelEntry = new ExcelEntry
			{
				Id = id,
				Fields = new List<EntryField>(),
				Languages = entryLanguages,
				SearchText = excelTerm.Source,
				IsDirty = true
			};

			return excelEntry;
		}

		public ExcelEntry CreateExcelEntry(ExcelTerm excelTerm, int id = 0)
		{
			var entryLanguages = CreateEntryLanguages(excelTerm);

			var excelEntry = new ExcelEntry
			{
				Id = id,
				Fields = new List<EntryField>(),
				Languages = entryLanguages,
				SearchText = excelTerm.Source
			};

			return excelEntry;
		}

		public ExcelTerm GetExcelTerm(string source, string target, CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			return new ExcelTerm
			{
				SourceCulture = sourceLanguage,
				TargetCulture = targetLanguage,
				Source = source,
				Target = target
			};
		}
	}
}