using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.ExcelTerminology.Model;

namespace Sdl.Community.ExcelTerminology
{
	public static class Extensions
    {
        public static string RemoveUriForbiddenCharacters(this string uriString)
        {
            var regex = new Regex(@"[$%+!*'(), ]");
            return regex.Replace(uriString, "");
        }

        public static ExcelTerm ToExcelTerm(this ExcelEntry excelEntry)
        {
            var result = new ExcelTerm();

            foreach (var entryLanguages in excelEntry.Languages.Cast<ExcelEntryLanguage>())
            {
                if (entryLanguages.IsSource)
                {
                    result.SourceCulture = entryLanguages.Locale;
                    result.Source = string.Join("|", entryLanguages.Terms.Select(x => x.Value));
                }
                else
                {
                    result.TargetCulture = entryLanguages.Locale;
                    result.Target = string.Join("|", entryLanguages.Terms.Select(x => x.Value));
                    result.Approved = string.Join("|", entryLanguages.GetTermCustomFields());
                }
            }
            return result;
        }

        public static string[] GetTermCustomFields(this ExcelEntryLanguage entryLanguages)
        {
            var result = new List<string>();
            foreach (var term in entryLanguages.Terms)
            {
                result.AddRange(term.Fields.Select(x => x.Value));
            }

            return result.ToArray();
        }
    }
}
