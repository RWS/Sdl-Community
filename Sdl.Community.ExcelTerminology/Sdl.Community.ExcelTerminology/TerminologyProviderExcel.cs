using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.ExcelTerminology.Services;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology
{
    public class TerminologyProviderExcel:ITerminologyProvider
    {
       // private  ReadExcelFile _readExcelFile;
        public const string FixedUri = "excel://terminologyproviderfactoryexcel/";
        public string Name { get; }
        public string Description { get; }
        public string Id { get { return Uri.AbsoluteUri; } }
        public Uri Uri { get; }
        public TerminologyProviderType Type { get { return TerminologyProviderType.Custom; } }
        public bool IsReadOnly { get; }
        public bool SearchEnabled { get; }
        public IDefinition Definition { get; }

        public TerminologyProviderExcel()
        {
           

            Name = "TerminologyTermbaseExcel";
            Description= "TerminologyTermbaseExcel";
            Uri = new Uri(FixedUri);
            SearchEnabled = true;
        }
        public void SetDefault(bool value)
        {
        }

        public IList<ILanguage> GetLanguages()
        {
            return new List<ILanguage>();
        }

        public IEntry GetEntry(int id)
        {
            return new Entry();
        }

        public IEntry GetEntry(int id, IEnumerable<ILanguage> languages)
        {
            return null;
        }

        public IList<ISearchResult> Search(string text, ILanguage source, ILanguage destination, int maxResultsCount, SearchMode mode,
            bool targetRequired)
        {
            return null;
        }

        public void Dispose()
        {
        }
    }
}
