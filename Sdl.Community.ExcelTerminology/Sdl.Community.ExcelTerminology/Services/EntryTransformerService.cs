using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.ExcelTerminology.Model;
using Sdl.Community.ExcelTerminology.Services.Interfaces;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology.Services
{
    public class EntryTransformerService: IEntryTransformerService
    {
        private readonly IParser _parser;
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
                    IsSource = false
                };
                result.Add(targetEntryLanguage);
            }


            return result;
        }

        public IList<IEntryTerm> CreateEntryTerms(string term, string approved = null)
        {
            var terms = _parser.Parse(term);

            return terms.Select(rawTerm => new EntryTerm
            {
                Fields = CreateEntryTermFields(approved),
                Value = rawTerm
            }).Cast<IEntryTerm>().ToList();
        }

        public IList<IEntryField> CreateEntryTermFields(string approved)
        {
            var result = new List<IEntryField>();
            if (string.IsNullOrEmpty(approved)) return result;
            var approvals = _parser.Parse(approved);

            result.AddRange(approvals.Select(approval => new EntryField
            {
                Name = "Approved",
                Value = approval
            }));
            return result;
        }
    }
}
