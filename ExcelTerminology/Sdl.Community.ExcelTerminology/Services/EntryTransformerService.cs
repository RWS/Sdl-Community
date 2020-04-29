using System.Collections.Generic;
using System.Linq;
using Sdl.Community.ExcelTerminology.Model;
using Sdl.Community.ExcelTerminology.Services.Interfaces;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology.Services
{
	public class EntryTransformerService: IEntryTransformerService
    {
        private readonly IParser _parser;

		public const string ApprovedFieldName = "Approved";

		public EntryTransformerService(IParser parser)
        {
            _parser = parser;
        }

        public IList<IEntryLanguage> CreateEntryLanguages(ExcelTerm excelTerm)
        {
            var result = new List<IEntryLanguage>();

            var sourceEntryLanguage = new ExcelEntryLanguage
            {
                Locale = excelTerm.SourceCulture,
                Name = excelTerm.SourceCulture.EnglishName,
                Terms = CreateEntryTerms(excelTerm.Source),
                Fields = new List<IEntryField>(),
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
                    Fields = new List<IEntryField>(),
                    IsSource = false
                };
                result.Add(targetEntryLanguage);
            }
            return result;
        }

        public IList<IEntryTerm> CreateEntryTerms(string term, string approved = null)
        {
            var terms = _parser.Parse(term);
            var approvals = _parser.Parse(approved);

            return terms.Select((t, i) => new EntryTerm
            {
                Fields = CreateEntryTermFields(i, approvals), Value = t
            }).Cast<IEntryTerm>().ToList();
        }

        public IList<IEntryField> CreateEntryTermFields(int index, IList<string> approvals)
        {
            var result = new List<IEntryField>();
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
    }
}
