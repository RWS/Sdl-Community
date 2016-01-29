using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.ExcelTerminology.Model;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology.Services
{
    public class ExcelTermProviderService
    {
        private readonly IExcelTermLoaderService _excelTermLoaderService;
        private readonly IParser _parser;
        public ExcelTermProviderService(IExcelTermLoaderService excelTermLoaderService
            ,IParser parser)
        {
            _excelTermLoaderService = excelTermLoaderService;
            _parser = parser;
        }


        public List<Entry> LoadEntries()
        {
            var excelTerms = _excelTermLoaderService.LoadTerms();

            return excelTerms.Select(excelTerm => new Entry
            {
                Id = excelTerm.Key, Languages = CreateEntryLanguages(excelTerm.Value)
            }).ToList();
        }

        public IList<IEntryLanguage> CreateEntryLanguages(ExcelTerm excelTerm)
        {
            var result = new List<IEntryLanguage>();

            var sourceEntryLanguage = new EntryLanguage
            {
                 Locale = excelTerm.SourceCulture,
                  Name = excelTerm.SourceCulture.EnglishName,
                   Terms = CreateEntryTerms(excelTerm.Source)
            };

                return result;
        }

        private IList<IEntryTerm> CreateEntryTerms(string terms)
        {
            throw new NotImplementedException();
        }

        //public static List<Term> ParseRow(List<string> row)
        //{
        //    var excelEntryList = new List<Term>();
        //    var target = ParseCell(row[1]);
        //    var status = ParseCell(row[2]);
        //    for (var i = 0; i < target.Count; i++)
        //    {
        //        excelEntryList.Add(new Term
        //        {
        //            Target = target[i],
        //            Status = status[i]
        //        });
        //    }
        //   // _textDictionary.Add(row[0], excelEntryList);
        //    target.Clear();
        //    status.Clear();
        //    return excelEntryList;
        //}

        //private static List<string> ParseCell(string cellValue)
        //{
        //    var values = cellValue.Split('|').Select(value => value.Trim()).ToList();
        //    return values;
        //}

        //var entry = new Entry
        //{
        //     Id = index,
        //      Languages = new List<IEntryLanguage>() { new EntryLanguage
        //      {
        //         Name = "",
        //          Terms = new List<IEntryTerm>()
        //          {
        //              new EntryTerm
        //              {
        //                   Value = "",
        //                    Fields = new List<IEntryField>
        //                    {
        //                        new EntryField
        //                        {

        //                        }
        //                    }
        //              }
        //          }  
        //      } }
        //};
        //result.Add(GetSource(cell, index));
        //index++;
    }
}
